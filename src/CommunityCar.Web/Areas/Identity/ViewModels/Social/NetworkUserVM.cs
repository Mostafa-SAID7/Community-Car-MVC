namespace CommunityCar.Web.Areas.Identity.ViewModels.Social;

public class NetworkUserVM
{
    public Guid Id { get; set; }
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
    public string FollowerName { get; set; } = string.Empty;
    public string FollowingName { get; set; } = string.Empty;
    public string? FollowerProfilePicture { get; set; }
    public string? FollowingProfilePicture { get; set; }
    public string? FollowerBio { get; set; }
    public string? FollowingBio { get; set; }
    public DateTime CreatedAt { get; set; }
    public string FollowedTimeAgo { get; set; } = string.Empty;
    public bool IsMutual { get; set; }
    public bool IsOnline { get; set; }
    public DateTime? LastSeen { get; set; }
}
