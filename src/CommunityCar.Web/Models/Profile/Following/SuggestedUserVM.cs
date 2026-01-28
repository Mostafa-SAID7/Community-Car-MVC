namespace CommunityCar.Web.Models.Profile.Following;

/// <summary>
/// Suggested user for following
/// </summary>
public class SuggestedUserVM
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public int MutualFollowersCount { get; set; }
    public List<string> MutualFollowerNames { get; set; } = new();
    public string SuggestionReason { get; set; } = string.Empty;
    public bool IsFollowing { get; set; }
    
    // Statistics
    public int FollowersCount { get; set; }
    public int PostsCount { get; set; }
}