namespace CommunityCar.Web.Areas.Identity.ViewModels.Activity;

public class ActivityAnalyticsVM
{
    public Guid UserId { get; set; }
    public int TotalActivities { get; set; }
    public Dictionary<string, int> ActivitiesByType { get; set; } = new();
    public DateTime? LastActivityDate { get; set; }
    public List<ActivityTrendVM> ActivityTrends { get; set; } = new();
    public List<TimelineActivityVM> RecentActivities { get; set; } = new();
}
