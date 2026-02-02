namespace CommunityCar.AI.Models;

/// <summary>
/// Insights generated from chat analysis
/// </summary>
public class ChatInsight
{
    /// <summary>
    /// Type of insight (e.g., "Sentiment Trend", "Topic Analysis")
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable description of the insight
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Confidence level of the insight (0.0 to 1.0)
    /// </summary>
    public float Confidence { get; set; }

    /// <summary>
    /// When the insight was generated
    /// </summary>
    public DateTime Timestamp { get; set; }
}