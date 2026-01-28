namespace CommunityCar.Domain.Enums.Community;

/// <summary>
/// Represents the status of community content (posts, comments, etc.)
/// </summary>
public enum ContentStatus
{
    /// <summary>
    /// Content is in draft state
    /// </summary>
    Draft = 0,
    
    /// <summary>
    /// Content is pending moderation review
    /// </summary>
    PendingReview = 1,
    
    /// <summary>
    /// Content is published and visible
    /// </summary>
    Published = 2,
    
    /// <summary>
    /// Content is archived (not visible but preserved)
    /// </summary>
    Archived = 3,
    
    /// <summary>
    /// Content is hidden due to reports or violations
    /// </summary>
    Hidden = 4,
    
    /// <summary>
    /// Content is deleted (soft delete)
    /// </summary>
    Deleted = 5,
    
    /// <summary>
    /// Content is featured/highlighted
    /// </summary>
    Featured = 6,
    
    /// <summary>
    /// Content is pinned to top
    /// </summary>
    Pinned = 7
}