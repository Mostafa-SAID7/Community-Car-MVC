using System.ComponentModel.DataAnnotations;

using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Web.Areas.Identity.ViewModels.Core;

public class UserSocialStatsVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public int FriendsCount { get; set; }
    public int MutualFriendsCount { get; set; }
    public int GroupsJoined { get; set; }
    public int EventsAttended { get; set; }
    public int EventsCreated { get; set; }
    public double SocialInfluenceScore { get; set; }
    public double NetworkReachScore { get; set; }
    public int DirectMessages { get; set; }
    public int GroupMessages { get; set; }
    public DateTime LastSocialActivity { get; set; }
    public List<ChartDataVM> SocialGrowthTrend { get; set; } = new();
    public Dictionary<string, int> ConnectionsByType { get; set; } = new();
    public List<string> TopInteractions { get; set; } = new();
}
