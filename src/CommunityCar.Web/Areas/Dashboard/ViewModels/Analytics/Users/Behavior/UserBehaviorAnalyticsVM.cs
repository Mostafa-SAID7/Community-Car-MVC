namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Analytics.Users.Behavior;

/// <summary>
/// User behavior analytics view model
/// </summary>
public class UserBehaviorAnalyticsVM
{
    public TimeSpan AverageSessionDuration { get; set; }
    public double PagesPerSession { get; set; }
    public double BounceRate { get; set; }
    public double ReturnVisitorRate { get; set; }
    public int TotalSessions { get; set; }
    public int UniqueVisitors { get; set; }
    public Dictionary<string, int> TopPages { get; set; } = new();
    public Dictionary<string, int> TrafficSources { get; set; } = new();
    public Dictionary<string, int> DeviceTypes { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}




