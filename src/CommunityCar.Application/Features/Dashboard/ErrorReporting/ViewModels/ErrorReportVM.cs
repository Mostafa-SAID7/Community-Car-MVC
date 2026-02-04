namespace CommunityCar.Application.Features.Dashboard.ErrorReporting.ViewModels;

public class ErrorReportVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ErrorType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ReportedBy { get; set; } = string.Empty;
    public string AssignedTo { get; set; } = string.Empty;
    public DateTime ReportedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string StackTrace { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string TicketId { get; set; } = string.Empty;
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
    public List<string> Recipients { get; set; } = new();
    public int ThresholdCount { get; set; }
    public int TimeWindowMinutes { get; set; }
}