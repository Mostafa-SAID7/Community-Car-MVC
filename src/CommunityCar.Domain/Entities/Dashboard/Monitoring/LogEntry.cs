using System;
using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Dashboard.Monitoring;

public class LogEntry : BaseEntity
{
    public string Level { get; private set; } // Info, Warning, Error
    public string Message { get; private set; }
    public string? StackTrace { get; private set; }
    public string Source { get; private set; }

    public LogEntry(string level, string message, string source, string? stackTrace = null)
    {
        Level = level;
        Message = message;
        Source = source;
        StackTrace = stackTrace;
    }
}
