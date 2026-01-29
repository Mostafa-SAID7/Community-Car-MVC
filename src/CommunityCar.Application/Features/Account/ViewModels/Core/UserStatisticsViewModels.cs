namespace CommunityCar.Application.Features.Account.ViewModels.Core;

public class UserStatisticsVM
{
    public Guid UserId { get; set; }
    public int PostsCount { get; set; }
    public int CommentsCount { get; set; }
    public int LikesReceived { get; set; }
    public int SharesReceived { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public int AchievementsCount { get; set; }
    public int BadgesCount { get; set; }
    public int GalleryItemsCount { get; set; }
    public DateTime JoinedDate { get; set; }
    public int DaysActive { get; set; }
    public DateTime? LastActivityDate { get; set; }
    public string JoinedTimeAgo { get; set; } = string.Empty;
    public string LastActivityTimeAgo { get; set; } = string.Empty;
}

public class UserEngagementVM
{
    public Guid UserId { get; set; }
    public int TotalPosts { get; set; }
    public int TotalLikes { get; set; }
    public int TotalComments { get; set; }
    public int TotalShares { get; set; }
    public int TotalViews { get; set; }
    public double EngagementRate { get; set; }
    public int ActiveDays { get; set; }
    public DateTime? LastActiveAt { get; set; }
    public string EngagementLevel { get; set; } = string.Empty;
    public string EngagementLevelColor { get; set; } = string.Empty;
    public Dictionary<string, int> ContentTypeBreakdown { get; set; } = new();
    public Dictionary<string, double> EngagementTrends { get; set; } = new();
}