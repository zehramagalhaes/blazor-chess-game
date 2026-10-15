using BlazorChessWebApp.Core.Models.Enums;

namespace BlazorChessWebApp.Core.Models;

public class MoveLogEntry
{
    public LogType Type { get; set; } = LogType.Info;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
}