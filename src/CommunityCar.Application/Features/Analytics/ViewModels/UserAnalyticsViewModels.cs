using CommunityCar.Domain.Enums.Account;

namespace CommunityCar.Application.Features.Analytics.ViewModels;

public class AnalyticsActivityVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ActivityType ActivityType { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string? EntityTitle { get; set; }
    public string? Description { get; set; }
    public DateTime ActivityDate { get; set; }
    public int Duration { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public string ActivityIcon { get; set; } = string.Empty;
    public string ActivityColor { get; set; } = string.Empty;
}

public class UserActivityStatsVM
{
    public int TotalActivities { get; set; }
    public int ViewsCount { get; set; }
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public int SharesCount { get; set; }
    public int SearchesCount { get; set; }
    public double AverageSessionDuration { get; set; }
    public int ActiveDays { get; set; }
    public DateTime? LastActivity { get; set; }
    public string MostActiveDay { get; set; } = string.Empty;
    public string MostActiveHour { get; set; } = string.Empty;
    public Dictionary<string, int> ActivityBreakdown { get; set; } = new();
    public Dictionary<string, int> DailyActivity { get; set; } = new();
}

public class AnalyticsInterestVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string SubCategory { get; set; } = string.Empty;
    public string InterestType { get; set; } = string.Empty;
    public string InterestValue { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public double Score { get; set; }
    public int InteractionCount { get; set; }
    public DateTime LastInteraction { get; set; }
    public string? Source { get; set; }
    public bool IsActive { get; set; }
    public string ScoreLevel { get; set; } = string.Empty; // Low, Medium, High, Very High
}

public class AnalyticsFollowingVM
{
    public Guid Id { get; set; }
    public Guid FollowerId { get; set; }
    public Guid FollowedUserId { get; set; }
    public string FollowedUserName { get; set; } = string.Empty;
    public string? FollowedUserAvatar { get; set; }
    public DateTime FollowedAt { get; set; }
    public string? FollowReason { get; set; }
    public bool NotificationsEnabled { get; set; }
    public DateTime? LastInteractionAt { get; set; }
    public int InteractionCount { get; set; }
    public bool IsRecentFollower { get; set; }
    public bool IsEngaged { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
}

public class AnalyticsSuggestionVM
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string SuggestionReason { get; set; } = string.Empty;
    public double RelevanceScore { get; set; }
    public int MutualFriendsCount { get; set; }
    public IEnumerable<string> CommonInterests { get; set; } = new List<string>();
    public bool IsVerified { get; set; }
    public string? Location { get; set; }
    public int FollowersCount { get; set; }
    public int PostsCount { get; set; }
}

public class ContentRecommendationVM
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorAvatar { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public double RelevanceScore { get; set; }
    public string RecommendationReason { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public IEnumerable<string> Tags { get; set; } = new List<string>();
}

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

public class TrendingTopicVM
{
    public string Topic { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int PostCount { get; set; }
    public int ViewCount { get; set; }
    public int EngagementCount { get; set; }
    public double TrendingScore { get; set; }
    public string TrendingReason { get; set; } = string.Empty;
    public DateTime FirstSeenAt { get; set; }
    public DateTime LastSeenAt { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public bool IsRising { get; set; }
    public double GrowthRate { get; set; }
}

public class PopularContentVM
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorAvatar { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public int ShareCount { get; set; }
    public double PopularityScore { get; set; }
    public string PopularityReason { get; set; } = string.Empty;
    public IEnumerable<string> Tags { get; set; } = new List<string>();
}

public class UserPrivacySettingsVM
{
    public Guid UserId { get; set; }
    public bool AllowActivityTracking { get; set; }
    public bool AllowInterestTracking { get; set; }
    public bool AllowLocationTracking { get; set; }
    public bool AllowPersonalizedRecommendations { get; set; }
    public bool AllowDataSharing { get; set; }
    public bool AllowAnalytics { get; set; }
    public DateTime LastUpdated { get; set; }
}


