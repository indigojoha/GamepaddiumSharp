namespace GamepaddiumSharp;

public static class GamepadLog
{
    public static Action<string>? Sink { get; set; }
    internal static void Debug(string message) => Sink?.Invoke(message);
}
