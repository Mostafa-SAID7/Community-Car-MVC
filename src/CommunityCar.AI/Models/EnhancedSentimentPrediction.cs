namespace CommunityCar.AI.Models;

/// <summary>
/// Enhanced sentiment prediction with additional details
/// </summary>
public class EnhancedSentimentPrediction
{
    /// <summary>
    /// Sentiment label (Positive, Negative, Neutral)
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Sentiment score
    /// </summary>
    public double Score { get; set; }

    /// <summary>
    /// Confidence level of the prediction
    /// </summary>
    public double Confidence { get; set; }

    /// <summary>
    /// Emotion scores detected in the text
    /// </summary>
    public Dictionary<string, double> Emotions { get; set; } = new();

    /// <summary>
    /// Keywords that influenced the sentiment
    /// </summary>
    public List<string> Keywords { get; set; } = new();

    /// <summary>
    /// Context in which the sentiment was analyzed
    /// </summary>
    public string Context { get; set; } = string.Empty;
}