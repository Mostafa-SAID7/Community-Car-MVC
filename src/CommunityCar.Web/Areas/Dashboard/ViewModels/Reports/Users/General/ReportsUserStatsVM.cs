using CommunityCar.Web.Areas.Dashboard.ViewModels.Overview.Users.Trends;

namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.Users.General;

/// <summary>
/// Reports user statistics view model
/// </summary>
public class ReportsUserStatsVM
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int NewUsersToday { get; set; }
    public int OnlineUsers { get; set; }
    public decimal UserGrowthRate { get; set; }
    public int BannedUsers { get; set; }
    public int SuspendedUsers { get; set; }
    public int VerifiedUsers { get; set; }
    public int UnverifiedUsers { get; set; }
    public int AverageSessionDuration { get; set; }
    public int DailyActiveUsers { get; set; }
    public int WeeklyActiveUsers { get; set; }
    public int MonthlyActiveUsers { get; set; }
    public Dictionary<string, int> UsersByRole { get; set; } = new();
    public Dictionary<string, int> UsersByStatus { get; set; } = new();
    public List<UserRegistrationTrendVM> RegistrationTrend { get; set; } = new();
}




