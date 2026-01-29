namespace CommunityCar.Application.Features.Account.DTOs.Activity;

public class ActivityAnalyticsDTO
{
    public Guid UserId { get; set; }
    public int TotalActivities { get; set; }
    public Dictionary<string, int> ActivitiesByType { get; set; } = new();
    public DateTime? LastActivityDate { get; set; }
    public List<ActivityTrendDTO> ActivityTrends { get; set; } = new();
}

public class ActivityTrendDTO
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
    public string ActivityType { get; set; } = string.Empty;
}

public class ActivitySummaryDTO
{
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public int TotalActivities { get; set; }
    public Dictionary<string, int> ActivityBreakdown { get; set; } = new();
    public TimeSpan TotalActiveTime { get; set; }
}