using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Community.Friends.ViewModels;

/// <summary>
/// View model for blocking a user
/// </summary>
public class BlockUserVM
{
    [Required(ErrorMessage = "User ID is required")]
    public Guid UserToBlockId { get; set; }

    /// <summary>
    /// Reason for blocking the user
    /// </summary>
    [Required(ErrorMessage = "Please provide a reason for blocking this user")]
    [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
    [Display(Name = "Reason for Blocking")]
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Confirmation that the user wants to block
    /// </summary>
    [Display(Name = "Confirm Block")]
    public bool ConfirmBlock { get; set; }

    /// <summary>
    /// User's name for display purposes
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Whether to also report the user for inappropriate behavior
    /// </summary>
    [Display(Name = "Report User")]
    public bool ReportUser { get; set; }

    /// <summary>
    /// Helper property to validate user ID
    /// </summary>
    public bool IsValidUserId => UserToBlockId != Guid.Empty;
}