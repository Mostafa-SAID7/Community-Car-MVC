using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Community.Friends.ViewModels;

/// <summary>
/// View model for sending a friend request
/// </summary>
public class SendFriendRequestVM
{
    [Required(ErrorMessage = "Receiver ID is required")]
    public Guid ReceiverId { get; set; }

    /// <summary>
    /// Optional message to include with the friend request
    /// </summary>
    [StringLength(500, ErrorMessage = "Message cannot exceed 500 characters")]
    public string? Message { get; set; }

    /// <summary>
    /// Helper property to validate that the receiver is not the same as sender
    /// </summary>
    public bool IsValidReceiver(Guid senderId) => ReceiverId != senderId && ReceiverId != Guid.Empty;
}