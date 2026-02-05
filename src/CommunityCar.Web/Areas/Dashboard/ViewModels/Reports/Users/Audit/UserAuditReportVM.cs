namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.Users.Audit;

/// <summary>
/// User audit report view model
/// </summary>
public class UserAuditReportVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime ReportPeriodStart { get; set; }
    public DateTime ReportPeriodEnd { get; set; }
    public int TotalActions { get; set; }
    public int SuccessfulActions { get; set; }
    public int FailedActions { get; set; }
    public int SecurityEvents { get; set; }
    public int LoginAttempts { get; set; }
    public int FailedLogins { get; set; }
    public int PasswordChanges { get; set; }
    public int ProfileUpdates { get; set; }
    public int PermissionChanges { get; set; }
    public List<UserAuditEntryVM> AuditEntries { get; set; } = new();
    public Dictionary<string, int> ActionsByType { get; set; } = new();
}




