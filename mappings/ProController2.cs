using HidSharp;
using GamepaddiumSharp.core;

namespace GamepaddiumSharp.mappings;

public sealed class ProController2Mappings : IMappings
{
    public Action<HidDevice> OnDeviceAttached { get; set; } = _ => { };

    public Mapping A { get; set; } = Atom.Bit(5, 4);
    public Mapping B { get; set; } = Atom.Bit(5, 5);
    public Mapping X { get; set; } = Atom.Bit(5, 6);
    public Mapping Y { get; set; } = Atom.Bit(5, 7);

    public Mapping DpadUp { get; set; } = Atom.Bit(7, 6);
    public Mapping DpadRight { get; set; } = Atom.Bit(7, 5);
    public Mapping DpadDown { get; set; } = Atom.Bit(7, 7);
    public Mapping DpadLeft { get; set; } = Atom.Bit(7, 4);

    public AnalogMapping LeftStickX { get; set; } = Atom.DoubleFromUInt12(12, true, AtomIntDouble.NZP);
    public AnalogMapping LeftStickY { get; set; } = Atom.DoubleFromUInt12(11, false, AtomIntDouble.NZP); // this is likely wrong
    public AnalogMapping RightStickX { get; set; } = Atom.DoubleFromUInt12(15, true, AtomIntDouble.NZP);
    public AnalogMapping RightStickY { get; set; } = Atom.DoubleFromUInt12(14, false, AtomIntDouble.NZP); // this is likely wrong
    // to make a long fucking story short: only allah knows how the FUCK the Y axes are encoded

    public Mapping LB { get; set; } = Atom.Bit(7, 1);
    public Mapping RB { get; set; } = Atom.Bit(5, 1);

    public AnalogMapping LT { get; set; } = Atom.ZeroFalse;
    public AnalogMapping RT { get; set; } = Atom.ZeroFalse;

    public Mapping LTB_ { get; set; } = Atom.Bit(7, 0);
    public Mapping RTB_ { get; set; } = Atom.Bit(5, 0);

    public Mapping LTB { get; set; } = Atom.Bit(7, 0); // end-of-travel digibutton
    public Mapping RTB { get; set; } = Atom.Bit(5, 0); // ditto

    public Mapping L3 { get; set; } = Atom.Bit(6, 4);
    public Mapping R3 { get; set; } = Atom.Bit(6, 5);

    public Mapping Select { get; set; } = Atom.Bit(6, 7);
    public Mapping Start { get; set; } = Atom.Bit(6, 6);
    public Mapping Share { get; set; } = Atom.Bit(6, 2);
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


    public AnalogMapping PointerX { get; set; } = Atom.ZeroFalse;
    public AnalogMapping PointerY { get; set; } = Atom.ZeroFalse;
    public AnalogMapping PointerRotation { get; set; } = Atom.ZeroFalse;
    public Mapping WiiOne { get; set; } = Atom.ZeroFalse;
    public Mapping WiiTwo { get; set; } = Atom.ZeroFalse;

    public Mapping SideLeft { get; set; } = Atom.ZeroFalse;
    public Mapping SideRight { get; set; } = Atom.ZeroFalse;

    public Mapping GL { get; set; } = Atom.Bit(8, 6);
    public Mapping GR { get; set; } = Atom.Bit(8, 7);
    public AnalogMapping MouseVelX { get; set; } = Atom.ZeroFalse;
    public AnalogMapping MouseVelY { get; set; } = Atom.ZeroFalse;
    public Mapping GameChat { get; set; } = Atom.Bit(6, 1);

    public Mapping VolumeDown { get; set; } = Atom.ZeroFalse;
    public Mapping VolumeUp { get; set; } = Atom.ZeroFalse;
    public Mapping BrightnessCycle { get; set; } = Atom.ZeroFalse;
}

// TODO
public sealed class ProController2Outputs : IOutputs
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
