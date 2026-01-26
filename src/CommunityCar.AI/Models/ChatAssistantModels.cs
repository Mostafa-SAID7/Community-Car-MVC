using Microsoft.ML.Data;

namespace CommunityCar.AI.Models;

/// <summary>
/// Chat message model
/// </summary>
public class ChatMessage
{
    public string Role { get; set; } = string.Empty; // "user", "assistant", "system"
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Chat context data for intelligent responses
/// </summary>
public class ChatContextData
{
    public string UserId { get; set; } = string.Empty;
    public string ConversationId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Intent { get; set; } = string.Empty;
    public List<string> Entities { get; set; } = new();
    public Dictionary<string, object> Context { get; set; } = new();
    public DateTime Timestamp { get; set; }
    public string Language { get; set; } = "en";
}

/// <summary>
/// Intent classification data
/// </summary>
public class IntentData
{
    [LoadColumn(0)]
    public string Text { get; set; } = string.Empty;

    [LoadColumn(1), ColumnName("Label")]
    public string Intent { get; set; } = string.Empty;
}

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

/// <summary>
/// Smart response generation data
/// </summary>
public class ResponseGenerationData
{
    public string UserMessage { get; set; } = string.Empty;
    public string Intent { get; set; } = string.Empty;
    public List<string> Entities { get; set; } = new();
    public SentimentPrediction Sentiment { get; set; } = new();
    public Dictionary<string, object> UserProfile { get; set; } = new();
    public List<string> ConversationHistory { get; set; } = new();
    public string PreferredLanguage { get; set; } = "en";
}

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

/// <summary>
/// User behavior prediction data
/// </summary>
public class UserBehaviorData
{
    public string UserId { get; set; } = string.Empty;
    public Dictionary<string, float> ActivityMetrics { get; set; } = new();
    public List<string> Interests { get; set; } = new();
    public Dictionary<string, int> InteractionCounts { get; set; } = new();
    public DateTime LastActivity { get; set; }
    public string UserSegment { get; set; } = string.Empty;
}

/// <summary>
/// Comprehensive AI assistant response
/// </summary>
public class AIAssistantResponse
{
    public string Response { get; set; } = string.Empty;
    public string Intent { get; set; } = string.Empty;
    public float IntentConfidence { get; set; }
    public SentimentPrediction Sentiment { get; set; } = new();
    public List<string> SuggestedActions { get; set; } = new();
    public List<ContentRecommendationPrediction> Recommendations { get; set; } = new();
    public Dictionary<string, object> Context { get; set; } = new();
    public bool RequiresHumanEscalation { get; set; }
    public List<string> DetectedEntities { get; set; } = new();
    public string Language { get; set; } = "en";
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Training data metrics
/// </summary>
public class TrainingMetrics
{
    public double Accuracy { get; set; }
    public double Precision { get; set; }
    public double Recall { get; set; }
    public double F1Score { get; set; }
    public double LogLoss { get; set; }
    public DateTime TrainingDate { get; set; }
    public int DatasetSize { get; set; }
    public string ModelVersion { get; set; } = string.Empty;
}

/// <summary>
/// Enhanced sentiment prediction with additional details
/// </summary>
public class EnhancedSentimentPrediction
{
    public string Label { get; set; } = string.Empty;
    public double Score { get; set; }
    public double Confidence { get; set; }
    public Dictionary<string, double> Emotions { get; set; } = new();
    public List<string> Keywords { get; set; } = new();
    public string Context { get; set; } = string.Empty;
}

/// <summary>
/// User behavior prediction data
/// </summary>
public class UserBehaviorPrediction
{
    public Guid UserId { get; set; }
    public double EngagementScore { get; set; }
    public double ChurnRisk { get; set; }
    public double ActivityLevel { get; set; }
    public List<string> PredictedInterests { get; set; } = new();
    public List<string> RecommendedContent { get; set; } = new();
    public DateTime PredictionDate { get; set; }
    public double Confidence { get; set; }
}

/// <summary>
/// User segment classification
/// </summary>
public class UserSegment
{
    public string SegmentId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Characteristics { get; set; } = new();
    public double Score { get; set; }
}

/// <summary>
/// Chat response model
/// </summary>
public class ChatResponse
{
    public string Message { get; set; } = string.Empty;
    public bool IsSuccessful { get; set; }
    public DateTime Timestamp { get; set; }
    public string Source { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}