using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Community.Feed.ViewModels;

/// <summary>
/// ViewModel for handling feed interaction requests (like, share, bookmark, hide, report, mark as seen)
/// </summary>
public class FeedInteractionRequestVM
{
    /// <summary>
    /// The ID of the content being interacted with
    /// </summary>
    [Required(ErrorMessage = "Content ID is required")]
    public Guid ContentId { get; set; }

    /// <summary>
    /// The type of content (Post, Story, Guide, etc.)
    /// </summary>
    [Required(ErrorMessage = "Content type is required")]
    [StringLength(50, ErrorMessage = "Content type cannot exceed 50 characters")]
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// The type of interaction (like, share, bookmark, hide, report, mark-seen)
    /// </summary>
    [StringLength(20, ErrorMessage = "Interaction type cannot exceed 20 characters")]
    public string? InteractionType { get; set; }

    /// <summary>
    /// Reason for the interaction (mainly used for reporting)
    /// </summary>
    [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
    public string? Reason { get; set; }

    /// <summary>
    /// Helper property to check if this is a like interaction
    /// </summary>
    public bool IsLike => InteractionType?.ToLower() == "like";

    /// <summary>
    /// Helper property to check if this is a share interaction
    /// </summary>
    public bool IsShare => InteractionType?.ToLower() == "share";

    /// <summary>
    /// Helper property to check if this is a bookmark interaction
    /// </summary>
    public bool IsBookmark => InteractionType?.ToLower() == "bookmark";

    /// <summary>
    /// Helper property to check if this is a hide interaction
    /// </summary>
    public bool IsHide => InteractionType?.ToLower() == "hide";

    /// <summary>
    /// Helper property to check if this is a report interaction
    /// </summary>
    public bool IsReport => InteractionType?.ToLower() == "report";

    /// <summary>
    /// Helper property to check if this is a mark as seen interaction
    /// </summary>
    public bool IsMarkSeen => InteractionType?.ToLower() == "mark-seen";

    /// <summary>
    /// Helper property to check if a reason is required (for reports)
    /// </summary>
    public bool RequiresReason => IsReport;
}