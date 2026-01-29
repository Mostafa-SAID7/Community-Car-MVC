namespace CommunityCar.Application.Features.Account.DTOs.Core;

#region User Statistics DTOs

public class UserStatisticsDTO
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
}

public class UserEngagementStatsDTO
{
    public Guid UserId { get; set; }
    public int TotalInteractions { get; set; }
    public int PostsCreated { get; set; }
    public int CommentsCreated { get; set; }
    public int LikesGiven { get; set; }
    public int SharesGiven { get; set; }
    public double EngagementRate { get; set; }
    public int ActiveDays { get; set; }
    public DateTime? LastEngagementDate { get; set; }
    public Dictionary<string, int> InteractionsByType { get; set; } = new();
    public Dictionary<string, int> DailyEngagement { get; set; } = new();
}

public class UserGrowthStatsDTO
{
    public Guid UserId { get; set; }
    public int FollowersGrowth { get; set; }
    public int FollowingGrowth { get; set; }
    public int ContentGrowth { get; set; }
    public int EngagementGrowth { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public Dictionary<string, int> GrowthByPeriod { get; set; } = new();
}

#endregion