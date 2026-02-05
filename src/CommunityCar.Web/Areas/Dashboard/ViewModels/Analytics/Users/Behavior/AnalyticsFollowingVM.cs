namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Analytics.Users.Behavior;

public class AnalyticsFollowingVM
{
    public Guid Id { get; set; }
    public Guid FollowerId { get; set; }
    public Guid FollowedUserId { get; set; }
    public string FollowedUserName { get; set; } = string.Empty;
    public string? FollowedUserAvatar { get; set; }
    public DateTime FollowedAt { get; set; }
    public string? FollowReason { get; set; }
    public bool NotificationsEnabled { get; set; }
    public DateTime? LastInteractionAt { get; set; }
    public int InteractionCount { get; set; }
    public bool IsRecentFollower { get; set; }
    public bool IsEngaged { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
}




