namespace CommunityCar.Domain.Enums.Account;

/// <summary>
/// Represents different actions a user can perform in the system
/// </summary>
public enum UserAction
{
    /// <summary>
    /// Create a new post
    /// </summary>
    CreatePost,
    
    /// <summary>
    /// Comment on existing posts
    /// </summary>
    CommentOnPost,
    
    /// <summary>
    /// Send direct messages to other users
    /// </summary>
    SendDirectMessage,
    
    /// <summary>
    /// Create new groups
    /// </summary>
    CreateGroup,
    
    /// <summary>
    /// Moderate content (admin/moderator action)
    /// </summary>
    ModerateContent,
    
    /// <summary>
    /// Upload media files
    /// </summary>
    UploadMedia,
    
    /// <summary>
    /// Create events
    /// </summary>
    CreateEvent,
    
    /// <summary>
    /// Write reviews
    /// </summary>
    WriteReview,
    
    /// <summary>
    /// Create guides
    /// </summary>
    CreateGuide,
    
    /// <summary>
    /// Vote on content
    /// </summary>
    VoteOnContent
}