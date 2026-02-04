using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Communication.Chat.ViewModels;

/// <summary>
/// ViewModel for starting new AI conversations
/// </summary>
public class StartConversationRequestVM
{
    /// <summary>
    /// Optional title for the conversation
    /// </summary>
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string? Title { get; set; }

    /// <summary>
    /// Optional initial context for the conversation
    /// </summary>
    [StringLength(1000, ErrorMessage = "Context cannot exceed 1000 characters")]
    public string? Context { get; set; }

    /// <summary>
    /// Helper property to check if title is provided
    /// </summary>
    public bool HasTitle => !string.IsNullOrWhiteSpace(Title);

    /// <summary>
    /// Helper property to check if context is provided
    /// </summary>
    public bool HasContext => !string.IsNullOrWhiteSpace(Context);

    /// <summary>
    /// Helper property to get trimmed title
    /// </summary>
    public string? TrimmedTitle => Title?.Trim();

    /// <summary>
    /// Helper property to get display title (with fallback)
    /// </summary>
    public string DisplayTitle => TrimmedTitle ?? "New Conversation";

    /// <summary>
    /// Helper property to check if conversation has any initial data
    /// </summary>
    public bool HasInitialData => HasTitle || HasContext;
}