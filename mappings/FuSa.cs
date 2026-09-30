using GamepaddiumSharp.core;
using HidSharp;

namespace GamepaddiumSharp.mappings;

public sealed class FuSaMappings : IMappings
{
    public Action<HidDevice> OnDeviceAttached { get; set; } = _ => { };

    public Mapping A { get; set; } = Atom.ZeroFalse;
    public Mapping B { get; set; } = Atom.ZeroFalse;
    public Mapping X { get; set; } = Atom.ZeroFalse;
    public Mapping Y { get; set; } = Atom.ZeroFalse;

    private static readonly Atom HatAtom = Atom.ZeroFalse;
    public Mapping DpadUp { get; set; } = new InSetMapping(HatAtom, 1, 2, 8);
    public Mapping DpadRight { get; set; } = new InSetMapping(HatAtom, 2, 3, 4);
    public Mapping DpadDown { get; set; } = new InSetMapping(HatAtom, 4, 5, 6);
    public Mapping DpadLeft { get; set; } = new InSetMapping(HatAtom, 6, 7, 8);

    public AnalogMapping LeftStickX { get; set; } = Atom.ZeroFalse;
    public AnalogMapping LeftStickY { get; set; } = Atom.ZeroFalse;
    public AnalogMapping RightStickX { get; set; } = Atom.ZeroFalse;
    public AnalogMapping RightStickY { get; set; } = Atom.ZeroFalse;

    public Mapping LB { get; set; } = Atom.Byte(1, AtomIntDouble.NZP);
    public Mapping RB { get; set; } = Atom.Byte(1, AtomIntDouble.NZP);

    public AnalogMapping LT { get; set; } = Atom.ZeroFalse;
    public AnalogMapping RT { get; set; } = Atom.ZeroFalse;

    public Mapping LTB_ { get; set; } = Atom.ZeroFalse;
    public Mapping RTB_ { get; set; } = Atom.ZeroFalse;

    public Mapping LTB { get; set; } = Atom.ZeroFalse; // no end-of-travel digibutton
    public Mapping RTB { get; set; } = Atom.ZeroFalse; // ditto

    public Mapping L3 { get; set; } = Atom.ZeroFalse;
    public Mapping R3 { get; set; } = Atom.ZeroFalse;

    public Mapping Select { get; set; } = Atom.Bit(6, 5);
    public Mapping Start { get; set; } = Atom.Bit(6, 4);
    public Mapping Share { get; set; } = Atom.ZeroFalse;
    public Mapping Super { get; set; } = Atom.Bit(6, 3);

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


    // not applicable

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

    public Mapping VolumeDown { get; set; } = Atom.Bit(6, 2);
    public Mapping VolumeUp { get; set; } = Atom.Bit(6, 1);
    public Mapping BrightnessCycle { get; set; } = Atom.Bit(6, 0);
}

public sealed class FuSaOutputs : IOutputs
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
