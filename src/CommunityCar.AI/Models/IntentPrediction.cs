using Microsoft.ML.Data;

namespace CommunityCar.AI.Models;

/// <summary>
/// Intent prediction result
/// </summary>
public class IntentPrediction
{
    [ColumnName("PredictedLabel")]
    public string Intent { get; set; } = string.Empty;

    [ColumnName("Score")]
    public float[] Score { get; set; } = Array.Empty<float>();

    public float Confidence => Score.Max();
    public Dictionary<string, float> IntentScores { get; set; } = new();
}