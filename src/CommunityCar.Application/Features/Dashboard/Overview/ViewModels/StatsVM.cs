namespace CommunityCar.Application.Features.Dashboard.Overview.ViewModels;

public class StatsVM
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalPosts { get; set; }
    public int TotalComments { get; set; }
    public int TotalViews { get; set; }
    public int TotalLikes { get; set; }
    public int TotalShares { get; set; }
    public double EngagementRate { get; set; }
    public double GrowthRate { get; set; }
    public int NewUsersToday { get; set; }
    public int NewPostsToday { get; set; }
    public int ActiveUsersToday { get; set; }
}