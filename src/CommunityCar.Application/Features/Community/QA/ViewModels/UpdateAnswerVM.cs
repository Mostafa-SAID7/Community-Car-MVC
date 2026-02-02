using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Community.QA.ViewModels;

/// <summary>
/// View model for updating an existing answer
/// </summary>
public class UpdateAnswerVM
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid QuestionId { get; set; }

    [Required(ErrorMessage = "Answer body is required")]
    [StringLength(10000, ErrorMessage = "Answer cannot exceed 10,000 characters")]
    [Display(Name = "Your Answer")]
    public string Body { get; set; } = string.Empty;

    [StringLength(10000, ErrorMessage = "Arabic answer cannot exceed 10,000 characters")]
    [Display(Name = "Arabic Answer")]
    public string? BodyAr { get; set; }

    /// <summary>
    /// Reason for the update (optional)
    /// </summary>
    [StringLength(200, ErrorMessage = "Update reason cannot exceed 200 characters")]
    [Display(Name = "Reason for Update")]
    public string? UpdateReason { get; set; }

    /// <summary>
    /// Question context for display purposes
    /// </summary>
    [Display(Name = "Question")]
    public string QuestionTitle { get; set; } = string.Empty;

    /// <summary>
    /// Original answer body for comparison
    /// </summary>
    public string OriginalBody { get; set; } = string.Empty;

    /// <summary>
    /// Answer creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Helper property to check if answer has been modified
    /// </summary>
    public bool IsModified => !string.Equals(Body?.Trim(), OriginalBody?.Trim(), StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Helper property to get word count
    /// </summary>
    public int WordCount => 
        string.IsNullOrWhiteSpace(Body) ? 0 : 
        Body.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;

    /// <summary>
    /// Helper property for time since creation
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