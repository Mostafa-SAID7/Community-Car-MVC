namespace CommunityCar.AI.Models;

/// <summary>
/// Comprehensive AI assistant response
/// </summary>
public class AIAssistantResponse
{
    public string Response { get; set; } = string.Empty;
    public string Intent { get; set; } = string.Empty;
    public float IntentConfidence { get; set; }
    public SentimentPrediction Sentiment { get; set; } = new();
    public List<string> SuggestedActions { get; set; } = new();
    public List<ContentRecommendationPrediction> Recommendations { get; set; } = new();
    public Dictionary<string, object> Context { get; set; } = new();
    public bool RequiresHumanEscalation { get; set; }
    public List<string> DetectedEntities { get; set; } = new();
    public string Language { get; set; } = "en";
    public DateTime Timestamp { get; set; }
}