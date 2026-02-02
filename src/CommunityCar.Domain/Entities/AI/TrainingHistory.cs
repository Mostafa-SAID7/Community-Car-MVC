using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums.AI;

namespace CommunityCar.Domain.Entities.AI;

public class TrainingHistory : BaseEntity
{
    public Guid AIModelId { get; set; }
    public string Version { get; set; } = string.Empty;
    public DateTime TrainingDate { get; set; }
    public TimeSpan Duration { get; set; }
    public double InitialAccuracy { get; set; }
    public double FinalAccuracy { get; set; }
    public double Improvement { get; set; }
    public Guid? TrainingJobId { get; set; }
    public TrainingResult Result { get; set; }
    public bool IsSuccessful => Result == TrainingResult.Success;
    public string TrainingLog { get; set; } = string.Empty;
    public string Metrics { get; set; } = string.Empty; // JSON metrics
    public int Epochs { get; set; }
    public int BatchSize { get; set; }
    public double LearningRate { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public virtual AIModel AIModel { get; set; } = null!;
}