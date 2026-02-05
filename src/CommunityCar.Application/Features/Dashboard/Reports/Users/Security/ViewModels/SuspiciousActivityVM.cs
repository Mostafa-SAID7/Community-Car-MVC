namespace CommunityCar.Application.Features.Dashboard.Reports.Users.Security.ViewModels;

/// <summary>
/// ViewModel for suspicious activity
/// </summary>
public class SuspiciousActivityVM
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string ActivityType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; }
    public string RiskLevel { get; set; } = string.Empty; // Low, Medium, High, Critical
    public string Status { get; set; } = string.Empty; // New, Investigating, Resolved, False Positive
    public string Resolution { get; set; } = string.Empty;
    public string ResolvedBy { get; set; } = string.Empty;
    public DateTime? ResolvedAt { get; set; }
    public Dictionary<string, object> ActivityData { get; set; } = new();
}