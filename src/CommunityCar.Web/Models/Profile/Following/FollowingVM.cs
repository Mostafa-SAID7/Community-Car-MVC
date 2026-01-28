namespace CommunityCar.Web.Models.Profile.Following;

/// <summary>
/// Represents a user in following/followers lists
/// </summary>
public class FollowingVM
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public DateTime FollowedAt { get; set; }
    public bool IsFollowingBack { get; set; }
    public bool IsOnline { get; set; }
    public DateTime? LastActiveAt { get; set; }
    
    // Statistics
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public int PostsCount { get; set; }
}