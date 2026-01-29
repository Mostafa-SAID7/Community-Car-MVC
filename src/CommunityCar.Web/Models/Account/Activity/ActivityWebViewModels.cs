namespace CommunityCar.Web.Models.Account.Activity;

public class UserActivityWebVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public string ActivityIcon { get; set; } = string.Empty;
    public string ActivityColor { get; set; } = string.Empty;
}

public class ActivityDashboardWebVM
{
    public Guid UserId { get; set; }
    public List<UserActivityWebVM> RecentActivities { get; set; } = new();
    public Dictionary<string, int> ActivitiesByType { get; set; } = new();
    public int TotalActivities { get; set; }
    public DateTime? LastActivityDate { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool HasMore { get; set; }
}