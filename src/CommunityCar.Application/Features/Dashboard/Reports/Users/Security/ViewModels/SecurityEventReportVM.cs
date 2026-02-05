namespace CommunityCar.Application.Features.Dashboard.Reports.Users.Security.ViewModels;

/// <summary>
/// ViewModel for security event in reports
/// </summary>
public class SecurityEventReportVM
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string EventCategory { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty; // Low, Medium, High, Critical
    public Guid? UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Status { get; set; } = string.Empty; // New, Investigating, Resolved
    public string Resolution { get; set; } = string.Empty;
    public string ResolvedBy { get; set; } = string.Empty;
    public DateTime? ResolvedAt { get; set; }
    public Dictionary<string, object> EventData { get; set; } = new();
}