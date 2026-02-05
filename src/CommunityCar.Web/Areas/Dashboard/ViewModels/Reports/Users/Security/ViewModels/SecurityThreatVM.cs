namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.Users.Security.ViewModels;

/// <summary>
/// ViewModel for security threats in security reports
/// </summary>
public class SecurityThreatVM
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty; // Low, Medium, High, Critical
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; }
    public bool IsResolved { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ResolvedBy { get; set; } = string.Empty;
    public DateTime? ResolvedAt { get; set; }
    public Dictionary<string, object> ThreatData { get; set; } = new();
}




