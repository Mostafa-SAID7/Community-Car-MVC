using System.ComponentModel.DataAnnotations;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.QA.ViewModels;

/// <summary>
/// View model for updating an existing question
/// </summary>
public class UpdateQuestionVM
{
    [Required]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    [Display(Name = "Question Title")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Question body is required")]
    [StringLength(10000, ErrorMessage = "Question body cannot exceed 10,000 characters")]
    [Display(Name = "Question Details")]
    public string Body { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Arabic title cannot exceed 200 characters")]
    [Display(Name = "Arabic Title")]
    public string? TitleAr { get; set; }

    [StringLength(10000, ErrorMessage = "Arabic body cannot exceed 10,000 characters")]
    [Display(Name = "Arabic Details")]
    public string? BodyAr { get; set; }

    [Display(Name = "Difficulty Level")]
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Beginner;

    [StringLength(50, ErrorMessage = "Car make cannot exceed 50 characters")]
    [Display(Name = "Car Make")]
    public string? CarMake { get; set; }

    [StringLength(50, ErrorMessage = "Car model cannot exceed 50 characters")]
    [Display(Name = "Car Model")]
    public string? CarModel { get; set; }

    [Range(1900, 2030, ErrorMessage = "Car year must be between 1900 and 2030")]
    [Display(Name = "Car Year")]
    public int? CarYear { get; set; }

    [StringLength(100, ErrorMessage = "Car engine cannot exceed 100 characters")]
    [Display(Name = "Engine Type")]
    public string? CarEngine { get; set; }

    [StringLength(500, ErrorMessage = "Tags cannot exceed 500 characters")]
    [Display(Name = "Tags (comma-separated)")]
    public string? Tags { get; set; }

    /// <summary>
    /// Reason for the update (optional)
    /// </summary>
    [StringLength(200, ErrorMessage = "Update reason cannot exceed 200 characters")]
    [Display(Name = "Reason for Update")]
    public string? UpdateReason { get; set; }

    /// <summary>
    /// Helper property to get tags as a list
    /// </summary>
    public List<string> TagsList => 
        Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .Where(t => !string.IsNullOrEmpty(t))
            .ToList() ?? new List<string>();

    /// <summary>
    /// Helper property for car display name
    /// </summary>
    public string CarDisplayName => 
        !string.IsNullOrEmpty(CarMake) && !string.IsNullOrEmpty(CarModel) 
            ? $"{CarYear} {CarMake} {CarModel}".Trim()
            : !string.IsNullOrEmpty(CarMake) 
                ? CarMake 
                : string.Empty;
}