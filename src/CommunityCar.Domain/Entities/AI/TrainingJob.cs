using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.AI;

public class TrainingJob : BaseEntity
{
    public Guid AIModelId { get; set; }
    public string JobName { get; set; } = string.Empty;
    public TrainingJobStatus Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public TimeSpan? EstimatedDuration { get; set; }
    public TimeSpan? ActualDuration { get; set; }
    public string Parameters { get; set; } = string.Empty; // JSON parameters
    public string? ErrorMessage { get; set; }
    public double? ResultAccuracy { get; set; }
    public string? ResultMetrics { get; set; } // JSON metrics
    public int Priority { get; set; } = 0;
    
    // Navigation properties
    public virtual AIModel AIModel { get; set; } = null!;
}

public enum TrainingJobStatus
{
    Queued,
    InProgress,
    Completed,
    Failed,
    Cancelled,
    Paused
}