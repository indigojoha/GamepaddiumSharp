
using HidSharp;

namespace GamepaddiumSharp.core;

public enum Endianness
{
    Little,
    Big
}


public enum AtomIntDouble
{
    Raw,
    ZP,
    NZ,
    NZP,
    PZN,
}

public readonly record struct Atom
{
    public required int ByteOffset { get; init; }
    public int BitOffset { get; init; } = 0;
    public required int BitWidth { get; init; }
    public bool Signed { get; init; } = false;
    public Endianness Endianness { get; init; } = Endianness.Little;
    public AtomIntDouble AnalogType { get; init; } = AtomIntDouble.Raw;

    public Atom() { }

    public static Atom Bit(int byteOffset, int bitOffset) => new() { ByteOffset = byteOffset, BitOffset = bitOffset, BitWidth = 1 };

    public static Atom Byte(int byteOffset, AtomIntDouble analogType) => new() { ByteOffset = byteOffset, BitWidth = 8, AnalogType = analogType };
    public static Atom ByteWrite(int byteOffset) => new() { ByteOffset = byteOffset, BitWidth = 8 };

    public static Atom Nibble(int byteOffset, int bitOffset) => new() { ByteOffset = byteOffset, BitOffset = bitOffset, BitWidth = 4 };

    public static Atom UInt12(int byteOffset, bool startAtHalfByte, Endianness endianness = Endianness.Little) => new() { ByteOffset = byteOffset, BitOffset = startAtHalfByte ? 4 : 0, BitWidth = 12, Endianness = endianness };
    public static Atom Int12(int byteOffset, bool startAtHalfByte, Endianness endianness = Endianness.Little) => new() { ByteOffset = byteOffset, BitOffset = startAtHalfByte ? 4 : 0, BitWidth = 12, Endianness = endianness, Signed = true };
    public static Atom DoubleFromUInt12(int byteOffset, bool startAtHalfByte, AtomIntDouble analogType, Endianness intEndianness = Endianness.Little) => new() { ByteOffset = byteOffset, BitOffset = startAtHalfByte ? 4 : 0, BitWidth = 12, Endianness = intEndianness, AnalogType = analogType };
    public static Atom UInt16(int byteOffset, Endianness endianness = Endianness.Little) => new() { ByteOffset = byteOffset, BitWidth = 16, Endianness = endianness };
    public static Atom Int16(int byteOffset, Endianness endianness = Endianness.Little) => new() { ByteOffset = byteOffset, BitWidth = 16, Endianness = endianness, Signed = true };

    public static Atom Bits(int byteOffset, int bitOffset, int width) => new() { ByteOffset = byteOffset, BitOffset = bitOffset, BitWidth = width };

    public static Atom ZeroFalse => new() { ByteOffset = 0, BitWidth = 0 };

    private long ReadRawBits(ReadOnlySpan<byte> report)
    {
        long value = 0;
        int bitsRead = 0;
        int byteIndex = ByteOffset;
        int bitOffset = BitOffset;

        while (bitsRead < BitWidth)
        {
            byte b = report[byteIndex];
            int bitsAvailableInByte = 8 - bitOffset;
            int bitsToRead = Math.Min(bitsAvailableInByte, BitWidth - bitsRead);

            int mask = (1 << bitsToRead) - 1;
            int chunk = (b >> bitOffset) & mask;

            if (Endianness == Endianness.Big)
                value = (value << bitsToRead) | (uint)chunk;
            else
                value |= (long)chunk << bitsRead;

            bitsRead += bitsToRead;
            bitOffset = 0;
            byteIndex++;
        }

        return value;
    }

    public long Read(ReadOnlySpan<byte> report)
    {
        long raw = ReadRawBits(report);

        if (Signed && BitWidth is > 0 and < 64)
        {
            long signBit = 1L << (BitWidth - 1);
            if ((raw & signBit) != 0)
                raw -= 1L << BitWidth;
        }

        return raw;
    }

    public double ReadAnalog(ReadOnlySpan<byte> report)
    {
        if (AnalogType == AtomIntDouble.Raw)
            return Read(report);

        long raw = ReadRawBits(report);

        long maxRaw = BitWidth >= 63 ? long.MaxValue : (1L << BitWidth) - 1;
        if (maxRaw == 0) return 0.0;
        long midRaw = maxRaw / 2;

        switch (AnalogType)
        {
            case AtomIntDouble.ZP:
                return (double)raw / maxRaw;

            case AtomIntDouble.NZ:
                return 1.0 - (double)raw / maxRaw;

            case AtomIntDouble.NZP:
                return raw <= midRaw
                    ? (double)(raw - midRaw) / midRaw
                    : (double)(raw - midRaw) / (maxRaw - midRaw);

            case AtomIntDouble.PZN:
                return raw <= midRaw
                    ? -(double)(raw - midRaw) / midRaw
                    : -(double)(raw - midRaw) / (maxRaw - midRaw);

            default:
                throw new ArgumentOutOfRangeException(nameof(AnalogType));
        }
    }

    public void Write(Span<byte> report, long value)
    {
        int bitsWritten = 0;
        int byteIndex = ByteOffset;
        int bitOffset = BitOffset;

        while (bitsWritten < BitWidth)
        {
            int bitsAvailableInByte = 8 - bitOffset;
            int bitsToWrite = Math.Min(bitsAvailableInByte, BitWidth - bitsWritten);
            int mask = (1 << bitsToWrite) - 1;

            int chunk = Endianness == Endianness.Big
                ? (int)(value >> (BitWidth - bitsWritten - bitsToWrite)) & mask
                : (int)(value >> bitsWritten) & mask;

            report[byteIndex] = (byte)((report[byteIndex] & ~(mask << bitOffset)) | (chunk << bitOffset));

            bitsWritten += bitsToWrite;
            bitOffset = 0;
            byteIndex++;
        }
    }
}


public delegate bool BoolExpr(ReadOnlySpan<byte> report);
public delegate double IntExpr(ReadOnlySpan<byte> report);

public abstract record Mapping
{
    public abstract bool Read(ReadOnlySpan<byte> report);

    public static implicit operator Mapping(Atom atom) => new AtomMapping(atom);
    public static implicit operator Mapping(BoolExpr expr) => new ScriptedMapping(expr);
}

public sealed record AtomMapping(Atom Atom) : Mapping
{
    public override bool Read(ReadOnlySpan<byte> report) => Atom.Read(report) != 0;
}

public sealed record InSetMapping(Atom Atom, IReadOnlySet<long> Values) : Mapping
{
    public InSetMapping(Atom atom, params long[] values) : this(atom, values.ToHashSet()) { }

    public override bool Read(ReadOnlySpan<byte> report) => Values.Contains(Atom.Read(report));
}

public sealed record ScriptedMapping(BoolExpr Expr) : Mapping
{
    public override bool Read(ReadOnlySpan<byte> report) => Expr(report);
}

public sealed record OutputField(Atom? Value = null, Atom? Enable = null)
{
    public void Write(Span<byte> report, long value)
    {
        if (!Value.HasValue) return;

        Value.Value.Write(report, value);
        Enable?.Write(report, 1);
    }
}


public abstract record AnalogMapping
{
    public abstract double Read(ReadOnlySpan<byte> report);

    public static implicit operator AnalogMapping(Atom atom) => new AtomAnalogMapping(atom);
    public static implicit operator AnalogMapping(IntExpr expr) => new ScriptedAnalogMapping(expr);
}

public sealed record AtomAnalogMapping(Atom Atom) : AnalogMapping
{
    public override double Read(ReadOnlySpan<byte> report) => Atom.ReadAnalog(report);
}

public sealed record ScaledAnalogMapping(Atom Atom, double Factor, double Offset = 0) : AnalogMapping
{
    public override double Read(ReadOnlySpan<byte> report) => Atom.Read(report) * Factor + Offset;
}

public sealed record LookupAnalogMapping(Atom Atom, IReadOnlyDictionary<long, double> Table, double Fallback = 0)
    : AnalogMapping
{
    public override double Read(ReadOnlySpan<byte> report) =>
        Table.TryGetValue(Atom.Read(report), out var v) ? v : Fallback;
}

public sealed record ScriptedAnalogMapping(IntExpr Expr) : AnalogMapping
{
    public override double Read(ReadOnlySpan<byte> report) => Expr(report);
}

public abstract record AdaptiveTriggerEffect
{
    public abstract void Encode(Span<byte> dest);

    private void Pad(int start, ref Span<byte> dest)
    {
        while (start < dest.Length) {
            dest[start++] = 0x00;
        }
    }

    public sealed record Off : AdaptiveTriggerEffect
    {
        public override void Encode(Span<byte> dest) => dest[0] = 0x05;
    }

    public sealed record Rigid(double StartPosition, double Force) : AdaptiveTriggerEffect
    {
        public override void Encode(Span<byte> dest)
        {
            dest[0] = 0x01;
            dest[1] = (byte)(StartPosition * 255);
            dest[2] = (byte)(Force * 255);
            Pad(3, ref dest);
        }
    }

    public sealed record BowDeprecated(double StartPosition, double EndPosition, double PullForce, double SnapForce) : AdaptiveTriggerEffect
    {
        public override void Encode(Span<byte> dest)
        {
            dest[0] = 0x22;
            dest[1] = (byte)(StartPosition * 255);
            dest[2] = (byte)(EndPosition * 255);
            dest[3] = (byte)(PullForce * 255);
            dest[4] = (byte)(SnapForce * 255);
            Pad(4, ref dest);
        }
    }

    public sealed record GallopDeprecated(double StartPosition, double EndPosition, double ForceFoot1, double ForceFoot2, double Frequency) : AdaptiveTriggerEffect
    {
        public override void Encode(Span<byte> dest)
        {
            dest[0] = 0x23;
            dest[1] = (byte)(StartPosition * 255);
            dest[2] = (byte)(EndPosition * 255);
            dest[3] = (byte)(ForceFoot1 * 255);
            dest[4] = (byte)(ForceFoot2 * 255);
            dest[5] = (byte)(Frequency * 255);
            Pad(6, ref dest);
        }
    }

    public sealed record Snap(double StartPosition, double EndPosition, double Force) : AdaptiveTriggerEffect
    {
        public override void Encode(Span<byte> dest)
        {
            dest[0] = 0x25;
            dest[1] = (byte)(StartPosition * 255);
            dest[2] = (byte)(EndPosition * 255);
            dest[3] = (byte)(Force * 255);
            Pad(4, ref dest);
        }
    }

    public sealed record Vibration(double StartPosition, double Amplitude, double Frequency) : AdaptiveTriggerEffect
    {
        public override void Encode(Span<byte> dest)
        {
            dest[0] = 0x26;
            dest[1] = (byte)(StartPosition * 255);
            dest[2] = (byte)(Amplitude * 255);
            dest[3] = (byte)(Frequency * 255);
            Pad(4, ref dest);
        }
    }

    public sealed record FullAuto(double StartPosition, double EndPosition, double ForceA, double ForceB, double Frequency, double Period) : AdaptiveTriggerEffect
    {
        public override void Encode(Span<byte> dest)
        {
            dest[0] = 0x27;
            dest[1] = (byte)(StartPosition * 255);
            dest[2] = (byte)(EndPosition * 255);
            dest[3] = (byte)(ForceA * 255);
            dest[4] = (byte)(ForceB * 255);
            dest[5] = (byte)(Frequency * 255);
            dest[6] = (byte)(Period * 255);
            Pad(6, ref dest);
        }
    }
}

public interface IMappings
{
    Action<HidDevice> OnDeviceAttached { get; set; }

    Mapping A { get; set; }
    Mapping B { get; set; }
    Mapping X { get; set; }
    Mapping Y { get; set; }

    Mapping DpadUp { get; set; }
    Mapping DpadDown { get; set; }
    Mapping DpadLeft { get; set; }
    Mapping DpadRight { get; set; }

    AnalogMapping LeftStickX { get; set; }
    AnalogMapping LeftStickY { get; set; }
    AnalogMapping RightStickX { get; set; }
    AnalogMapping RightStickY { get; set; }

    Mapping LB { get; set; }
    Mapping RB { get; set; }
    AnalogMapping LT { get; set; }
    Mapping LTB_ { get; set; } // DualSense has a bit that signals when a trigger is non-zero, for some reason
    Mapping LTB { get; set; } // GCN trigger button at the end (i guess bro)
    AnalogMapping RT { get; set; }
    Mapping RTB_ { get; set; } // ditto
    Mapping RTB { get; set; } // ditto
    Mapping L3 { get; set; }
    Mapping R3 { get; set; }

    Mapping Select { get; set; } // also PS's trackpad click, i guess
    Mapping Start { get; set; }
    Mapping Share { get; set; }
    Mapping Super { get; set; } // Linux kinda situation for Windows and macOS's Win Key and Cmd Key. Super is Home/Guide/PS Button/Shitmypants button

    // Misc
    AnalogMapping BatteryLevel { get; set; }
    AnalogMapping BatteryStatus { get; set; }
    AnalogMapping Temperature { get; set; }

    // Motion
    AnalogMapping GyroX { get; set; }
    AnalogMapping GyroY { get; set; }
    AnalogMapping GyroZ { get; set; }
    AnalogMapping AccelX { get; set; }
    AnalogMapping AccelY { get; set; }
    AnalogMapping AccelZ { get; set; }

    // PS
    AnalogMapping TouchpadFinger1X { get; set; }
    AnalogMapping TouchpadFinger1Y { get; set; }
    AnalogMapping TouchpadFinger2X { get; set; }
    AnalogMapping TouchpadFinger2Y { get; set; }
    Mapping MuteMic { get; set; }

    // PSP
    Mapping VolumeDown { get; set; }
    Mapping VolumeUp { get; set; }
    Mapping BrightnessCycle { get; set; }

    // WiiMote support when
    AnalogMapping PointerX { get; set; }
    AnalogMapping PointerY { get; set; }
    AnalogMapping PointerRotation { get; set; }
    Mapping WiiOne {  get; set; }
    Mapping WiiTwo { get; set; }

    // Switch
    Mapping SideLeft { get; set; }
    Mapping SideRight { get; set; }

    // Switch 2
    Mapping GL { get; set; }
    Mapping GR { get; set; }
    AnalogMapping MouseVelX { get; set; }
    AnalogMapping MouseVelY { get; set; }
    Mapping GameChat { get; set; }
}
// notice how the whole interface has nothing for xbox ? this is a subtle reference to the fact that xinput controllers suck ass and have not evolved since 2001

public interface IOutputs
{
    Action<HidDevice> OnDeviceAttached { get; set; }

    OutputField MotorCenterWeak { get; set; }
    OutputField MotorCenterStrong { get; set; }
    OutputField MotorLeft { get; set; }
    OutputField MotorRight { get; set; }
    OutputField LightbarRed { get; set; }
    OutputField LightbarGreen { get; set; }
    OutputField LightbarBlue { get; set; }
    OutputField PlayerLeds { get; set; }      // 5-bit mask
    OutputField MicLed { get; set; }

    int ReportLength { get; set; }
    byte ReportId { get; set; }

    // adaptive triggers are NOT OutputFields — see below
    void WriteLeftTrigger(Span<byte> report, AdaptiveTriggerEffect effect);
    void WriteRightTrigger(Span<byte> report, AdaptiveTriggerEffect effect);
    void WriteCrc(Span<byte> report);
}
