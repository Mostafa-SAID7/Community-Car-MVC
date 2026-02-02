using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Community.QA.ViewModels;

/// <summary>
/// View model for deleting an answer
/// </summary>
public class DeleteAnswerVM
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid QuestionId { get; set; }

    [Display(Name = "Answer Body")]
    public string Body { get; set; } = string.Empty;

    [Display(Name = "Question Title")]
    public string QuestionTitle { get; set; } = string.Empty;

    [Display(Name = "Author")]
    public string AuthorName { get; set; } = string.Empty;

    [Display(Name = "Created Date")]
    public DateTime CreatedAt { get; set; }

    [Display(Name = "Vote Count")]
    public int VoteCount { get; set; }

    [Display(Name = "Helpful Count")]
    public int HelpfulCount { get; set; }

    [Display(Name = "Is Accepted Answer")]
    public bool IsAccepted { get; set; }

    [Required(ErrorMessage = "Please provide a reason for deletion")]
    [StringLength(500, ErrorMessage = "Deletion reason cannot exceed 500 characters")]
    [Display(Name = "Reason for Deletion")]
    public string DeletionReason { get; set; } = string.Empty;

    [Display(Name = "Confirm Deletion")]
    public bool ConfirmDeletion { get; set; }

    /// <summary>
    /// Helper property to check if answer has significant activity
    /// </summary>
    public bool HasSignificantActivity => VoteCount > 0 || HelpfulCount > 0 || IsAccepted;

    /// <summary>
    /// Helper property for time ago display
    /// </summary>
    public string TimeAgo
    {
        get
        {
            var timeAgo = DateTime.UtcNow - CreatedAt;
            return timeAgo.TotalDays > 7 ? CreatedAt.ToString("MMM dd, yyyy") :
                   timeAgo.TotalDays >= 1 ? $"{(int)timeAgo.TotalDays} days ago" :
                   timeAgo.TotalHours >= 1 ? $"{(int)timeAgo.TotalHours} hours ago" :
                   "Just now";
        }
    }

    /// <summary>
    /// Helper property to get truncated body for preview
    /// </summary>
    public string BodyPreview => 
        Body.Length > 150 ? Body.Substring(0, 150) + "..." : Body;
}