namespace CommunityCar.Application.Features.Account.ViewModels.Social;

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

public class FollowingDashboardVM
{
    public Guid UserId { get; set; }
    public int FollowingCount { get; set; }
    public int FollowerCount { get; set; }
    public int MutualFollowingCount { get; set; }
    public List<NetworkUserVM> RecentFollowers { get; set; } = new();
    public List<NetworkUserVM> RecentFollowing { get; set; } = new();
    public List<UserSuggestionVM> SuggestedFollowing { get; set; } = new();
    public List<NetworkUserVM> MutualConnections { get; set; } = new();
}

public class FollowingListVM
{
    public Guid UserId { get; set; }
    public string ListType { get; set; } = string.Empty; // "following" or "followers"
    public List<NetworkUserVM> Users { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasMore { get; set; }
    public string? SearchTerm { get; set; }
}

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

public class FollowStatsVM
{
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public bool IsFollowing { get; set; }
    public bool IsFollowedBy { get; set; }
    public bool CanFollow { get; set; } = true;
}

// Aliases
public class UserFollowingWebVM : NetworkUserVM { }
public class FollowingDashboardWebVM : FollowingDashboardVM { }
public class FollowingListWebVM : FollowingListVM { }

