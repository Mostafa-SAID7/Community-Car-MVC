namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Overview.Users.Statistics;

/// <summary>
/// User activity statistics view model
/// </summary>
public class UserActivityStatsVM
{
    public int TotalUsers { get; set; }
    public int ActiveUsersToday { get; set; }
    public int ActiveUsersThisWeek { get; set; }
    public int ActiveUsersThisMonth { get; set; }
    public double AverageSessionDuration { get; set; }
    public int TotalSessions { get; set; }
    public int TotalPageViews { get; set; }
    public Dictionary<string, int> ActivityByHour { get; set; } = new();
    public Dictionary<string, int> ActivityByDay { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}




