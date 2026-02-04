using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Communication.Chat.ViewModels;

/// <summary>
/// ViewModel for submitting feedback on AI chat messages
/// </summary>
public class FeedbackRequestVM
{
    /// <summary>
    /// The ID of the conversation the feedback is for
    /// </summary>
    [Required(ErrorMessage = "Conversation ID is required")]
    public Guid ConversationId { get; set; }

    /// <summary>
    /// The ID of the specific message being rated
    /// </summary>
    [Required(ErrorMessage = "Message ID is required")]
    public Guid MessageId { get; set; }

    /// <summary>
    /// Rating score (typically 1-5 scale)
    /// </summary>
    [Required(ErrorMessage = "Rating is required")]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    /// <summary>
    /// Optional detailed feedback text
    /// </summary>
    [StringLength(1000, ErrorMessage = "Feedback cannot exceed 1000 characters")]
    public string? Feedback { get; set; }

    /// <summary>
    /// Helper property to check if detailed feedback is provided
    /// </summary>
    public bool HasFeedback => !string.IsNullOrWhiteSpace(Feedback);

    /// <summary>
    /// Helper property to get trimmed feedback
    /// </summary>
    public string? TrimmedFeedback => Feedback?.Trim();

    /// <summary>
    /// Helper property to check if rating is positive (4-5)
    /// </summary>
    public bool IsPositiveRating => Rating >= 4;

    /// <summary>
    /// Helper property to check if rating is negative (1-2)
    /// </summary>
    public bool IsNegativeRating => Rating <= 2;

    /// <summary>
    /// Helper property to check if rating is neutral (3)
    /// </summary>
    public bool IsNeutralRating => Rating == 3;

    /// <summary>
    /// Helper property to get rating description
    /// </summary>
    public string RatingDescription => Rating switch
    {
        1 => "Very Poor",
        2 => "Poor",
        3 => "Average",
        4 => "Good",
        5 => "Excellent",
        _ => "Unknown"
    };

    /// <summary>
    /// Helper property to check if feedback requires attention (low rating)
    /// </summary>
    public bool RequiresAttention => IsNegativeRating;
}