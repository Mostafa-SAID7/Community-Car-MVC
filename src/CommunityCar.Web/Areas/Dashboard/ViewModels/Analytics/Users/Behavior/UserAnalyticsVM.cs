namespace CommunityCar.Application.Features.Dashboard.Analytics.Users.Behavior;

/// <summary>
/// User analytics view model
/// </summary>
public class UserAnalyticsVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime AnalysisDate { get; set; }
    public int PageViews { get; set; }
    public int SessionCount { get; set; }
    public TimeSpan AverageSessionDuration { get; set; }
    public int InteractionCount { get; set; }
    public double EngagementScore { get; set; }
    public List<string> TopPages { get; set; } = new();
    public Dictionary<string, int> ActivityByHour { get; set; } = new();
    public Dictionary<string, int> DeviceTypes { get; set; } = new();
}

/// <summary>
/// User activity view model
/// </summary>
public class UserActivityVM
{
    public Guid UserId { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public DateTime ActivityDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}