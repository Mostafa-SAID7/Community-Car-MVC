namespace CommunityCar.Application.Features.Dashboard.Analytics.Users;

/// <summary>
/// User analytics view model for dashboard analytics
/// </summary>
public class UserAnalyticsVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int TotalSessions { get; set; }
    public int PageViews { get; set; }
    public TimeSpan AverageSessionDuration { get; set; }
    public DateTime LastActivity { get; set; }
    public int InteractionsCount { get; set; }
    public double EngagementRate { get; set; }
    public Dictionary<string, int> ActivityByHour { get; set; } = new();
    public Dictionary<string, int> ActivityByDay { get; set; } = new();
    public List<string> TopPages { get; set; } = new();
}