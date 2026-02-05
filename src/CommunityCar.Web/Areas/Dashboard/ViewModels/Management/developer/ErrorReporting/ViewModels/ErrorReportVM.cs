namespace CommunityCar.Application.Features.Dashboard.Management.developer.ErrorReporting.ViewModels;

public class ErrorReportVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string ErrorType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public Guid? UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string ReportedBy { get; set; } = string.Empty;
    public string AssignedTo { get; set; } = string.Empty;
    public DateTime ReportedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string RequestPath { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public string StackTrace { get; set; } = string.Empty;
    public string InnerException { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalData { get; set; } = new();
    public string UserAgent { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string TicketId { get; set; } = string.Empty;
    public bool IsResolved { get; set; }
    public string ResolvedBy { get; set; } = string.Empty;
    public string Resolution { get; set; } = string.Empty;
    public int OccurrenceCount { get; set; }
    public DateTime FirstOccurrence { get; set; }
    public DateTime LastOccurrence { get; set; }
    public int AffectedUsers { get; set; }
    public string Environment { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public List<ErrorCommentVM> Comments { get; set; } = new();
}

public class CreateErrorReportVM
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ErrorType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
}

public class ErrorCommentVM
{
    public Guid Id { get; set; }
    public string Comment { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ErrorAlertConfigVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ErrorType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public bool EnableEmailAlerts { get; set; }
    public string AlertEmail { get; set; } = string.Empty;
    public int CriticalErrorThreshold { get; set; }
    public int HighErrorThreshold { get; set; }
    public int MediumErrorThreshold { get; set; }
    public int AlertInterval { get; set; }
    public bool EnableSlackAlerts { get; set; }
    public string SlackWebhookUrl { get; set; } = string.Empty;
    public bool EnableSmsAlerts { get; set; }
    public string SmsPhoneNumber { get; set; } = string.Empty;
    public bool AlertOnNewErrorTypes { get; set; }
    public bool AlertOnErrorSpikes { get; set; }
    public int ErrorSpikeThreshold { get; set; }
    public List<string> Recipients { get; set; } = new();
    public int ThresholdCount { get; set; }
    public int TimeWindowMinutes { get; set; }
}