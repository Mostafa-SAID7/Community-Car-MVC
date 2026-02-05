namespace CommunityCar.Application.Features.Dashboard.Overview.Users.Security;

public class UserSecurityOverviewVM
{
    public int TotalUsers { get; set; }
    public int SecureUsers { get; set; }
    public int VulnerableUsers { get; set; }
    public int RecentThreats { get; set; }
    public double SecurityScore { get; set; }
    public int ActiveSessions { get; set; }
    public int SuspiciousActivities { get; set; }
    public DateTime LastSecurityScan { get; set; }
}