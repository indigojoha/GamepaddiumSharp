using HidSharp;
using GamepaddiumSharp.core;

namespace GamepaddiumSharp.mappings;

public sealed class GameCubeAdaptedMappings : IMappings
{
    public Action<HidDevice> OnDeviceAttached { get; set; } = _ => { };

    public Mapping A { get; set; } = Atom.Bit(1, 7 - 6);
    public Mapping B { get; set; } = Atom.Bit(1, 7 - 5);
    public Mapping X { get; set; } = Atom.Bit(1, 7 - 7);
    public Mapping Y { get; set; } = Atom.Bit(1, 7 - 4);

    public Mapping DpadUp { get; set; } = Atom.Bit(2, 7 - 3);
    public Mapping DpadRight { get; set; } = Atom.Bit(2, 7 - 2);
    public Mapping DpadDown { get; set; } = Atom.Bit(2, 7 - 1);
    public Mapping DpadLeft { get; set; } = Atom.Bit(2, 7 - 0);

    public AnalogMapping LeftStickX { get; set; } = Atom.Byte(3, AtomIntDouble.NZP);
    public AnalogMapping LeftStickY { get; set; } = Atom.Byte(4, AtomIntDouble.NZP);
    public AnalogMapping RightStickX { get; set; } = Atom.Byte(6, AtomIntDouble.NZP);
    public AnalogMapping RightStickY { get; set; } = Atom.Byte(5, AtomIntDouble.NZP);

    public Mapping LB { get; set; } = Atom.ZeroFalse;
    public Mapping RB { get; set; } = Atom.Bit(1, 7 - 0);

    public AnalogMapping LT { get; set; } = Atom.Byte(7, AtomIntDouble.ZP);
    public AnalogMapping RT { get; set; } = Atom.Byte(8, AtomIntDouble.ZP);

    public Mapping LTB_ { get; set; } = new ScriptedMapping((r) => { return r[7] > 0b0000_0100; });
    public Mapping RTB_ { get; set; } = new ScriptedMapping((r) => { return r[8] > 0b0000_0100; });

    public Mapping LTB { get; set; } = Atom.Bit(1, 7 - 3); // end-of-travel digibutton
    public Mapping RTB { get; set; } = Atom.Bit(1, 7 - 2); // ditto

    public Mapping L3 { get; set; } = Atom.ZeroFalse;
    public Mapping R3 { get; set; } = Atom.ZeroFalse;

    public Mapping Select { get; set; } = Atom.ZeroFalse;
    public Mapping Start { get; set; } = Atom.Bit(2, 7 - 6);
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

    public Mapping VolumeDown { get; set; } = Atom.ZeroFalse;
    public Mapping VolumeUp { get; set; } = Atom.ZeroFalse;
    public Mapping BrightnessCycle { get; set; } = Atom.ZeroFalse;
}

public sealed class GameCubeAdaptedOutputs : IOutputs
{
    public Action<HidDevice> OnDeviceAttached { get; set; } = _ => { };

    public byte ReportId { get; set; } = 0x00;
    public int ReportLength { get; set; } = 3;


    public OutputField MotorCenterWeak { get; set; } = new();
    public OutputField MotorCenterStrong { get; set; } = new(Atom.ByteWrite(1));
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
