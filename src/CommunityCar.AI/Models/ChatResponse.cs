namespace CommunityCar.AI.Models;

/// <summary>
/// Response model for chat interactions
/// </summary>
public class ChatResponse
{
    /// <summary>
    /// Unique identifier for this message
    /// </summary>
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// User ID associated with this response
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The AI-generated response message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Whether the message was blocked by moderation
    /// </summary>
    public bool IsBlocked { get; set; }

    /// <summary>
    /// Confidence score of the AI response (0.0 to 1.0)
    /// </summary>
    public float Confidence { get; set; }

    /// <summary>
    /// Conversation ID this response belongs to
    /// </summary>
    public Guid ConversationId { get; set; }

    /// <summary>
    /// Detailed sentiment analysis of the message
    /// </summary>
    public EnhancedSentimentPrediction? SentimentAnalysis { get; set; }

    /// <summary>
    /// Content moderation results
    /// </summary>
    public ChatModerationResult? ModerationResult { get; set; }

    /// <summary>
    /// AI-generated suggestions for follow-up actions
    /// </summary>
    public List<string> Suggestions { get; set; } = new();

    /// <summary>
    /// Predicted user engagement score
    /// </summary>
    public float EngagementScore { get; set; }

    /// <summary>
    /// Topics extracted from the message
    /// </summary>
    public List<string> ExtractedTopics { get; set; } = new();

    /// <summary>
    /// Entities extracted from the message
    /// </summary>
    public List<string> ExtractedEntities { get; set; } = new();

    /// <summary>
    /// Timestamp when the response was generated
    /// </summary>
    public DateTime Timestamp { get; set; }
}