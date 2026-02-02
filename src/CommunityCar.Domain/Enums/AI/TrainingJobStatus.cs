namespace CommunityCar.Domain.Enums.AI;

/// <summary>
/// Represents the current status of a training job
/// </summary>
public enum TrainingJobStatus
{
    /// <summary>
    /// Training job is queued and waiting to start
    /// </summary>
    Queued,

    /// <summary>
    /// Training job is currently in progress
    /// </summary>
    InProgress,

    /// <summary>
    /// Training job has completed successfully
    /// </summary>
    Completed,

    /// <summary>
    /// Training job has failed due to an error
    /// </summary>
    Failed,

    /// <summary>
    /// Training job was cancelled by user or system
    /// </summary>
    Cancelled,

    /// <summary>
    /// Training job has been paused and can be resumed
    /// </summary>
    Paused
}