using HidSharp;
using GamepaddiumSharp.core;
using GamepaddiumSharp.core.blackmagic;

namespace GamepaddiumSharp.mappings;

public sealed class GenericHidMappings : IMappings
{
    private readonly StumpMappings _underlying;

    public Action<HidDevice> OnDeviceAttached { get; set; } = _ => { };

    public Mapping A
    {
        get => _underlying.A;
        set => _underlying.A = value;
    }

    public Mapping B
    {
        get => _underlying.B;
        set => _underlying.B = value;
    }

    public Mapping X
    {
        get => _underlying.X;
        set => _underlying.X = value;
    }

    public Mapping Y
    {
        get => _underlying.Y;
        set => _underlying.Y = value;
    }

    public Mapping DpadUp
    {
        get => _underlying.DpadUp;
        set => _underlying.DpadUp = value;
    }

    public Mapping DpadDown
    {
        get => _underlying.DpadDown;
        set => _underlying.DpadDown = value;
    }

    public Mapping DpadLeft
    {
        get => _underlying.DpadLeft;
        set => _underlying.DpadLeft = value;
    }

    public Mapping DpadRight
    {
        get => _underlying.DpadRight;
        set => _underlying.DpadRight = value;
    }

    public AnalogMapping LeftStickX
    {
        get => _underlying.LeftStickX;
        set => _underlying.LeftStickX = value;
    }

    public AnalogMapping LeftStickY
    {
        get => _underlying.LeftStickY;
        set => _underlying.LeftStickY = value;
    }

    public AnalogMapping RightStickX
    {
        get => _underlying.RightStickX;
        set => _underlying.RightStickX = value;
    }

    public AnalogMapping RightStickY
    {
        get => _underlying.RightStickY;
        set => _underlying.RightStickY = value;
    }

    public Mapping LB
    {
        get => _underlying.LB;
        set => _underlying.LB = value;
    }

    public Mapping RB
    {
        get => _underlying.RB;
        set => _underlying.RB = value;
    }

    public AnalogMapping LT
    {
        get => _underlying.LT;
        set => _underlying.LT = value;
    }

    public Mapping LTB_
    {
        get => _underlying.LTB_;
        set => _underlying.LTB_ = value;
    }

    public Mapping LTB
    {
        get => _underlying.LTB;
        set => _underlying.LTB = value;
    }

    public AnalogMapping RT
    {
        get => _underlying.RT;
        set => _underlying.RT = value;
    }

    public Mapping RTB_
    {
        get => _underlying.RTB_;
        set => _underlying.RTB_ = value;
    }

    public Mapping RTB
    {
        get => _underlying.RTB;
        set => _underlying.RTB = value;
    }

    public Mapping L3
    {
        get => _underlying.L3;
        set => _underlying.L3 = value;
    }

    public Mapping R3
    {
        get => _underlying.R3;
        set => _underlying.R3 = value;
    }

    public Mapping Select
    {
        get => _underlying.Select;
        set => _underlying.Select = value;
    }

    public Mapping Start
    {
        get => _underlying.Start;
        set => _underlying.Start = value;
    }

    public Mapping Share
    {
        get => _underlying.Share;
        set => _underlying.Share = value;
    }

    public Mapping Super
    {
        get => _underlying.Super;
        set => _underlying.Super = value;
    }

    public AnalogMapping BatteryLevel
    {
        get => _underlying.BatteryLevel;
        set => _underlying.BatteryLevel = value;
    }

    public AnalogMapping BatteryStatus
    {
        get => _underlying.BatteryStatus;
        set => _underlying.BatteryStatus = value;
    }

    public AnalogMapping Temperature
    {
        get => _underlying.Temperature;
        set => _underlying.Temperature = value;
    }

    public AnalogMapping GyroX
    {
        get => _underlying.GyroX;
        set => _underlying.GyroX = value;
    }

    public AnalogMapping GyroY
    {
        get => _underlying.GyroY;
        set => _underlying.GyroY = value;
    }

    public AnalogMapping GyroZ
    {
        get => _underlying.GyroZ;
        set => _underlying.GyroZ = value;
    }

    public AnalogMapping AccelX
    {
        get => _underlying.AccelX;
        set => _underlying.AccelX = value;
    }

    public AnalogMapping AccelY
    {
        get => _underlying.AccelY;
        set => _underlying.AccelY = value;
    }

    public AnalogMapping AccelZ
    {
        get => _underlying.AccelZ;
        set => _underlying.AccelZ = value;
    }

    public AnalogMapping TouchpadFinger1X
    {
        get => _underlying.TouchpadFinger1X;
        set => _underlying.TouchpadFinger1X = value;
    }

    public AnalogMapping TouchpadFinger1Y
    {
        get => _underlying.TouchpadFinger1Y;
        set => _underlying.TouchpadFinger1Y = value;
    }

    public AnalogMapping TouchpadFinger2X
    {
        get => _underlying.TouchpadFinger2X;
        set => _underlying.TouchpadFinger2X = value;
    }

    public AnalogMapping TouchpadFinger2Y
    {
        get => _underlying.TouchpadFinger2Y;
        set => _underlying.TouchpadFinger2Y = value;
    }

    public Mapping MuteMic
    {
        get => _underlying.MuteMic;
        set => _underlying.MuteMic = value;
    }

    public AnalogMapping PointerX
    {
        get => _underlying.PointerX;
        set => _underlying.PointerX = value;
    }

    public AnalogMapping PointerY
    {
        get => _underlying.PointerY;
        set => _underlying.PointerY = value;
    }

    public AnalogMapping PointerRotation
    {
        get => _underlying.PointerRotation;
        set => _underlying.PointerRotation = value;
    }

    public Mapping WiiOne
    {
        get => _underlying.WiiOne;
        set => _underlying.WiiOne = value;
    }

    public Mapping WiiTwo
    {
        get => _underlying.WiiTwo;
        set => _underlying.WiiTwo = value;
    }

    public Mapping SideLeft
    {
        get => _underlying.SideLeft;
        set => _underlying.SideLeft = value;
    }

    public Mapping SideRight
    {
        get => _underlying.SideRight;
        set => _underlying.SideRight = value;
    }

    public Mapping GL
    {
        get => _underlying.GL;
        set => _underlying.GL = value;
    }

    public Mapping GR
    {
        get => _underlying.GR;
        set => _underlying.GR = value;
    }

    public AnalogMapping MouseVelX
    {
        get => _underlying.MouseVelX;
        set => _underlying.MouseVelX = value;
    }

    public AnalogMapping MouseVelY
    {
        get => _underlying.MouseVelY;
        set => _underlying.MouseVelY = value;
    }

    public Mapping GameChat
    {
        get => _underlying.GameChat;
        set => _underlying.GameChat = value;
    }

    public Mapping VolumeDown
    {
        get => _underlying.VolumeDown;
        set => _underlying.VolumeDown = value;
    }

    public Mapping VolumeUp
    {
        get => _underlying.VolumeUp;
        set => _underlying.VolumeUp = value;
    }

    public Mapping BrightnessCycle
    {
        get => _underlying.BrightnessCycle;
        set => _underlying.BrightnessCycle = value;
    }

    public static GenericHidMappings CreateFromDevice(HidDevice device)
    {
        var (mappings, _) = HidParser.CompileFrom(device);
        return new GenericHidMappings(mappings);
    }

    private GenericHidMappings(StumpMappings underlying)
    {
        _underlying = underlying ?? throw new ArgumentNullException(nameof(underlying));
    }
}

public sealed class GenericHidOutputs : IOutputs
{
    public Action<HidDevice> OnDeviceAttached { get; set; } = _ => { };

    private readonly StumpOutputs _underlying;

    public OutputField MotorCenterWeak
    {
        get => _underlying.MotorCenterWeak;
        set => _underlying.MotorCenterWeak = value;
    }

    public OutputField MotorCenterStrong
    {
        get => _underlying.MotorCenterStrong;
        set => _underlying.MotorCenterStrong = value;
    }

    public OutputField MotorLeft
    {
        get => _underlying.MotorLeft;
        set => _underlying.MotorLeft = value;
    }

    public OutputField MotorRight
    {
        get => _underlying.MotorRight;
        set => _underlying.MotorRight = value;
    }

    public OutputField LightbarRed
    {
        get => _underlying.LightbarRed;
        set => _underlying.LightbarRed = value;
    }

    public OutputField LightbarGreen
    {
        get => _underlying.LightbarGreen;
        set => _underlying.LightbarGreen = value;
    }

    public OutputField LightbarBlue
    {
        get => _underlying.LightbarBlue;
        set => _underlying.LightbarBlue = value;
    }

    public OutputField PlayerLeds
    {
        get => _underlying.PlayerLeds;
        set => _underlying.PlayerLeds = value;
    }

    public OutputField MicLed
    {
        get => _underlying.MicLed;
        set => _underlying.MicLed = value;
    }

    public int ReportLength
    {
        get => _underlying.ReportLength;
        set => _underlying.ReportLength = value;
    }

    public byte ReportId
    {
        get => _underlying.ReportId;
        set => _underlying.ReportId = value;
    }

    public void WriteLeftTrigger(Span<byte> report, AdaptiveTriggerEffect effect)
    {
        _underlying.WriteLeftTrigger(report, effect);
    }

    public void WriteRightTrigger(Span<byte> report, AdaptiveTriggerEffect effect)
    {
        _underlying.WriteRightTrigger(report, effect);
    }

    public void WriteCrc(Span<byte> report)
    {
        _underlying.WriteCrc(report);
    }

    public static GenericHidOutputs CreateFromDevice(HidDevice device)
    {
        var (_, outputs) = HidParser.CompileFrom(device);
        return new GenericHidOutputs(outputs);
    }

    private GenericHidOutputs(StumpOutputs underlying)
    {
        _underlying = underlying ?? throw new ArgumentNullException(nameof(underlying));
    }
}
