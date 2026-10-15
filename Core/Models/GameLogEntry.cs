namespace Chess.Core.Models;

public enum LogType
{
    Move,
    Kill,
    Check,
    Checkmate,
    GameOver,
    Info
}

public class GameLogEntry
{
    public LogType Type { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
