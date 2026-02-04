using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.AI.ViewModels;

/// <summary>
/// Individual validation test case
/// </summary>
public class ValidationTestCase
{
    /// <summary>
    /// Input for the test case
    /// </summary>
    [Required(ErrorMessage = "Input is required")]
    [StringLength(1000, ErrorMessage = "Input cannot exceed 1000 characters")]
    public string Input { get; set; } = string.Empty;

    /// <summary>
    /// Expected output or pattern
    /// </summary>
    [Required(ErrorMessage = "Expected output is required")]
    [StringLength(2000, ErrorMessage = "Expected output cannot exceed 2000 characters")]
    public string ExpectedOutput { get; set; } = string.Empty;

    /// <summary>
    /// Test case category
    /// </summary>
    [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
    public string Category { get; set; } = "General";

    /// <summary>
    /// Test case description
    /// </summary>
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
}