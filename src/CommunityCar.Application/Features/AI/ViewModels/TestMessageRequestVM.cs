using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.AI.ViewModels;

/// <summary>
/// ViewModel for testing AI message functionality
/// </summary>
public class TestMessageRequestVM
{
    /// <summary>
    /// The test message to send to the AI
    /// </summary>
    [Required(ErrorMessage = "Message is required")]
    [StringLength(2000, ErrorMessage = "Message cannot exceed 2000 characters")]
    [MinLength(1, ErrorMessage = "Message cannot be empty")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Context for the test message
    /// </summary>
    [Required(ErrorMessage = "Context is required")]
    [StringLength(1000, ErrorMessage = "Context cannot exceed 1000 characters")]
    public string Context { get; set; } = string.Empty;

    /// <summary>
    /// Helper property to check if message has content
    /// </summary>
    public bool HasMessage => !string.IsNullOrWhiteSpace(Message);

    /// <summary>
    /// Helper property to check if context has content
    /// </summary>
    public bool HasContext => !string.IsNullOrWhiteSpace(Context);

    /// <summary>
    /// Helper property to get trimmed message
    /// </summary>
    public string TrimmedMessage => Message?.Trim() ?? string.Empty;

    /// <summary>
    /// Helper property to get trimmed context
    /// </summary>
    public string TrimmedContext => Context?.Trim() ?? string.Empty;

    /// <summary>
    /// Helper property to get message word count
    /// </summary>
    public int MessageWordCount => TrimmedMessage.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
}