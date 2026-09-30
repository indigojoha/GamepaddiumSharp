using HidSharp;
using GamepaddiumSharp.core;
using GamepaddiumSharp.mappings;

namespace GamepaddiumSharp;

public enum ConnectionType
{
    Unknown,
    Usb,
    Bluetooth
}

public interface IHidConnection : IDisposable
{
    int InputReportLength { get; }
    int OutputReportLength { get; }
    ConnectionType ConnectionType { get; }
    string DevicePath { get; }

    int VendorId { get; }
    int ProductId { get; }
    int ReleaseNumberBcd { get; }
    string ProductName { get; }
    string Manufacturer { get; }
    string? SerialNumber { get; }

    bool TryReadReport(byte[] buffer, int timeoutMs);

    void WriteReport(byte[] buffer);
}

public sealed class HidSharpConnection : IHidConnection
{
    private readonly HidDevice _device;
    private readonly HidStream _stream;

    public int InputReportLength { get; }
    public int OutputReportLength { get; }
    public ConnectionType ConnectionType { get; }
    public string DevicePath => _device.DevicePath;
    public HidDevice Device => _device;

    public int VendorId { get; }
    public int ProductId { get; }
    public int ReleaseNumberBcd { get; }
    public string ProductName { get; }
    public string Manufacturer { get; }
    public string? SerialNumber { get; }

    private HidSharpConnection(HidDevice device, HidStream stream)
    {
        _device = device;
        _stream = stream;
        InputReportLength = device.GetMaxInputReportLength();
        OutputReportLength = device.GetMaxOutputReportLength();
        ConnectionType = GuessConnectionType(device.DevicePath);

        VendorId = device.VendorID;
        ProductId = device.ProductID;
        ReleaseNumberBcd = device.ReleaseNumberBcd;

        ProductName = TryGetDeviceString(device.GetProductName) ?? $"Unknown device ({VendorId:X4}:{ProductId:X4})";
        Manufacturer = TryGetDeviceString(device.GetManufacturer) ?? "Unknown manufacturer";
        SerialNumber = TryGetDeviceString(device.GetSerialNumber);
    }

    private static string? TryGetDeviceString(Func<string> getter)
    {
        try
        {
            var value = getter();
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }
        catch
        {
            return null;
        }
    }

    public static bool TryOpen(HidDevice device, out HidSharpConnection? connection)
    {
        connection = null;
        if (!device.TryOpen(out var stream))
            return false;

        connection = new HidSharpConnection(device, stream);
        return true;
    }

    public static IEnumerable<HidSharpConnection> OpenAll(int vendorId, int productId)
    {
        foreach (var device in DeviceList.Local.GetHidDevices(vendorId, productId))
        {
            if (TryOpen(device, out var conn) && conn is not null)
                yield return conn;
        }
    }

    public static IEnumerable<HidSharpConnection> OpenAll()
    {
        foreach (var device in DeviceList.Local.GetHidDevices())
        {
            if (TryOpen(device, out var conn) && conn is not null)
                yield return conn;
        }
    }

    public static HidDevice? GetHidDevice(IHidConnection connection)
    {
        return (connection as HidSharpConnection)?._device;
    }

    public bool TryReadReport(byte[] buffer, int timeoutMs)
    {
        try
        {
            _stream.ReadTimeout = timeoutMs;
            int read = _stream.Read(buffer, 0, buffer.Length);
            return read > 0;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    public void WriteReport(byte[] buffer) => _stream.Write(buffer, 0, buffer.Length);

    public void Dispose() => _stream.Dispose();

    private static ConnectionType GuessConnectionType(string devicePath)
    {
        var p = devicePath.ToLowerInvariant();
        if (p.Contains("{00001") && p.Contains("0000-1000-8000-00805f9b34fb}"))
            return ConnectionType.Bluetooth;
        if (p.Contains("usb") || (p.Contains("vid_") && p.Contains("&pid_")))
            return ConnectionType.Usb;
        return ConnectionType.Unknown;
    }
}

public sealed class GamepadOptions
{
    public int ReadTimeoutMs { get; init; } = 100;

    public bool AutoStart { get; init; } = true;
}

public sealed class Gamepad : IDisposable
{
    private readonly IHidConnection _connection;
    private readonly IMappings _mappings;
    private readonly IOutputs _outputs;
    private readonly GamepadOptions _options;

    private readonly byte[] _outputBuffer;
    private readonly object _outputLock = new();

    private volatile byte[] _latestReport;
    private Thread? _readThread;
    private volatile bool _running;

    public ConnectionType Connection => _connection.ConnectionType;
    public string DevicePath => _connection.DevicePath;
    public bool IsConnected { get; private set; } = true;

    public ControllerDescriptor Descriptor { get; }

    public int VendorId => _connection.VendorId;
    public int ProductId => _connection.ProductId;
    public int ReleaseNumberBcd => _connection.ReleaseNumberBcd;

    public string ProductName => _connection.ProductName;

    public string Manufacturer => _connection.Manufacturer;

    public string? SerialNumber => _connection.SerialNumber;

    public event Action<Gamepad>? ReportReceived;

    public event Action<Gamepad>? Disconnected;

    public int DeviceIndex => GamepadManager.GetIndex(this);

    public Gamepad(IHidConnection connection, IMappings mappings, IOutputs outputs, ControllerDescriptor descriptor, GamepadOptions? options = null)
    {
        _connection = connection;
        _mappings = mappings;
        _outputs = outputs;
        Descriptor = descriptor;
        _options = options ?? new GamepadOptions();

        _latestReport = new byte[Math.Max(_connection.InputReportLength, 1)];

        int outputBufferSize = Math.Max(outputs.ReportLength, _connection.OutputReportLength);
        outputBufferSize = Math.Max(outputBufferSize, outputs.ReportLength + 4);

        _outputBuffer = new byte[outputBufferSize];
        _outputBuffer[0] = outputs.ReportId;

        if (_options.AutoStart)
            StartReading();
    }

    public void StartReading()
    {
        if (_running) return;
        _running = true;
        _readThread = new Thread(ReadLoop) { IsBackground = true, Name = "Gamepad-Read" };
        _readThread.Start();
    }

    public void StopReading()
    {
        _running = false;
        _readThread?.Join(500);
        _readThread = null;
    }

    private void ReadLoop()
    {
        var buffer = new byte[_connection.InputReportLength];
        while (_running)
        {
            bool ok;
            try
            {
                ok = _connection.TryReadReport(buffer, _options.ReadTimeoutMs);
            }
            catch (Exception)
            {
                IsConnected = false;
                _running = false;
                Disconnected?.Invoke(this);
                return;
            }

            if (!ok) continue;

            var snapshot = new byte[buffer.Length];
            Buffer.BlockCopy(buffer, 0, snapshot, 0, buffer.Length);
            _latestReport = snapshot;
            ReportReceived?.Invoke(this);
        }
    }

    public ReadOnlySpan<byte> Report => _latestReport;

    private Mapping GetMapping(PadDigital element) => element switch
    {
        PadDigital.A => _mappings.A,
        PadDigital.B => _mappings.B,
        PadDigital.X => _mappings.X,
        PadDigital.Y => _mappings.Y,
        PadDigital.DPadUp => _mappings.DpadUp,
        PadDigital.DPadDown => _mappings.DpadDown,
        PadDigital.DPadLeft => _mappings.DpadLeft,
        PadDigital.DPadRight => _mappings.DpadRight,
        PadDigital.LeftButton => _mappings.LB,
        PadDigital.LeftTriggerHeld => _mappings.LTB_,
        PadDigital.LeftTriggerEndButton => _mappings.LTB,
        PadDigital.LeftStickButton => _mappings.L3,
        PadDigital.RightButton => _mappings.RB,
        PadDigital.RightTriggerHeld => _mappings.RTB_,
        PadDigital.RightTriggerEndButton => _mappings.RTB,
        PadDigital.RightStickButton => _mappings.R3,
        PadDigital.Select => _mappings.Select,
        PadDigital.Start => _mappings.Start,
        PadDigital.Share => _mappings.Share,
        PadDigital.Super => _mappings.Super,
        PadDigital.MuteMic => _mappings.MuteMic,
        PadDigital.VolumeDown => _mappings.VolumeDown,
        PadDigital.VolumeUp => _mappings.VolumeUp,
        PadDigital.BrightnessCycle => _mappings.BrightnessCycle,
        PadDigital.WiiOne => _mappings.WiiOne,
        PadDigital.WiiTwo => _mappings.WiiTwo,
        PadDigital.SideLeft => _mappings.SideLeft,
        PadDigital.SideRight => _mappings.SideRight,
        PadDigital.GripLeft => _mappings.GL,
        PadDigital.GripRight => _mappings.GR,
        PadDigital.GameChat => _mappings.GameChat,
        _ => throw new ArgumentException($"Unaccounted PadDigital enum.", nameof(element))
    };

    public AnalogMapping GetMapping(PadAnalog element) => element switch
    {
        PadAnalog.LeftStickX => _mappings.LeftStickX,
        PadAnalog.LeftStickY => _mappings.LeftStickY,
        PadAnalog.RightStickX => _mappings.RightStickX,
        PadAnalog.RightStickY => _mappings.RightStickY,
        PadAnalog.LeftTrigger => _mappings.LT,
        PadAnalog.RightTrigger => _mappings.RT,
        PadAnalog.BatteryLevel => _mappings.BatteryLevel,
        PadAnalog.BatteryStatus => _mappings.BatteryStatus,
        PadAnalog.Temperature => _mappings.Temperature,
        PadAnalog.GyroX => _mappings.GyroX,
        PadAnalog.GyroY => _mappings.GyroY,
        PadAnalog.GyroZ => _mappings.GyroZ,
        PadAnalog.AccelerometerX => _mappings.AccelX,
        PadAnalog.AccelerometerY => _mappings.AccelY,
        PadAnalog.AccelerometerZ => _mappings.AccelZ,
        PadAnalog.TouchpadFinger1X => _mappings.TouchpadFinger1X,
        PadAnalog.TouchpadFinger1Y => _mappings.TouchpadFinger1Y,
        PadAnalog.TouchpadFinger2X => _mappings.TouchpadFinger2X,
        PadAnalog.TouchpadFinger2Y => _mappings.TouchpadFinger2Y,
        PadAnalog.PointerX => _mappings.PointerX,
        PadAnalog.PointerY => _mappings.PointerY,
        PadAnalog.PointerRotation => _mappings.PointerRotation,
        PadAnalog.MouseVelocityX => _mappings.MouseVelX,
        PadAnalog.MouseVelocityY => _mappings.MouseVelY,
        _ => throw new ArgumentException($"Unaccounted PadAnalog enum.", nameof(element))
    };

    public bool IsPressed(PadDigital element) => GetMapping(element).Read(Report);
    public double ReadAnalog(PadAnalog element) => GetMapping(element).Read(Report);

    public bool HasMapping(PadDigital element)
    {
        return GetMapping(element) != Atom.ZeroFalse;
    }

    public bool HasMapping(PadAnalog element)
    {
        return GetMapping(element) != Atom.ZeroFalse;
    }

    private bool oBatching = false;

    public void StartSetBatch()
    {
        oBatching = true;
    }

    public void EndSetBatch()
    {
        lock (_outputLock) Flush();
        oBatching = false;
    }

    public void SetRumbleC(byte weak, byte strong)
    {
        lock (_outputLock)
        {
            _outputs.MotorCenterWeak.Write(_outputBuffer, weak);
            _outputs.MotorCenterStrong.Write(_outputBuffer, strong);
            if (!oBatching) Flush();
        }
    }

    public void SetRumbleLR(byte left, byte right)
    {
        lock (_outputLock)
        {
            _outputs.MotorLeft.Write(_outputBuffer, left);
            _outputs.MotorRight.Write(_outputBuffer, right);
            if (!oBatching) Flush();
        }
    }

    public void SetLightbar(byte r, byte g, byte b)
    {
        lock (_outputLock)
        {
            _outputs.LightbarRed.Write(_outputBuffer, r);
            _outputs.LightbarGreen.Write(_outputBuffer, g);
            _outputs.LightbarBlue.Write(_outputBuffer, b);
            if (!oBatching) Flush();
        }
    }

    public void SetPlayerLeds(byte mask)
    {
        lock (_outputLock)
        {
            _outputs.PlayerLeds.Write(_outputBuffer, mask);
            if (!oBatching) Flush();
        }
    }

    public void SetMicLed(bool on)
    {
        lock (_outputLock)
        {
            _outputs.MicLed.Write(_outputBuffer, on ? 1 : 0);
            if (!oBatching) Flush();
        }
    }

    public void SetLeftTrigger(AdaptiveTriggerEffect effect)
    {
        lock (_outputLock)
        {
            _outputs.WriteLeftTrigger(_outputBuffer, effect);
            if (!oBatching) Flush();
        }
    }

    public void SetRightTrigger(AdaptiveTriggerEffect effect)
    {
        lock (_outputLock)
        {
            _outputs.WriteRightTrigger(_outputBuffer, effect);
            if (!oBatching) Flush();
        }
    }

    private void Flush()
    {
        try
        {
            if (_outputs is DualSenseBtOutputs btOutputs)
            {
                Span<byte> reportWithCrc = _outputBuffer.AsSpan(0, btOutputs.ReportLength + 4);
                _outputs.WriteCrc(reportWithCrc);
                _connection.WriteReport(reportWithCrc.ToArray());
            }
            else
            {
                _outputs.WriteCrc(_outputBuffer);
                _connection.WriteReport(_outputBuffer);
            }
        }
        catch (IOException ex)
        {
            GamepadLog.Sink?.Invoke($"Failed to write output report: {ex.Message}");
        }
    }

    public void Dispose()
    {
        StopReading();
        _connection.Dispose();
    }

    public override string ToString() =>
        $"{ProductName} ({Manufacturer}) [{VendorId:X4}:{ProductId:X4}] via {Connection}";
}
