using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Community.Friends.ViewModels;

/// <summary>
/// View model for removing a friend
/// </summary>
public class RemoveFriendVM
{
    [Required(ErrorMessage = "Friend ID is required")]
    public Guid FriendId { get; set; }

    /// <summary>
    /// Optional reason for removing the friend
    /// </summary>
    [StringLength(200, ErrorMessage = "Reason cannot exceed 200 characters")]
    public string? Reason { get; set; }

    /// <summary>
    /// Confirmation that the user wants to remove the friend
    /// </summary>
    [Display(Name = "Confirm Removal")]
    public bool ConfirmRemoval { get; set; }

    /// <summary>
    /// Friend's name for display purposes
    /// </summary>
    public string? FriendName { get; set; }

    /// <summary>
    /// Helper property to validate friend ID
    /// </summary>
    public bool IsValidFriendId => FriendId != Guid.Empty;
}