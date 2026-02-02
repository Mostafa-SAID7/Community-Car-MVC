using Microsoft.ML.Data;

namespace CommunityCar.AI.Models;

/// <summary>
/// Content recommendation prediction
/// </summary>
public class ContentRecommendationPrediction
{
    [ColumnName("Score")]
    public float Score { get; set; }

    public string ContentId { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public float RelevanceScore { get; set; }
}