namespace CommunityCar.Domain.Enums;

public enum NotificationType
{
    Info,
    Success,
    Warning,
    Error,
    NewMessage,
    NewAnswer,
    QuestionSolved,
    VoteReceived,
    CommentReceived,
    FriendRequest,
    FriendRequestAccepted,
    FriendRequestDeclined,
    FriendRemoved,
    UserBlocked,
    SystemUpdate,
    
    // Guide-related notifications
    GuidePublished,
    GuideVerified,
    GuideFeatured,
    GuideBookmarked,
    GuideRated,
    GuideCommented,
    GuideUpdated,
    GuideDeleted,
    NewGuideFromFollowedAuthor,
    
    // News-related notifications
    NewsPublished,
    NewsFeatured,
    NewsLiked,
    NewsCommented,
    NewsShared,
    BreakingNews,
    NewNewsFromFollowedAuthor,
    
    // General notification types
    Achievement,
    Interaction,
    NewContent,
    Breaking
}