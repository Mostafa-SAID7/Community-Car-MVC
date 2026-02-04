namespace CommunityCar.Application.Features.Dashboard.Security.ViewModels;

public class SecurityAuditVM
{
    public DateTime AuditDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TotalChecks { get; set; }
    public int PassedChecks { get; set; }
    public int FailedChecks { get; set; }
    public int WarningChecks { get; set; }
    public List<SecurityCheckVM> SecurityChecks { get; set; } = new();
    public List<VulnerabilityVM> Vulnerabilities { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}