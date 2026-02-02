using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Community.QA.ViewModels;

/// <summary>
/// View model for creating a new answer
/// </summary>
public class CreateAnswerVM
{
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
    /// Question context for display purposes
    /// </summary>
    [Display(Name = "Question")]
    public string QuestionTitle { get; set; } = string.Empty;

    /// <summary>
    /// Question body for context
    /// </summary>
    public string QuestionBody { get; set; } = string.Empty;

    /// <summary>
    /// Question author for context
    /// </summary>
    public string QuestionAuthor { get; set; } = string.Empty;

    /// <summary>
    /// Helper property to check if answer has content
    /// </summary>
    public bool HasContent => !string.IsNullOrWhiteSpace(Body);

    /// <summary>
    /// Helper property to get word count
    /// </summary>
    public int WordCount => 
        string.IsNullOrWhiteSpace(Body) ? 0 : 
        Body.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
}