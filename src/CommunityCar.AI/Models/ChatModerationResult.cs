namespace CommunityCar.AI.Models;

/// <summary>
/// Result of content moderation analysis
/// </summary>
public class ChatModerationResult
{
    /// <summary>
    /// Unique identifier for the message being moderated
    /// </summary>
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// User ID of the message sender
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The original message content
    /// </summary>
    public string OriginalMessage { get; set; } = string.Empty;

    /// <summary>
    /// Whether the message should be blocked
    /// </summary>
    public bool IsBlocked { get; set; }

    /// <summary>
    /// Whether the message is classified as spam
    /// </summary>
    public bool IsSpam { get; set; }

    /// <summary>
    /// Whether the message contains inappropriate content
    /// </summary>
    public bool HasInappropriateContent { get; set; }

    /// <summary>
    /// Toxicity score (0.0 to 1.0, higher is more toxic)
    /// </summary>
    public float ToxicityScore { get; set; }

    /// <summary>
    /// List of reasons why the message was flagged
    /// </summary>
    public List<string> ModerationReasons { get; set; } = new();

    /// <summary>
    /// AI-suggested alternative message if blocked
    /// </summary>
    public string SuggestedAlternative { get; set; } = string.Empty;
}