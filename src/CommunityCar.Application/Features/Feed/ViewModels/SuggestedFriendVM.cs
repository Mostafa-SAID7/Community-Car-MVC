namespace CommunityCar.Application.Features.Feed.ViewModels;

public class SuggestedFriendVM
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string? Bio { get; set; }
    public bool IsVerified { get; set; }
    
    // Connection info
    public int MutualFriendsCount { get; set; }
    public List<string> MutualFriendNames { get; set; } = new();
    public string SuggestionReason { get; set; } = string.Empty; // "Mutual friends", "Similar interests", "Location"
    
    // Activity info
    public int PostCount { get; set; }
    public int FollowerCount { get; set; }
    public DateTime LastActiveAt { get; set; }
    public string LastActiveAgo { get; set; } = string.Empty;
    
    // Interests
    public List<string> FavoriteCarMakes { get; set; } = new();
    public List<string> CommonInterests { get; set; } = new();
    public string? Location { get; set; }
    
    // Interaction
    public bool IsFriendRequestSent { get; set; }
    public bool IsFollowing { get; set; }
}


