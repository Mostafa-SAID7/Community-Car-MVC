namespace CommunityCar.AI.Models;

/// <summary>
/// Enhanced sentiment analysis result with context
/// </summary>
public class EnhancedSentimentResult
{
    public SentimentPrediction Prediction { get; set; } = new();
    public List<string> KeyPhrases { get; set; } = new();
    public Dictionary<string, float> EmotionScores { get; set; } = new();
    public List<string> Topics { get; set; } = new();
    public float ToxicityScore { get; set; }
    public bool IsSpam { get; set; }
    public CarRelatedContext? CarContext { get; set; }
}