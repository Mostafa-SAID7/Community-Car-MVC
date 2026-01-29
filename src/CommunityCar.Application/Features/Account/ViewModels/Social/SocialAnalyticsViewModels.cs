using CommunityCar.Application.Features.Account.ViewModels.Core;

namespace CommunityCar.Application.Features.Account.ViewModels.Social;

public class FollowingAnalyticsVM
{
    public Guid UserId { get; set; }
    public int FollowingCount { get; set; }
    public int FollowerCount { get; set; }
    public int MutualFollowingCount { get; set; }
    public List<NetworkUserVM> RecentFollowers { get; set; } = new();
    public List<NetworkUserVM> RecentFollowing { get; set; } = new();
    public List<UserSuggestionVM> SuggestedFollowing { get; set; } = new();
}

public class InterestAnalyticsVM
{
    public Guid UserId { get; set; }
    public int TotalInterests { get; set; }
    public Dictionary<string, int> InterestsByCategory { get; set; } = new();
    public List<ProfileInterestVM> TopInterests { get; set; } = new();
    public List<string> RecommendedInterests { get; set; } = new();
    public List<UserSuggestionVM> SimilarUsers { get; set; } = new();
}