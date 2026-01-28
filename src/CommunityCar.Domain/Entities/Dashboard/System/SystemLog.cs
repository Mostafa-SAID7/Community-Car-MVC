using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Dashboard.System;

public enum LogLevel
{
    Info = 0,
    Warning = 1,
    Error = 2,
    Critical = 3
}

public class SystemLog : BaseEntity
{
    public DateTime Timestamp { get; set; }
    public LogLevel Level { get; set; }
    public string Source { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Exception { get; set; }
    public string? UserId { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? RequestPath { get; set; }
    public int? StatusCode { get; set; }
    public TimeSpan? Duration { get; set; }

    public SystemLog()
    {
        Timestamp = DateTime.UtcNow;
    }

    public static SystemLog CreateInfo(string source, string message, string? userId = null)
    {
        return new SystemLog
        {
            Level = LogLevel.Info,
            Source = source,
            Message = message,
            UserId = userId
        };
    }

    public static SystemLog CreateWarning(string source, string message, string? userId = null)
    {
        return new SystemLog
        {
            Level = LogLevel.Warning,
            Source = source,
            Message = message,
            UserId = userId
        };
    }

    public static SystemLog CreateError(string source, string message, string? exception = null, string? userId = null)
    {
        return new SystemLog
        {
            Level = LogLevel.Error,
            Source = source,
            Message = message,
            Exception = exception,
            UserId = userId
        };
    }
}