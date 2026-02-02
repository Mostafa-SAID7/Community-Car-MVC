using Microsoft.ML.Data;
using CommunityCar.Domain.Enums.AI;

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