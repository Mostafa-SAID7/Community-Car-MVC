namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.Users.General;

/// <summary>
/// User statistics for reports
/// </summary>
public class UserStatsVM
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int NewUsersToday { get; set; }
    public int NewUsersThisWeek { get; set; }
    public int NewUsersThisMonth { get; set; }
    public int OnlineUsers { get; set; }
    public int VerifiedUsers { get; set; }
    public int UnverifiedUsers { get; set; }
    public int LockedUsers { get; set; }
    public decimal GrowthRate { get; set; }
    public decimal RetentionRate { get; set; }
}




