using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Community.Friends.ViewModels;

/// <summary>
/// View model for friendship actions (accept, decline, etc.)
/// </summary>
public class FriendshipActionVM
{
    [Required(ErrorMessage = "Friendship ID is required")]
    public Guid FriendshipId { get; set; }

    /// <summary>
    /// Optional reason for the action (e.g., decline reason)
    /// </summary>
    [StringLength(200, ErrorMessage = "Reason cannot exceed 200 characters")]
    public string? Reason { get; set; }

    /// <summary>
    /// Helper property to validate friendship ID
    /// </summary>
    public bool IsValidFriendshipId => FriendshipId != Guid.Empty;
}