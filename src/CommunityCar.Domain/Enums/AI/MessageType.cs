namespace CommunityCar.Domain.Enums.AI;

/// <summary>
/// Represents different types of messages for AI processing
/// </summary>
public enum MessageType
{
    /// <summary>
    /// General message without specific classification
    /// </summary>
    General,

    /// <summary>
    /// Message containing a question
    /// </summary>
    Question,

    /// <summary>
    /// Message reporting a problem or issue
    /// </summary>
    Problem,

    /// <summary>
    /// Greeting or welcome message
    /// </summary>
    Greeting,

    /// <summary>
    /// Message expressing appreciation or thanks
    /// </summary>
    Appreciation,

    /// <summary>
    /// Message containing a complaint
    /// </summary>
    Complaint
}