namespace CommunityCar.Application.Features.Account.DTOs.Social;

#region Social Analytics DTOs

public class FollowingAnalyticsDTO
{
    public Guid UserId { get; set; }
    public int FollowingCount { get; set; }
    public int FollowerCount { get; set; }
    public int MutualFollowingCount { get; set; }
    public List<UserFollowingDTO> RecentFollowers { get; set; } = new();
    public List<UserFollowingDTO> RecentFollowing { get; set; } = new();
    public List<Guid> SuggestedFollowing { get; set; } = new();
}

public class InterestAnalyticsDTO
{
    public Guid UserId { get; set; }
    public int TotalInterests { get; set; }
    public Dictionary<string, int> InterestsByCategory { get; set; } = new();
    public List<UserInterestDTO> TopInterests { get; set; } = new();
    public List<Guid> RecommendedInterests { get; set; } = new();
    public List<Guid> SimilarUsers { get; set; } = new();
}

public class ProfileViewAnalyticsDTO
{
    public Guid ProfileUserId { get; set; }
    public int TotalViews { get; set; }
    public int UniqueViewers { get; set; }
    public Dictionary<DateTime, int> ViewsByDate { get; set; } = new();
    public List<UserProfileViewDTO> RecentViews { get; set; } = new();
    public List<Guid> TopViewers { get; set; } = new();
    public double AverageViewsPerDay { get; set; }
}

public class SocialEngagementDTO
{
    public Guid UserId { get; set; }
    public int TotalConnections { get; set; }
    public int ActiveConnections { get; set; }
    public int ProfileViews { get; set; }
    public int InteractionScore { get; set; }
    public double EngagementRate { get; set; }
    public Dictionary<string, int> EngagementByType { get; set; } = new();
    public List<Guid> MostEngagedUsers { get; set; } = new();
}

#endregion