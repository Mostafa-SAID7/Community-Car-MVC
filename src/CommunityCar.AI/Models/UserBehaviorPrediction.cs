using CommunityCar.Domain.Enums.AI;

namespace CommunityCar.AI.Models;

/// <summary>
/// User behavior prediction result
/// </summary>
public class UserBehaviorPrediction
{
    public string UserId { get; set; } = string.Empty;
    public float EngagementScore { get; set; }
    public float ChurnProbability { get; set; }
    public float ToxicityRisk { get; set; }
    public UserSegmentType Segment { get; set; }
    public List<string> RecommendedActions { get; set; } = new();
    public double ActivityLevel { get; set; }
    public List<string> PredictedInterests { get; set; } = new();
    public List<string> RecommendedContent { get; set; } = new();
    public DateTime PredictionDate { get; set; }
    public double Confidence { get; set; }
}