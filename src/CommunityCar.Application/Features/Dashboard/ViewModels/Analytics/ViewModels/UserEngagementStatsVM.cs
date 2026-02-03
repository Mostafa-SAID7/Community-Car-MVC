namespace CommunityCar.Application.Features.Analytics.ViewModels;

public class UserEngagementStatsVM
{
    public int TotalPosts { get; set; }
    public int TotalLikes { get; set; }
    public int TotalComments { get; set; }
    public int TotalShares { get; set; }
    public int TotalViews { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public double EngagementRate { get; set; }
    public int ActiveDays { get; set; }
    public DateTime? LastActiveAt { get; set; }
    public string EngagementLevel { get; set; } = string.Empty; // Low, Medium, High, Very High
    public Dictionary<string, int> ContentTypeBreakdown { get; set; } = new();
    public Dictionary<string, double> EngagementTrends { get; set; } = new();
}