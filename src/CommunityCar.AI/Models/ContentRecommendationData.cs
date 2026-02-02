using Microsoft.ML.Data;

namespace CommunityCar.AI.Models;

/// <summary>
/// Content recommendation data
/// </summary>
public class ContentRecommendationData
{
    [LoadColumn(0)]
    public string UserId { get; set; } = string.Empty;

    [LoadColumn(1)]
    public string ContentId { get; set; } = string.Empty;

    [LoadColumn(2)]
    public string ContentType { get; set; } = string.Empty;

    [LoadColumn(3)]
    public float Rating { get; set; }

    [LoadColumn(4)]
    public string Features { get; set; } = string.Empty; // Serialized features
}