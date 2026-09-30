using HidSharp;
using GamepaddiumSharp.core;

namespace GamepaddiumSharp.mappings;

public sealed class StumpMappings : IMappings
{
    public Action<HidDevice> OnDeviceAttached { get; set; } = _ => { };

    public Mapping A { get; set; } = Atom.ZeroFalse;
    public Mapping B { get; set; } = Atom.ZeroFalse;
    public Mapping X { get; set; } = Atom.ZeroFalse;
    public Mapping Y { get; set; } = Atom.ZeroFalse;

    public Mapping DpadUp { get; set; } = Atom.ZeroFalse;
    public Mapping DpadRight { get; set; } = Atom.ZeroFalse;
    public Mapping DpadDown { get; set; } = Atom.ZeroFalse;
    public Mapping DpadLeft { get; set; } = Atom.ZeroFalse;

    public AnalogMapping LeftStickX { get; set; } = Atom.ZeroFalse;
    public AnalogMapping LeftStickY { get; set; } = Atom.ZeroFalse;
    public AnalogMapping RightStickX { get; set; } = Atom.ZeroFalse;
    public AnalogMapping RightStickY { get; set; } = Atom.ZeroFalse;

    public Mapping LB { get; set; } = Atom.ZeroFalse;
    public Mapping RB { get; set; } = Atom.ZeroFalse;

    public AnalogMapping LT { get; set; } = Atom.ZeroFalse;
    public AnalogMapping RT { get; set; } = Atom.ZeroFalse;

    public Mapping LTB_ { get; set; } = Atom.ZeroFalse;
    public Mapping RTB_ { get; set; } = Atom.ZeroFalse;

    public Mapping LTB { get; set; } = Atom.ZeroFalse; // end-of-travel digibutton
    public Mapping RTB { get; set; } = Atom.ZeroFalse; // ditto

    public Mapping L3 { get; set; } = Atom.ZeroFalse;
    public Mapping R3 { get; set; } = Atom.ZeroFalse;

    public Mapping Select { get; set; } = Atom.ZeroFalse;
    public Mapping Start { get; set; } = Atom.ZeroFalse;
    public Mapping Share { get; set; } = Atom.ZeroFalse;
    public Mapping Super { get; set; } = Atom.ZeroFalse;

    public Mapping MuteMic { get; set; } = Atom.ZeroFalse;


    // misc

    public AnalogMapping BatteryLevel { get; set; } = Atom.ZeroFalse;
    public AnalogMapping BatteryStatus { get; set; } = Atom.ZeroFalse;
    public AnalogMapping Temperature { get; set; } = Atom.ZeroFalse;


    // motion

    public AnalogMapping GyroX { get; set; } = Atom.ZeroFalse;
    public AnalogMapping GyroY { get; set; } = Atom.ZeroFalse;
    public AnalogMapping GyroZ { get; set; } = Atom.ZeroFalse;
    public AnalogMapping AccelX { get; set; } = Atom.ZeroFalse;
    public AnalogMapping AccelY { get; set; } = Atom.ZeroFalse;
    public AnalogMapping AccelZ { get; set; } = Atom.ZeroFalse;

    public AnalogMapping TouchpadFinger1X { get; set; } = Atom.ZeroFalse;
    public AnalogMapping TouchpadFinger1Y { get; set; } = Atom.ZeroFalse;
    public AnalogMapping TouchpadFinger2X { get; set; } = Atom.ZeroFalse;
    public AnalogMapping TouchpadFinger2Y { get; set; } = Atom.ZeroFalse;


    public AnalogMapping PointerX { get; set; } = Atom.ZeroFalse;
    public AnalogMapping PointerY { get; set; } = Atom.ZeroFalse;
    public AnalogMapping PointerRotation { get; set; } = Atom.ZeroFalse;
    public Mapping WiiOne { get; set; } = Atom.ZeroFalse;
    public Mapping WiiTwo { get; set; } = Atom.ZeroFalse;

    public Mapping SideLeft { get; set; } = Atom.ZeroFalse;
    public Mapping SideRight { get; set; } = Atom.ZeroFalse;

    public Mapping GL { get; set; } = Atom.ZeroFalse;
    public Mapping GR { get; set; } = Atom.ZeroFalse;
    public AnalogMapping MouseVelX { get; set; } = Atom.ZeroFalse;
    public AnalogMapping MouseVelY { get; set; } = Atom.ZeroFalse;
    public Mapping GameChat { get; set; } = Atom.ZeroFalse;

    public Mapping VolumeDown { get; set; } = Atom.ZeroFalse;
    public Mapping VolumeUp { get; set; } = Atom.ZeroFalse;
    public Mapping BrightnessCycle { get; set; } = Atom.ZeroFalse;
}

public sealed class StumpOutputs : IOutputs
{
    public Action<HidDevice> OnDeviceAttached { get; set; } = _ => { };

    public byte ReportId { get; set; } = 0x00;
    public int ReportLength { get; set; } = 0;


    public OutputField MotorCenterWeak { get; set; } = new();
    public OutputField MotorCenterStrong { get; set; } = new();
    public OutputField MotorRight { get; set; } = new();
    public OutputField MotorLeft { get; set; } = new();

    public OutputField MicLed { get; set; } = new();

    public OutputField LightbarRed { get; set; } = new();
    public OutputField LightbarGreen { get; set; } = new();
    public OutputField LightbarBlue { get; set; } = new();
    public OutputField PlayerLeds { get; set; } = new();

    public void WriteLeftTrigger(Span<byte> report, AdaptiveTriggerEffect effect) { }

    public void WriteRightTrigger(Span<byte> report, AdaptiveTriggerEffect effect) { }

    public void WriteCrc(Span<byte> report) { }
}
