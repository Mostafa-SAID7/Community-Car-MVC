namespace CommunityCar.AI.Models;

/// <summary>
/// Simple response model for controller compatibility
/// </summary>
public class ChatMessageResponse
{
    /// <summary>
    /// The response message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Confidence score of the response
    /// </summary>
    public float Confidence { get; set; }

    /// <summary>
    /// Suggested follow-up actions or responses
    /// </summary>
    public List<string> Suggestions { get; set; } = new();

    /// <summary>
    /// Conversation ID this response belongs to
    /// </summary>
    public Guid ConversationId { get; set; }
}