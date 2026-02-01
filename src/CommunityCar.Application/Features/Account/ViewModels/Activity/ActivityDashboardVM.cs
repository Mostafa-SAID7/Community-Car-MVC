namespace CommunityCar.Application.Features.Account.ViewModels.Activity;

public class ActivityDashboardVM
{
    public Guid UserId { get; set; }
    public List<TimelineActivityVM> RecentActivities { get; set; } = new();
    public Dictionary<string, int> ActivitiesByType { get; set; } = new();
    public int TotalActivities { get; set; }
    public DateTime? LastActivityDate { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool HasMore { get; set; }
}