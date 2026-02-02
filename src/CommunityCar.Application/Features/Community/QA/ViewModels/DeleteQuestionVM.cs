using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Community.QA.ViewModels;

/// <summary>
/// View model for deleting a question
/// </summary>
public class DeleteQuestionVM
{
    [Required]
    public Guid Id { get; set; }

    [Display(Name = "Question Title")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Question Body")]
    public string Body { get; set; } = string.Empty;

    [Display(Name = "Author")]
    public string AuthorName { get; set; } = string.Empty;

    [Display(Name = "Created Date")]
    public DateTime CreatedAt { get; set; }

    [Display(Name = "Answer Count")]
    public int AnswerCount { get; set; }

    [Display(Name = "Vote Count")]
    public int VoteCount { get; set; }

    [Display(Name = "View Count")]
    public int ViewCount { get; set; }

    [Required(ErrorMessage = "Please provide a reason for deletion")]
    [StringLength(500, ErrorMessage = "Deletion reason cannot exceed 500 characters")]
    [Display(Name = "Reason for Deletion")]
    public string DeletionReason { get; set; } = string.Empty;

    [Display(Name = "Confirm Deletion")]
    public bool ConfirmDeletion { get; set; }

    /// <summary>
    /// Helper property to check if question has activity
    /// </summary>
    public bool HasActivity => AnswerCount > 0 || VoteCount > 0 || ViewCount > 10;

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
}