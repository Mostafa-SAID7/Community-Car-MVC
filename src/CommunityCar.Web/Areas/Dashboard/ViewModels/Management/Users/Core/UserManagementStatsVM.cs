namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.Users.Core;

/// <summary>
/// User management statistics view model
/// </summary>
public class UserManagementStatsVM
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int NewUsersToday { get; set; }
    public int NewUsersThisWeek { get; set; }
    public int NewUsersThisMonth { get; set; }
    public int InactiveUsers { get; set; }
    public int LockedUsers { get; set; }
    public int PendingUsers { get; set; }
    public decimal UserGrowthRate { get; set; }
    public decimal UserRetentionRate { get; set; }
    public DateTime LastUpdated { get; set; }
}




