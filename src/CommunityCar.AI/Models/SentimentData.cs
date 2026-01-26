using Microsoft.ML.Data;

namespace CommunityCar.AI.Models;

/// <summary>
/// Input data model for sentiment analysis
/// </summary>
public class SentimentData
{
    [LoadColumn(0)]
    public string Text { get; set; } = string.Empty;

    [LoadColumn(1), ColumnName("Label")]
    public string Sentiment { get; set; } = string.Empty;

    // Additional context fields for community car analysis
    public string? UserId { get; set; }
    public string? ContentType { get; set; } // Post, Comment, Review, etc.
    public DateTime Timestamp { get; set; }
    public string? Category { get; set; } // Car-related category
    public float? Rating { get; set; } // If applicable (1-5 stars)
}

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

/// <summary>
/// Car-specific context analysis
/// </summary>
public class CarRelatedContext
{
    public List<string> CarBrands { get; set; } = new();
    public List<string> CarModels { get; set; } = new();
    public List<string> CarFeatures { get; set; } = new();
    public List<string> Issues { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    public float RelevanceScore { get; set; }
}

public enum SentimentType
{
    Positive,
    Negative,
    Neutral
}

public enum SentimentIntensity
{
    VeryLow,
    Low,
    Medium,
    High,
    VeryHigh
}