namespace CommunityCar.Application.Features.Dashboard.Analytics.ViewModels;

public class UserAnalyticsVM
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int NewUsers { get; set; }
    public int ReturnUsers { get; set; }
    public decimal UserGrowthRate { get; set; }
    public decimal UserRetentionRate { get; set; }
    public int AverageSessionDuration { get; set; }
    public int PageViewsPerUser { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<TopUserVM> TopUsers { get; set; } = new();
    public List<UserActivityVM> UserActivities { get; set; } = new();
}

public class TopUserVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int ActivityCount { get; set; }
    public DateTime LastActivity { get; set; }
}

public class UserActivityVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string ActivityType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Details { get; set; } = string.Empty;
}