namespace CommunityCar.Application.Features.Account.ViewModels.Activity;

public class ProfileViewerVM
{
    public Guid ViewerId { get; set; }
    public string ViewerName { get; set; } = string.Empty;
    public string? ViewerProfilePicture { get; set; }
    public int ViewCount { get; set; }
    public DateTime LastViewedAt { get; set; }
    public string LastViewedTimeAgo { get; set; } = string.Empty;
    public bool IsFollowing { get; set; }
    public TimeSpan AverageViewDuration { get; set; }
    public DateTime FirstViewedAt { get; set; }
    public bool IsMutualFollowing { get; set; }
    public string? ViewerLocation { get; set; }
}