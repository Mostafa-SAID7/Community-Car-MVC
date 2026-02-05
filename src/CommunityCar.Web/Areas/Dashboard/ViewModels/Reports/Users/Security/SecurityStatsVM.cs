using CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.Users.Security.ViewModels;

namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.Users.Security;

/// <summary>
/// Security statistics view model
/// </summary>
public class SecurityStatsVM
{
    // Missing properties that services expect
    public int TotalSecurityEvents { get; set; }
    public int BlockedAttacks { get; set; }
    public int FailedLoginAttempts { get; set; }
    
    public int FailedLogins { get; set; }
    public int BlockedIps { get; set; }
    public int SecurityThreats { get; set; }
    public int SuspiciousActivities { get; set; }
    public decimal SecurityScore { get; set; }
    public int SuccessfulLogins { get; set; }
    public int TwoFactorEnabled { get; set; }
    public int PasswordResets { get; set; }
    public int AccountLockouts { get; set; }
    public int BruteForceAttempts { get; set; }
    public int UnauthorizedAccess { get; set; }
    public DateTime LastSecurityIncident { get; set; }
    public List<SecurityThreatVM> RecentThreats { get; set; } = new();
    public List<FailedLoginTrendVM> FailedLoginTrend { get; set; } = new();
    public Dictionary<string, int> ThreatsByType { get; set; } = new();
    public Dictionary<string, int> LoginAttemptsByCountry { get; set; } = new();
}

public class FailedLoginTrendVM
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
    public string Period { get; set; } = string.Empty;
}




