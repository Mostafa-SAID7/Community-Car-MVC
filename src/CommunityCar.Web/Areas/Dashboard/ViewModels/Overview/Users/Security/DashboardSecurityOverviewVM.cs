using CommunityCar.Application.Features.Shared.ViewModels;
using CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.Users.Security.ViewModels;

namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Overview.Users.Security;

/// <summary>
/// Security overview for users in dashboard
/// </summary>
public class DashboardSecurityOverviewVM
{
    public string OverallStatus { get; set; } = string.Empty; // Secure, Warning, Critical
    public int TotalThreats { get; set; }
    public int ActiveThreats { get; set; }
    public int ResolvedThreats { get; set; }
    public int FailedLogins { get; set; }
    public int BlockedAttacks { get; set; }
    public int BlockedIPs { get; set; }
    public int SuspiciousActivities { get; set; }
    public int SecurityAlerts { get; set; }
    public int SecurityScore { get; set; }
    public int TwoFactorEnabled { get; set; }
    public int PasswordStrengthAverage { get; set; }
    public DateTime LastSecurityScan { get; set; }
    public DateTime LastThreatDetected { get; set; }
    public List<SecurityThreatVM> RecentThreats { get; set; } = new();
    public List<ChartDataVM> ThreatTrend { get; set; } = new();
    public Dictionary<string, int> ThreatsByType { get; set; } = new();
}




