namespace CommunityCar.Domain.Enums.Community;

/// <summary>
/// Represents different moderation actions that can be taken on content
/// </summary>
public enum ModerationAction
{
    /// <summary>
    /// No action taken
    /// </summary>
    None = 0,
    
    /// <summary>
    /// Content approved for publication
    /// </summary>
    Approve = 1,
    
    /// <summary>
    /// Content rejected and hidden
    /// </summary>
    Reject = 2,
    
    /// <summary>
    /// Content flagged for further review
    /// </summary>
    Flag = 3,
    
    /// <summary>
    /// Content removed due to policy violation
    /// </summary>
    Remove = 4,
    
    /// <summary>
    /// Content edited to remove violations
    /// </summary>
    Edit = 5,
    
    /// <summary>
    /// Warning issued to content author
    /// </summary>
    Warn = 6,
    
    /// <summary>
    /// Content author temporarily suspended
    /// </summary>
    Suspend = 7,
    
    /// <summary>
    /// Content author permanently banned
    /// </summary>
    Ban = 8,
    
    /// <summary>
    /// Content marked as spam
    /// </summary>
    MarkAsSpam = 9,
    
    /// <summary>
    /// Content quarantined (limited visibility)
    /// </summary>
    Quarantine = 10
}