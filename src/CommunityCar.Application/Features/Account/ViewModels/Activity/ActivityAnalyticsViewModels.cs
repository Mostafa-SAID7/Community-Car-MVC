namespace CommunityCar.Application.Features.Account.ViewModels.Activity;

public class ActivityAnalyticsVM
{
    public Guid UserId { get; set; }
    public int TotalActivities { get; set; }
    public Dictionary<string, int> ActivitiesByType { get; set; } = new();
    public DateTime? LastActivityDate { get; set; }
    public List<ActivityTrendVM> ActivityTrends { get; set; } = new();
    public List<UserActivityVM> RecentActivities { get; set; } = new();
}

public class ActivityTrendVM
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string DateLabel { get; set; } = string.Empty;
}