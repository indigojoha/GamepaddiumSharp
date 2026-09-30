using HidSharp;
using GamepaddiumSharp.core;

namespace GamepaddiumSharp.mappings;

public sealed class DualSenseUsbMappings : IMappings
{
    public Action<HidDevice> OnDeviceAttached { get; set; } = _ => { };

    public Mapping A { get; set; } = Atom.Bit(8, 5);
    public Mapping B { get; set; } = Atom.Bit(8, 6);
    public Mapping X { get; set; } = Atom.Bit(8, 4);
    public Mapping Y { get; set; } = Atom.Bit(8, 7);

    private static readonly Atom HatAtom = Atom.Bits(8, 0, 4);

    public Mapping DpadUp { get; set; } = new InSetMapping(HatAtom, 0, 1, 7);
    public Mapping DpadRight { get; set; } = new InSetMapping(HatAtom, 1, 2, 3);
    public Mapping DpadDown { get; set; } = new InSetMapping(HatAtom, 3, 4, 5);
    public Mapping DpadLeft { get; set; } = new InSetMapping(HatAtom, 5, 6, 7);

    public AnalogMapping LeftStickX { get; set; } = Atom.Byte(1, AtomIntDouble.NZP);
    public AnalogMapping LeftStickY { get; set; } = Atom.Byte(2, AtomIntDouble.NZP);
    public AnalogMapping RightStickX { get; set; } = Atom.Byte(3, AtomIntDouble.NZP);
    public AnalogMapping RightStickY { get; set; } = Atom.Byte(4, AtomIntDouble.NZP);

    public Mapping LB { get; set; } = Atom.Bit(9, 0);
    public Mapping RB { get; set; } = Atom.Bit(9, 1);

    public AnalogMapping LT { get; set; } = Atom.Byte(5, AtomIntDouble.ZP);
    public AnalogMapping RT { get; set; } = Atom.Byte(6, AtomIntDouble.ZP);

    public Mapping LTB_ { get; set; } = Atom.Bit(9, 2);
    public Mapping RTB_ { get; set; } = Atom.Bit(9, 3);

    public Mapping LTB { get; set; } = Atom.ZeroFalse; // no end-of-travel digibutton
    public Mapping RTB { get; set; } = Atom.ZeroFalse; // ditto

    public Mapping L3 { get; set; } = Atom.Bit(9, 6);
    public Mapping R3 { get; set; } = Atom.Bit(9, 7);

    public Mapping Select { get; set; } = Atom.Bit(10, 1); // trackpad click
    public Mapping Start { get; set; } = Atom.Bit(9, 5); // options
    public Mapping Share { get; set; } = Atom.Bit(9, 4); // create
    public Mapping Super { get; set; } = Atom.Bit(10, 0); // PS button

    public Mapping MuteMic { get; set; } = Atom.Bit(10, 2);


    // misc

    public AnalogMapping BatteryLevel { get; set; } = new ScriptedAnalogMapping(r => r[53] & 0x0F);
    public AnalogMapping BatteryStatus { get; set; } = new ScriptedAnalogMapping(r => (r[53] >> 4) & 0x0F);
    public AnalogMapping Temperature { get; set; } = Atom.ZeroFalse; // TODO (can it even be done at all, i wonder?)


    // motion

    public AnalogMapping GyroX { get; set; } = Atom.Int16(16);
    public AnalogMapping GyroY { get; set; } = Atom.Int16(18);
    public AnalogMapping GyroZ { get; set; } = Atom.Int16(20);
    public AnalogMapping AccelX { get; set; } = Atom.Int16(22);
    public AnalogMapping AccelY { get; set; } = Atom.Int16(24);
    public AnalogMapping AccelZ { get; set; } = Atom.Int16(26);

    public AnalogMapping TouchpadFinger1X { get; set; } = Atom.UInt12(34, false);
    public AnalogMapping TouchpadFinger1Y { get; set; } = Atom.UInt12(35, true);
    public AnalogMapping TouchpadFinger2X { get; set; } = Atom.UInt12(38, false);
    public AnalogMapping TouchpadFinger2Y { get; set; } = Atom.UInt12(39, true);


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

public sealed class DualSenseUsbOutputs : IOutputs
{
    public Action<HidDevice> OnDeviceAttached { get; set; } = _ => { };

    public byte ReportId { get; set; } = 0x02;
    public int ReportLength { get; set; } = 48;

    public OutputField MotorCenterWeak { get; set; } = new();
    public OutputField MotorCenterStrong { get; set; } = new();
    public OutputField MotorRight { get; set; } = new(Atom.ByteWrite(3), Atom.Bit(1, 0));
    public OutputField MotorLeft { get; set; } = new(Atom.ByteWrite(4), Atom.Bit(1, 1));

    public OutputField MicLed { get; set; } = new(Atom.ByteWrite(9), Atom.Bit(2, 0));

    public OutputField LightbarRed { get; set; } = new(Atom.ByteWrite(45), Atom.Bit(2, 2));
    public OutputField LightbarGreen { get; set; } = new(Atom.ByteWrite(46), Atom.Bit(2, 2));
    public OutputField LightbarBlue { get; set; } = new(Atom.ByteWrite(47), Atom.Bit(2, 2));
    public OutputField PlayerLeds { get; set; } = new(Atom.Bits(44, 0, 5), Atom.Bit(2, 4));

    private const int RightTriggerOffset = 11;
    private const int LeftTriggerOffset = 22;

    public void WriteLeftTrigger(Span<byte> report, AdaptiveTriggerEffect effect)
    {
        report[1] |= 0x08;
        effect.Encode(report.Slice(LeftTriggerOffset, 11));
    }

    public void WriteRightTrigger(Span<byte> report, AdaptiveTriggerEffect effect)
    {
        report[1] |= 0x04;
        effect.Encode(report.Slice(RightTriggerOffset, 11));
    }

    public void WriteCrc(Span<byte> report) { }
}
