using CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.Users.Security.ViewModels;

namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.Users.Security;

/// <summary>
/// ViewModel for user security report
/// </summary>
public class UserSecurityReportVM
{
    // Missing properties that services expect
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int BlockedAttempts { get; set; }
    public Guid UserId { get; set; }
    
    public int TotalSecurityEvents { get; set; }
    public int FailedLoginAttempts { get; set; }
    public int SuccessfulLogins { get; set; }
    public int BlockedIpAddresses { get; set; }
    public int SuspiciousActivities { get; set; }
    public int SecurityThreats { get; set; }
    public int TwoFactorEnabledUsers { get; set; }
    public int PasswordResets { get; set; }
    public int AccountLockouts { get; set; }
    public decimal SecurityScore { get; set; }
    public List<SecurityThreatReportVM> RecentThreats { get; set; } = new();
    public List<SecurityEventReportVM> SecurityEvents { get; set; } = new();
    public List<BlockedIpReportVM> BlockedIps { get; set; } = new();
}

/// <summary>
/// ViewModel for blocked IP in reports
/// </summary>
public class BlockedIpReportVM
{
    public string IpAddress { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime BlockedAt { get; set; }
    public string BlockedBy { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public int AttemptCount { get; set; }
    public string Location { get; set; } = string.Empty;
    public string ThreatLevel { get; set; } = string.Empty;
}




