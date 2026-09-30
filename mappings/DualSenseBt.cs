using System.Buffers.Binary;
using HidSharp;
using GamepaddiumSharp.core;

namespace GamepaddiumSharp.mappings;


// DualSenseUsb.cs has more comments
public sealed class DualSenseBtMappings : IMappings
{
    public Action<HidDevice> OnDeviceAttached { get; set; } = device =>
    {
        if (device.TryOpen(out HidStream stream))
        {
            byte[] featureReport = new byte[41];
            featureReport[0] = 0x05;

            stream.GetFeature(featureReport);

            stream.Close();
        }
    };

    public Mapping A { get; set; } = Atom.Bit(9, 5);
    public Mapping B { get; set; } = Atom.Bit(9, 6);
    public Mapping X { get; set; } = Atom.Bit(9, 4);
    public Mapping Y { get; set; } = Atom.Bit(9, 7);

    private static readonly Atom HatAtom = Atom.Bits(9, 0, 4);

    public Mapping DpadUp { get; set; } = new InSetMapping(HatAtom, 0, 1, 7);
    public Mapping DpadRight { get; set; } = new InSetMapping(HatAtom, 1, 2, 3);
    public Mapping DpadDown { get; set; } = new InSetMapping(HatAtom, 3, 4, 5);
    public Mapping DpadLeft { get; set; } = new InSetMapping(HatAtom, 5, 6, 7);

    public AnalogMapping LeftStickX { get; set; } = Atom.Byte(2, AtomIntDouble.NZP);
    public AnalogMapping LeftStickY { get; set; } = Atom.Byte(3, AtomIntDouble.NZP);
    public AnalogMapping RightStickX { get; set; } = Atom.Byte(4, AtomIntDouble.NZP);
    public AnalogMapping RightStickY { get; set; } = Atom.Byte(5, AtomIntDouble.NZP);

    public Mapping LB { get; set; } = Atom.Bit(10, 0);
    public Mapping RB { get; set; } = Atom.Bit(10, 1);

    public AnalogMapping LT { get; set; } = Atom.Byte(6, AtomIntDouble.ZP);
    public AnalogMapping RT { get; set; } = Atom.Byte(7, AtomIntDouble.ZP);

    public Mapping LTB_ { get; set; } = Atom.Bit(10, 2);
    public Mapping RTB_ { get; set; } = Atom.Bit(10, 3);

    public Mapping LTB { get; set; } = Atom.ZeroFalse;
    public Mapping RTB { get; set; } = Atom.ZeroFalse;

    public Mapping L3 { get; set; } = Atom.Bit(10, 6);
    public Mapping R3 { get; set; } = Atom.Bit(10, 7);

    public Mapping Select { get; set; } = Atom.Bit(11, 1);
    public Mapping Start { get; set; } = Atom.Bit(10, 5);
    public Mapping Share { get; set; } = Atom.Bit(10, 4);
    public Mapping Super { get; set; } = Atom.Bit(11, 0);

    public Mapping MuteMic { get; set; } = Atom.Bit(11, 2);


    // misc

    public AnalogMapping BatteryLevel { get; set; } = new ScriptedAnalogMapping(r => r[54] & 0x0F);
    public AnalogMapping BatteryStatus { get; set; } = new ScriptedAnalogMapping(r => (r[54] >> 4) & 0x0F);
    public AnalogMapping Temperature { get; set; } = Atom.ZeroFalse;


    // motion

    public AnalogMapping GyroX { get; set; } = Atom.Int16(17);
    public AnalogMapping GyroY { get; set; } = Atom.Int16(19);
    public AnalogMapping GyroZ { get; set; } = Atom.Int16(21);
    public AnalogMapping AccelX { get; set; } = Atom.Int16(23);
    public AnalogMapping AccelY { get; set; } = Atom.Int16(25);
    public AnalogMapping AccelZ { get; set; } = Atom.Int16(27);

    public AnalogMapping TouchpadFinger1X { get; set; } = Atom.UInt12(35, false);
    public AnalogMapping TouchpadFinger1Y { get; set; } = Atom.UInt12(36, true);
    public AnalogMapping TouchpadFinger2X { get; set; } = Atom.UInt12(39, false);
    public AnalogMapping TouchpadFinger2Y { get; set; } = Atom.UInt12(40, true);


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

public sealed class DualSenseBtOutputs : IOutputs
{
    public Action<HidDevice> OnDeviceAttached { get; set; } = _ => { };

    public byte ReportId { get; set; } = 0x31;
    public int ReportLength { get; set; } = 78;

    public OutputField MotorCenterWeak { get; set; } = new();
    public OutputField MotorCenterStrong { get; set; } = new();
    public OutputField MotorRight { get; set; } = new(Atom.ByteWrite(5), Atom.Bit(3, 0));
    public OutputField MotorLeft { get; set; } = new(Atom.ByteWrite(6), Atom.Bit(3, 1));

    public OutputField MicLed { get; set; } = new(Atom.ByteWrite(11), Atom.Bit(4, 0));

    public OutputField LightbarRed { get; set; } = new(Atom.ByteWrite(47), Atom.Bit(4, 2));
    public OutputField LightbarGreen { get; set; } = new(Atom.ByteWrite(48), Atom.Bit(4, 2));
    public OutputField LightbarBlue { get; set; } = new(Atom.ByteWrite(49), Atom.Bit(4, 2));
    public OutputField PlayerLeds { get; set; } = new(Atom.Bits(46, 0, 5), Atom.Bit(4, 4));

    private const int RightTriggerOffset = 13;
    private const int LeftTriggerOffset = 24;

    public void WriteLeftTrigger(Span<byte> report, AdaptiveTriggerEffect effect)
    {
        report[1] |= 0x10;
        report[3] |= 0x08;
        effect.Encode(report.Slice(LeftTriggerOffset, 11));
    }

    public void WriteRightTrigger(Span<byte> report, AdaptiveTriggerEffect effect)
    {
        report[1] |= 0x10;
        report[3] |= 0x04;
        effect.Encode(report.Slice(RightTriggerOffset, 11));
    }

    public void WriteCrc(Span<byte> report)
    {
        uint crc = Crc32Update(0xFFFFFFFF, [0xA2]);
        crc = ~Crc32Update(crc, report[..74]);
        BinaryPrimitives.WriteUInt32LittleEndian(report.Slice(74, 4), crc);
    }

    private static uint Crc32Update(uint crc, ReadOnlySpan<byte> data)
    {
        foreach (byte b in data)
        {
            crc ^= b;
            for (int i = 0; i < 8; i++)
                crc = (crc >> 1) ^ (0xEDB88320 & (uint)-(crc & 1));
        }
        return crc;
    }
}
