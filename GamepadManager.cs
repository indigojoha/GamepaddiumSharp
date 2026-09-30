using GamepaddiumSharp.core;

namespace GamepaddiumSharp;

public static class GamepadManager
{
    private static readonly List<ControllerDescriptor> _descriptors = [];
    private static readonly object _lock = new();

    private static readonly Dictionary<string, Gamepad> _byDevicePath = [];
    private static readonly Dictionary<Gamepad, bool[]> _lastButtonStates = [];
    private static readonly Dictionary<Gamepad, double[]> _lastAxisValues = [];

    private static readonly List<Gamepad?> _byIndex = [];
    private static readonly Dictionary<Gamepad, int> _indexByGamepad = [];

    private static GamepadOptions _gamepadOptions = new();
    private static Timer? _pollTimer;
    private static bool _initialized;

    public static double AxisActivityThreshold { get; set; } = 0.4;

    public static IReadOnlyCollection<Gamepad> Gamepads
    {
        get
        {
            lock (_lock)
                return [.. _byDevicePath.Values];
        }
    }

    public static int IndexCount
    {
        get
        {
            lock (_lock)
                return _byIndex.Count;
        }
    }

    public static IReadOnlyCollection<int> ConnectedIndices
    {
        get
        {
            lock (_lock)
                return [.. _indexByGamepad.Values.OrderBy(i => i)];
        }
    }

    public static Gamepad? LastUsedGamepad { get; private set; }

    public static event Action<Gamepad>? ControllerConnected;

    public static event Action<Gamepad>? ControllerDisconnected;

    public static event Action<Gamepad, PadDigital, PadAnalog>? OnImportantControllerActivity;
    public static event Action<Gamepad>? LastUsedGamepadChanged;

    public static event Action<Gamepad, int>? ControllerIndexAssigned;

    public static event Action<Gamepad, int>? ControllerIndexFreed;

    public static void Initialize(IEnumerable<ControllerDescriptor>? additionalDescriptors = null, GamepadOptions? gamepadOptions = null)
    {
        lock (_lock)
        {
            _descriptors.Clear();
            _descriptors.AddRange(ControllerDescriptorRegister.List);
            if (additionalDescriptors is not null) _descriptors.AddRange(additionalDescriptors);
            _gamepadOptions = gamepadOptions ?? new GamepadOptions();
            _initialized = true;
        }
    }

    public static void RegisterDescriptor(ControllerDescriptor descriptor)
    {
        lock (_lock)
            _descriptors.Add(descriptor);
    }

    public static Gamepad? GetGamepad(int index)
    {
        lock (_lock)
            return index >= 0 && index < _byIndex.Count ? _byIndex[index] : null;
    }

    public static int GetIndex(Gamepad pad)
    {
        lock (_lock)
            if (_indexByGamepad.TryGetValue(pad, out int index))
                return index;

        return -1;
    }

    public static bool TryGetIndex(Gamepad pad, out int index)
    {
        lock (_lock)
            return _indexByGamepad.TryGetValue(pad, out index);
    }

    public static void Refresh()
    {
        EnsureInitialized();

        var seenPaths = new HashSet<string>();

        var sortedDescriptors = _descriptors.OrderBy(d => d.Connection switch
        {
            ConnectionType.Usb => 1,
            ConnectionType.Bluetooth => 0,
            _ => 2
        }).ToList();

        foreach (var descriptor in sortedDescriptors)
        {
            IEnumerable<HidSharpConnection> connections;

            if (descriptor.VendorId == -1 && descriptor.ProductId == -1)
            {
                connections = HidSharpConnection.OpenAll();
            }
            else
            {
                connections = HidSharpConnection.OpenAll(descriptor.VendorId, descriptor.ProductId);
            }

            foreach (var conn in connections)
            {
                if (seenPaths.Contains(conn.DevicePath))
                {
                    conn.Dispose();
                    continue;
                }

                if (!MatchesConnectionType(descriptor.Connection, conn.ConnectionType))
                {
                    conn.Dispose();
                    continue;
                }

                bool alreadyTracked;
                lock (_lock)
                    alreadyTracked = _byDevicePath.ContainsKey(conn.DevicePath);

                if (alreadyTracked)
                {
                    seenPaths.Add(conn.DevicePath);
                    conn.Dispose();
                    continue;
                }

                seenPaths.Add(conn.DevicePath);

                if (descriptor.VendorId == -1 && descriptor.ProductId == -1 && conn.ConnectionType == ConnectionType.Unknown)
                {
                    bool hasExistingDevice;
                    lock (_lock)
                        hasExistingDevice = _byDevicePath.Values.Any(g => 
                            g.Descriptor.VendorId == conn.VendorId && 
                            g.Descriptor.ProductId == conn.ProductId);

                    if (hasExistingDevice)
                    {
                        conn.Dispose();
                        continue;
                    }
                }

                Gamepad pad;
                try
                {
                    var mappings = descriptor.MappingsFactory(conn);
                    var outputs = descriptor.OutputsFactory(conn);

                    mappings.OnDeviceAttached?.Invoke(conn.Device);
                    outputs.OnDeviceAttached?.Invoke(conn.Device);

                    pad = new Gamepad(conn, mappings, outputs, descriptor, _gamepadOptions);
                }
                catch
                {
                    conn.Dispose();
                    continue;
                }

                TrackNewGamepad(pad);
            }
        }

        PruneMissing(seenPaths);
    }

    public static void StartAutoRefresh(int intervalMs = 1000)
    {
        EnsureInitialized();

        StopAutoRefresh();
        Refresh();
        _pollTimer = new Timer(_ => Refresh(), null, intervalMs, intervalMs);
    }

    public static void StopAutoRefresh()
    {
        _pollTimer?.Dispose();
        _pollTimer = null;
    }

    public static void Shutdown()
    {
        StopAutoRefresh();

        List<Gamepad> pads;
        lock (_lock)
            pads = [.. _byDevicePath.Values];

        foreach (var pad in pads)
            RemoveGamepad(pad, disposeConnection: true);

        lock (_lock)
        {
            _descriptors.Clear();
            _initialized = false;
        }
    }

    private static void EnsureInitialized()
    {
        if (!_initialized)
            throw new InvalidOperationException($"{nameof(GamepadManager)} must be initialized via {nameof(Initialize)} before use.");
    }

    private static bool MatchesConnectionType(ConnectionType descriptorConnection, ConnectionType actualConnection)
    {
        if (actualConnection == ConnectionType.Unknown)
            return descriptorConnection == ConnectionType.Unknown;

        return descriptorConnection == ConnectionType.Unknown || descriptorConnection == actualConnection;
    }

    private static void TrackNewGamepad(Gamepad pad)
    {
        var buttonStates = new bool[GamepaddiumEnumTools.DigitalList.Length];
        var axisValues = new double[GamepaddiumEnumTools.AnalogList.Length];

        for (int i = 0; i < GamepaddiumEnumTools.DigitalList.Length; i++)
            buttonStates[i] = SafeIsPressed(pad, GamepaddiumEnumTools.DigitalList[i]);

        for (int i = 0; i < GamepaddiumEnumTools.AnalogList.Length; i++)
            axisValues[i] = SafeReadAnalog(pad, GamepaddiumEnumTools.AnalogList[i]);

        int assignedIndex;

        lock (_lock)
        {
            _byDevicePath[pad.DevicePath] = pad;
            _lastButtonStates[pad] = buttonStates;
            _lastAxisValues[pad] = axisValues;

            assignedIndex = AssignIndex(pad);
        }

        pad.ReportReceived += OnReportReceived;
        pad.Disconnected += OnGamepadDisconnected;

        ControllerConnected?.Invoke(pad);
        ControllerIndexAssigned?.Invoke(pad, assignedIndex);
    }

    // holding _lock
    private static int AssignIndex(Gamepad pad)
    {
        for (int i = 0; i < _byIndex.Count; i++)
        {
            if (_byIndex[i] is null)
            {
                _byIndex[i] = pad;
                _indexByGamepad[pad] = i;
                return i;
            }
        }

        _byIndex.Add(pad);
        int index = _byIndex.Count - 1;
        _indexByGamepad[pad] = index;
        return index;
    }

    // holding _lock
    private static int? FreeIndex(Gamepad pad)
    {
        if (!_indexByGamepad.Remove(pad, out int index))
            return null;

        _byIndex[index] = null;

        while (_byIndex.Count > 0 && _byIndex[^1] is null)
            _byIndex.RemoveAt(_byIndex.Count - 1);

        return index;
    }

    private static void PruneMissing(HashSet<string> seenPaths)
    {
        List<Gamepad> stale;
        lock (_lock)
        {
            stale = [.. _byDevicePath
                .Where(kv => !seenPaths.Contains(kv.Key))
                .Select(kv => kv.Value)];
        }

        foreach (var pad in stale)
            RemoveGamepad(pad, disposeConnection: true);
    }

    private static void OnReportReceived(Gamepad pad)
    {
        bool activity = false;
        PadDigital button = PadDigital.Invalid;
        PadAnalog axis = PadAnalog.Invalid;

        lock (_lock)
        {
            if (!_lastButtonStates.TryGetValue(pad, out var lastButtons) ||
                !_lastAxisValues.TryGetValue(pad, out var lastAxes))
                return;

            for (int i = 0; i < GamepaddiumEnumTools.DigitalList.Length; i++)
            {
                bool current = SafeIsPressed(pad, GamepaddiumEnumTools.DigitalList[i]);
                if (current != lastButtons[i])
                {
                    activity = true;
                    button = GamepaddiumEnumTools.DigitalList[i];
                }
                lastButtons[i] = current;
            }

            foreach (var importantAxis in GamepaddiumEnumTools.ImportantAnalogList)
            {
                int idx = Array.IndexOf(GamepaddiumEnumTools.AnalogList, importantAxis);
                if (idx < 0) continue;

                double current = SafeReadAnalog(pad, importantAxis);
                if (Math.Abs(current - lastAxes[idx]) >= AxisActivityThreshold)
                {
                    activity = true;
                    axis = importantAxis;
                }
                lastAxes[idx] = current;
            }
        }

        if (activity)
            MarkLastUsed(pad, button, axis);
    }

    private static void MarkLastUsed(Gamepad pad, PadDigital button, PadAnalog axis)
    {
        OnImportantControllerActivity?.Invoke(pad, button, axis);

        if (ReferenceEquals(LastUsedGamepad, pad))
            return;

        LastUsedGamepad = pad;
        LastUsedGamepadChanged?.Invoke(pad);
    }

    private static void OnGamepadDisconnected(Gamepad pad) => RemoveGamepad(pad, disposeConnection: false);

    private static void RemoveGamepad(Gamepad pad, bool disposeConnection)
    {
        int? freedIndex;

        lock (_lock)
        {
            _byDevicePath.Remove(pad.DevicePath);
            _lastButtonStates.Remove(pad);
            _lastAxisValues.Remove(pad);

            freedIndex = FreeIndex(pad);
        }

        pad.ReportReceived -= OnReportReceived;
        pad.Disconnected -= OnGamepadDisconnected;

        if (ReferenceEquals(LastUsedGamepad, pad))
            LastUsedGamepad = null;

        ControllerDisconnected?.Invoke(pad);

        if (freedIndex is int idx)
            ControllerIndexFreed?.Invoke(pad, idx);

        if (disposeConnection)
            pad.Dispose();
    }

    private static bool SafeIsPressed(Gamepad pad, PadDigital element)
    {
        try { return pad.IsPressed(element); }
        catch { return false; }
    }

    private static double SafeReadAnalog(Gamepad pad, PadAnalog element)
    {
        try { return pad.ReadAnalog(element); }
        catch { return 0; }
    }
}
