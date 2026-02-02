namespace CommunityCar.Domain.Enums.AI;

/// <summary>
/// Represents the result of a training operation
/// </summary>
public enum TrainingResult
{
    /// <summary>
    /// Training completed successfully
    /// </summary>
    Success,

    /// <summary>
    /// Training failed due to an error
    /// </summary>
    Failed,

    /// <summary>
    /// Training was cancelled before completion
    /// </summary>
    Cancelled,

    /// <summary>
    /// Training exceeded the maximum allowed time
    /// </summary>
    Timeout,

    /// <summary>
    /// Training failed due to insufficient training data
    /// </summary>
    InsufficientData
}