namespace CommunityCar.Application.Features.Dashboard.Security.ViewModels;

public class SecurityAuditVM
{
    public DateTime LastAuditDate { get; set; }
    public DateTime AuditDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int OverallScore { get; set; }
    public int TotalChecks { get; set; }
    public int PassedChecks { get; set; }
    public int FailedChecks { get; set; }
    public int WarningChecks { get; set; }
    public int VulnerabilitiesFound { get; set; }
    public int CriticalIssues { get; set; }
    public DateTime NextAuditDue { get; set; }
    public List<SecurityCheckVM> SecurityChecks { get; set; } = new();
    public List<VulnerabilityVM> Vulnerabilities { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}