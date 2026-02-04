using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.AI.ViewModels;

/// <summary>
/// ViewModel for AI model validation requests
/// </summary>
public class ModelValidationRequestVM
{
    /// <summary>
    /// Name of the model to validate
    /// </summary>
    [Required(ErrorMessage = "Model name is required")]
    [StringLength(100, ErrorMessage = "Model name cannot exceed 100 characters")]
    public string ModelName { get; set; } = string.Empty;

    /// <summary>
    /// Validation test cases to run
    /// </summary>
    [Required(ErrorMessage = "Test cases are required")]
    [MinLength(1, ErrorMessage = "At least one test case is required")]
    public List<ValidationTestCase> TestCases { get; set; } = new();
}