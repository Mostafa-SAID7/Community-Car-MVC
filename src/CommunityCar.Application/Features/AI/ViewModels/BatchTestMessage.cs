using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.AI.ViewModels;

/// <summary>
/// Individual test message for batch testing
/// </summary>
public class BatchTestMessage
{
    /// <summary>
    /// The test message content
    /// </summary>
    [Required(ErrorMessage = "Message is required")]
    [StringLength(2000, ErrorMessage = "Message cannot exceed 2000 characters")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Optional context for the message
    /// </summary>
    [StringLength(1000, ErrorMessage = "Context cannot exceed 1000 characters")]
    public string? Context { get; set; }
}