namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Overview.Users.Statistics;

/// <summary>
/// User quick statistics view model for overview
/// </summary>
public class UserQuickStatsVM
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int NewUsersToday { get; set; }
    public int OnlineUsers { get; set; }
    public int PendingApprovals { get; set; }
    public int LockedAccounts { get; set; }
    public decimal GrowthPercentage { get; set; }
    public string TrendDirection { get; set; } = string.Empty; // Up, Down, Stable
}




