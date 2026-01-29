namespace CommunityCar.Infrastructure.Configuration.Account.Social;

/// <summary>
/// Social features configuration settings
/// </summary>
public class SocialSettings
{
    public const string SectionName = "Social";

    public FollowingSettings Following { get; set; } = new();
    public InteractionSettings Interactions { get; set; } = new();
    public MessagingSettings Messaging { get; set; } = new();
    public GroupSettings Groups { get; set; } = new();
    public FeedSettings Feed { get; set; } = new();
}

/// <summary>
/// Following/friendship settings
/// </summary>
public class FollowingSettings
{
    public bool EnableFollowing { get; set; } = true;
    public bool RequireFollowApproval { get; set; } = false;
    public int MaxFollowing { get; set; } = 5000;
    public int MaxFollowers { get; set; } = 10000;
    public bool ShowFollowerCount { get; set; } = true;
    public bool ShowFollowingCount { get; set; } = true;
    public bool EnableMutualFollowing { get; set; } = true;
    public bool EnableFollowSuggestions { get; set; } = true;
    public int FollowSuggestionsCount { get; set; } = 10;
    public bool EnableFollowNotifications { get; set; } = true;
    public bool AllowBlockingFollowers { get; set; } = true;
    public bool EnableFollowingActivity { get; set; } = true;
    public TimeSpan FollowingActivityRetention { get; set; } = TimeSpan.FromDays(30);
}

/// <summary>
/// User interaction settings
/// </summary>
public class InteractionSettings
{
    public bool EnableLikes { get; set; } = true;
    public bool EnableComments { get; set; } = true;
    public bool EnableShares { get; set; } = true;
    public bool EnableReactions { get; set; } = true;
    public bool EnableBookmarks { get; set; } = true;
    public bool EnableRatings { get; set; } = true;
    public bool EnableVoting { get; set; } = true;
    public int MaxCommentsPerPost { get; set; } = 1000;
    public int MaxCommentLength { get; set; } = 2000;
    public bool EnableCommentReplies { get; set; } = true;
    public int MaxCommentDepth { get; set; } = 5;
    public bool EnableCommentLikes { get; set; } = true;
    public bool RequireCommentApproval { get; set; } = false;
    public bool EnableInteractionNotifications { get; set; } = true;
    public TimeSpan InteractionCooldown { get; set; } = TimeSpan.FromSeconds(1);
}

/// <summary>
/// Messaging system settings
/// </summary>
public class MessagingSettings
{
    public bool EnableDirectMessages { get; set; } = true;
    public bool EnableGroupMessages { get; set; } = true;
    public bool RequireConnectionForMessages { get; set; } = false;
    public int MaxMessageLength { get; set; } = 5000;
    public int MaxMessagesPerConversation { get; set; } = 10000;
    public int MaxConversationsPerUser { get; set; } = 500;
    public int MaxParticipantsPerGroup { get; set; } = 50;
    public bool EnableMessageAttachments { get; set; } = true;
    public long MaxAttachmentSize { get; set; } = 10 * 1024 * 1024; // 10MB
    public string[] AllowedAttachmentTypes { get; set; } = { "jpg", "jpeg", "png", "gif", "pdf", "doc", "docx" };
    public bool EnableMessageEncryption { get; set; } = false;
    public bool EnableMessageNotifications { get; set; } = true;
    public bool EnableReadReceipts { get; set; } = true;
    public bool EnableTypingIndicators { get; set; } = true;
    public TimeSpan MessageRetentionPeriod { get; set; } = TimeSpan.FromDays(365);
    public bool EnableMessageSearch { get; set; } = true;
}

/// <summary>
/// Group/community settings
/// </summary>
public class GroupSettings
{
    public bool EnableGroups { get; set; } = true;
    public bool AllowGroupCreation { get; set; } = true;
    public int MaxGroupsPerUser { get; set; } = 20;
    public int MaxMembersPerGroup { get; set; } = 10000;
    public bool EnablePrivateGroups { get; set; } = true;
    public bool EnableGroupInvitations { get; set; } = true;
    public bool EnableGroupRequests { get; set; } = true;
    public bool RequireGroupApproval { get; set; } = false;
    public bool EnableGroupModeration { get; set; } = true;
    public int MaxModeratorsPerGroup { get; set; } = 10;
    public bool EnableGroupEvents { get; set; } = true;
    public bool EnableGroupFiles { get; set; } = true;
    public long MaxGroupFileSize { get; set; } = 50 * 1024 * 1024; // 50MB
    public bool EnableGroupNotifications { get; set; } = true;
    public bool EnableGroupAnalytics { get; set; } = true;
}

/// <summary>
/// Social feed settings
/// </summary>
public class FeedSettings
{
    public bool EnablePersonalizedFeed { get; set; } = true;
    public bool EnableChronologicalFeed { get; set; } = true;
    public bool EnableTrendingFeed { get; set; } = true;
    public int DefaultFeedPageSize { get; set; } = 20;
    public int MaxFeedPageSize { get; set; } = 100;
    public TimeSpan FeedCacheExpiry { get; set; } = TimeSpan.FromMinutes(15);
    public bool EnableFeedFiltering { get; set; } = true;
    public bool EnableContentRecommendations { get; set; } = true;
    public int RecommendationsCount { get; set; } = 5;
    public bool EnableFeedCustomization { get; set; } = true;
    public bool ShowFeedMetrics { get; set; } = false;
    public TimeSpan FeedUpdateInterval { get; set; } = TimeSpan.FromMinutes(5);
    public bool EnableRealTimeFeed { get; set; } = false;
    public bool EnableFeedNotifications { get; set; } = true;
}