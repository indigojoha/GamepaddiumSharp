namespace GamepaddiumSharp;

public enum PadDigital
{
    Invalid,

    A, // cross

    B, // circle

    X, // square

    Y, // triangle

    DPadUp,
    DPadDown,
    DPadLeft,
    DPadRight,

    LeftButton,
    LeftTriggerHeld,
    LeftTriggerEndButton,
    LeftStickButton,
    RightButton,
    RightTriggerHeld,
    RightTriggerEndButton,
    RightStickButton,

    Select,
    Start,
    Share,
    Super,

    // PS
    MuteMic,

    // PSP
    VolumeDown,
    VolumeUp,
    BrightnessCycle,

    // Wii
    WiiOne,
    WiiTwo,

    // Switch
    SideLeft,
    SideRight,

    // Switch 2
    GripLeft,
    GripRight,
    GameChat,
}

public enum PadAnalog
{
    Invalid,

    LeftStickX,
    LeftStickY,
    RightStickX,
    RightStickY,

    LeftTrigger,
    RightTrigger,

    BatteryLevel,
    BatteryStatus,
    Temperature,

    GyroX,
    GyroY,
    GyroZ,
    AccelerometerX,
    AccelerometerY,
    AccelerometerZ,

    // PS
    TouchpadFinger1X,
    TouchpadFinger1Y,
    TouchpadFinger2X,
    TouchpadFinger2Y,

    // Wii
    PointerX,
    PointerY,
    PointerRotation,

    // Switch 2
    MouseVelocityX,
    MouseVelocityY,
}

public static class GamepaddiumEnumTools
{
    public static readonly PadDigital[] DigitalList = (PadDigital[])Enum.GetValues(typeof(PadDigital));
    public static readonly PadAnalog[] AnalogList = (PadAnalog[])Enum.GetValues(typeof(PadAnalog));
    public static readonly PadAnalog[] ImportantAnalogList = [PadAnalog.LeftStickX, PadAnalog.LeftStickY, PadAnalog.RightStickX, PadAnalog.RightStickY, PadAnalog.LeftTrigger, PadAnalog.RightTrigger, PadAnalog.MouseVelocityX, PadAnalog.MouseVelocityY];
}