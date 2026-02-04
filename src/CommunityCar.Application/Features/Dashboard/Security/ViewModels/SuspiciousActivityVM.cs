namespace CommunityCar.Application.Features.Dashboard.Security.ViewModels;

public class SuspiciousActivityVM
{
    public Guid Id { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string Pattern { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsInvestigated { get; set; }
    public string? InvestigationNotes { get; set; }
    public bool IsBlocked { get; set; }
    public Dictionary<string, object> Evidence { get; set; } = new();
}