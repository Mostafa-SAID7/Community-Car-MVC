namespace CommunityCar.Application.Features.Dashboard.Reports.Users.Security.ViewModels;

/// <summary>
/// ViewModel for security audit
/// </summary>
public class SecurityAuditVM
{
    public DateTime AuditDate { get; set; }
    public string AuditType { get; set; } = string.Empty;
    public string OverallScore { get; set; } = string.Empty;
    public int TotalChecks { get; set; }
    public int PassedChecks { get; set; }
    public int FailedChecks { get; set; }
    public int WarningChecks { get; set; }
    public List<SecurityAuditItemVM> AuditItems { get; set; } = new();
    public List<SecurityRecommendationVM> Recommendations { get; set; } = new();
    public DateTime? LastAudit { get; set; }
    public DateTime? NextScheduledAudit { get; set; }
}

/// <summary>
/// ViewModel for security audit item
/// </summary>
public class SecurityAuditItemVM
{
    public string CheckName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Pass, Fail, Warning
    public string Description { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for security recommendation
/// </summary>
public class SecurityRecommendationVM
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty; // High, Medium, Low
    public string Category { get; set; } = string.Empty;
    public string ActionRequired { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
}