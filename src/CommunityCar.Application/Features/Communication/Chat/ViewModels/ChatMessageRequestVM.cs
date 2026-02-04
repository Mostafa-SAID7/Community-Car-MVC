using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Chat.ViewModels;

/// <summary>
/// ViewModel for AI chat message requests
/// </summary>
public class ChatMessageRequestVM
{
    /// <summary>
    /// The message content to send to the AI
    /// </summary>
    [Required(ErrorMessage = "Message is required")]
    [StringLength(4000, ErrorMessage = "Message cannot exceed 4000 characters")]
    [MinLength(1, ErrorMessage = "Message cannot be empty")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Optional context for the message (conversation history, topic, etc.)
    /// </summary>
    [StringLength(1000, ErrorMessage = "Context cannot exceed 1000 characters")]
    public string? Context { get; set; }

    /// <summary>
    /// Helper property to check if message has content
    /// </summary>
    public bool HasMessage => !string.IsNullOrWhiteSpace(Message);

    /// <summary>
    /// Helper property to check if context is provided
    /// </summary>
    public bool HasContext => !string.IsNullOrWhiteSpace(Context);

    /// <summary>
    /// Helper property to get trimmed message
    /// </summary>
    public string TrimmedMessage => Message?.Trim() ?? string.Empty;

    /// <summary>
    /// Helper property to get message word count
    /// </summary>
    public int WordCount => TrimmedMessage.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
}