namespace CommunityCar.Application.Features.Dashboard.Overview.Users.Activity;

/// <summary>
/// Recent user activity view model for overview
/// </summary>
public class RecentUserActivityVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public string ActivityType { get; set; } = string.Empty;
    public string ActivityDescription { get; set; } = string.Empty;
    public DateTime ActivityTime { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public bool IsOnline { get; set; }
}