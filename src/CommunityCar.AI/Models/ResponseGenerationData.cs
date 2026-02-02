namespace CommunityCar.AI.Models;

/// <summary>
/// Smart response generation data
/// </summary>
public class ResponseGenerationData
{
    public string UserMessage { get; set; } = string.Empty;
    public string Intent { get; set; } = string.Empty;
    public List<string> Entities { get; set; } = new();
    public SentimentPrediction Sentiment { get; set; } = new();
    public Dictionary<string, object> UserProfile { get; set; } = new();
    public List<string> ConversationHistory { get; set; } = new();
    public string PreferredLanguage { get; set; } = "en";
}