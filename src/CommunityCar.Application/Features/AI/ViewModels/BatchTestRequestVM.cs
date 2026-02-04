using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.AI.ViewModels;

/// <summary>
/// ViewModel for batch testing requests
/// </summary>
public class BatchTestRequestVM
{
    /// <summary>
    /// List of test messages to process
    /// </summary>
    [Required(ErrorMessage = "Test messages are required")]
    [MinLength(1, ErrorMessage = "At least one test message is required")]
    public List<BatchTestMessage> TestMessages { get; set; } = new();
}