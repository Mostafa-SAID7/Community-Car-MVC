namespace CommunityCar.Domain.Enums.AI;

/// <summary>
/// Represents the current status of an AI model
/// </summary>
public enum AIModelStatus
{
    /// <summary>
    /// Model is in draft state and not yet ready for training
    /// </summary>
    Draft,

    /// <summary>
    /// Model is currently being trained
    /// </summary>
    Training,

    /// <summary>
    /// Model is active and ready for use
    /// </summary>
    Active,

    /// <summary>
    /// Model is inactive and not available for use
    /// </summary>
    Inactive,

    /// <summary>
    /// Model training or operation has failed
    /// </summary>
    Failed,

    /// <summary>
    /// Model has been deprecated and should not be used
    /// </summary>
    Deprecated
}