namespace CommunityCar.AI.Models;

/// <summary>
/// Request model for chat interactions
/// </summary>
public class ChatRequest
{
    /// <summary>
    /// Unique identifier of the user sending the message
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The message content from the user
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Unique identifier of the conversation
    /// </summary>
    public string ConversationId { get; set; } = string.Empty;

    /// <summary>
    /// Additional context for the message
    /// </summary>
    public string Context { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the message was created
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}