namespace CommunityCar.Application.Features.Dashboard.Overview.Users.Activity;

/// <summary>
/// User activity overview view model
/// </summary>
public class UserActivityOverviewVM
{
    public int TotalActiveUsers { get; set; }
    public int DailyActiveUsers { get; set; }
    public int WeeklyActiveUsers { get; set; }
    public int MonthlyActiveUsers { get; set; }
    public decimal ActivityGrowthRate { get; set; }
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// User activity summary view model
/// </summary>
public class UserActivitySummaryVM
{
    public DateTime Date { get; set; }
    public int LoginCount { get; set; }
    public int PageViews { get; set; }
    public int InteractionsCount { get; set; }
    public TimeSpan AverageSessionDuration { get; set; }
    public int UniqueUsers { get; set; }
    
    // Missing properties that services expect
    public int TotalActivities { get; set; }
    public List<string> MostActiveUsers { get; set; } = new();
    public Dictionary<string, int> ActivityByType { get; set; } = new();
}

/// <summary>
/// User activity trend view model
/// </summary>
public class UserActivityTrendVM
{
    public DateTime Date { get; set; }
    public int ActiveUsers { get; set; }
    public int NewUsers { get; set; }
    public int ReturningUsers { get; set; }
    public decimal EngagementRate { get; set; }
}

/// <summary>
/// User engagement overview view model
/// </summary>
public class UserEngagementOverviewVM
{
    public decimal OverallEngagementRate { get; set; }
    public int TotalInteractions { get; set; }
    public int TotalSessions { get; set; }
    public TimeSpan AverageSessionDuration { get; set; }
    public decimal BounceRate { get; set; }
    
    // Missing properties that services expect
    public decimal AverageEngagementScore { get; set; }
    public int HighlyEngagedUsers { get; set; }
    public int LowEngagementUsers { get; set; }
    public List<string> EngagementTrend { get; set; } = new();
}