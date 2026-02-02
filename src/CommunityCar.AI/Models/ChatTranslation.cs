namespace CommunityCar.AI.Models;

/// <summary>
/// Result of message translation
/// </summary>
public class ChatTranslation
{
    /// <summary>
    /// The original message before translation
    /// </summary>
    public string OriginalMessage { get; set; } = string.Empty;

    /// <summary>
    /// Language code of the original message
    /// </summary>
    public string OriginalLanguage { get; set; } = string.Empty;

    /// <summary>
    /// Target language code for translation
    /// </summary>
    public string TargetLanguage { get; set; } = string.Empty;

    /// <summary>
    /// The translated message
    /// </summary>
    public string TranslatedMessage { get; set; } = string.Empty;

    /// <summary>
    /// Confidence score of the translation (0.0 to 1.0)
    /// </summary>
    public float Confidence { get; set; }
}