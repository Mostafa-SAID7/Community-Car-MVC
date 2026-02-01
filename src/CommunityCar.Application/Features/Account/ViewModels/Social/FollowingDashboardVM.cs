namespace CommunityCar.Application.Features.Account.ViewModels.Social;

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