namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.Users.Security.ViewModels;

/// <summary>
/// ViewModel for security threat in reports
/// </summary>
public class SecurityThreatReportVM
{
    public Guid Id { get; set; }
    public string ThreatType { get; set; } = string.Empty;
    public string ThreatLevel { get; set; } = string.Empty; // Low, Medium, High, Critical
    public string Description { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string Status { get; set; } = string.Empty; // Active, Resolved, Investigating
    public string Resolution { get; set; } = string.Empty;
    public string ResolvedBy { get; set; } = string.Empty;
    public int AttemptCount { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}




