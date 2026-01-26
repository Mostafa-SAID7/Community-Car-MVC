using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.AI;

public class AIModel : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public AIModelType Type { get; set; }
    public AIModelStatus Status { get; set; }
    public double Accuracy { get; set; }
    public int DatasetSize { get; set; }
    public DateTime LastTrained { get; set; }
    public DateTime? LastUsed { get; set; }
    public string Configuration { get; set; } = string.Empty; // JSON configuration
    public string ModelPath { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<TrainingJob> TrainingJobs { get; set; } = new List<TrainingJob>();
    public virtual ICollection<TrainingHistory> TrainingHistories { get; set; } = new List<TrainingHistory>();
}

public enum AIModelType
{
    IntentClassification,
    SentimentAnalysis,
    ResponseGeneration,
    TextSummarization,
    LanguageTranslation,
    ContentModeration
}

public enum AIModelStatus
{
    Draft,
    Training,
    Active,
    Inactive,
    Failed,
    Deprecated
}