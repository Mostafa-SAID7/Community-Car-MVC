using CommunityCar.Application.Features.Shared.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Areas.Identity.ViewModels.Core;

public class UserActivityStatsVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int TotalPosts { get; set; }
    public int TotalComments { get; set; }
    public int TotalLikes { get; set; }
    public int TotalShares { get; set; }
    public int TotalViews { get; set; }
    public int TotalFollowers { get; set; }
    public int TotalFollowing { get; set; }
    public DateTime LastActiveAt { get; set; }
    public DateTime JoinedAt { get; set; }
    public double EngagementRate { get; set; }
    public int DaysActive { get; set; }
    public int PostsThisWeek { get; set; }
    public int PostsThisMonth { get; set; }
    public List<ChartDataVM> ActivityTrend { get; set; } = new();
    public Dictionary<string, int> ActivityByType { get; set; } = new();
}
