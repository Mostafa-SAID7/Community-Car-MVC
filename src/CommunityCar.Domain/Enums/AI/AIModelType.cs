namespace CommunityCar.Domain.Enums.AI;

/// <summary>
/// Represents the different types of AI models available in the system
/// </summary>
public enum AIModelType
{
    /// <summary>
    /// Model for classifying user intents from messages
    /// </summary>
    IntentClassification,

    /// <summary>
    /// Model for analyzing sentiment in text content
    /// </summary>
    SentimentAnalysis,

    /// <summary>
    /// Model for generating intelligent responses to user queries
    /// </summary>
    ResponseGeneration,

    /// <summary>
    /// Model for summarizing long text content
    /// </summary>
    TextSummarization,

    /// <summary>
    /// Model for translating text between languages
    /// </summary>
    LanguageTranslation,

    /// <summary>
    /// Model for moderating and filtering inappropriate content
    /// </summary>
    ContentModeration
}