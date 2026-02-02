using Microsoft.ML.Data;
using CommunityCar.Domain.Enums.AI;

namespace CommunityCar.AI.Models;

/// <summary>
/// Output prediction model for sentiment analysis
/// </summary>
public class SentimentPrediction
{
    [ColumnName("PredictedLabel")]
    public bool Prediction { get; set; }

    [ColumnName("Probability")]
    public float Probability { get; set; }

    [ColumnName("Score")]
    public float Score { get; set; }

    // Enhanced prediction details for interface compatibility
    public string Label { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public Dictionary<string, double> Emotions { get; set; } = new();

    // Legacy properties
    public SentimentType SentimentType => GetSentimentType();
    public string SentimentLabel => Prediction ? "Positive" : "Negative";
    public SentimentIntensity Intensity => GetIntensity();

    private SentimentType GetSentimentType()
    {
        if (Confidence < 0.6) return SentimentType.Neutral;
        return Prediction ? SentimentType.Positive : SentimentType.Negative;
    }

    private SentimentIntensity GetIntensity()
    {
        return Confidence switch
        {
            >= 0.9 => SentimentIntensity.VeryHigh,
            >= 0.8 => SentimentIntensity.High,
            >= 0.7 => SentimentIntensity.Medium,
            >= 0.6 => SentimentIntensity.Low,
            _ => SentimentIntensity.VeryLow
        };
    }
}