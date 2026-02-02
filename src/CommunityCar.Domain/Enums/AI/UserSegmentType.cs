namespace CommunityCar.Domain.Enums.AI;

/// <summary>
/// Represents different user segments for AI predictions and analytics
/// </summary>
public enum UserSegmentType
{
    /// <summary>
    /// New user who recently joined the platform
    /// </summary>
    NewUser,

    /// <summary>
    /// Active user who regularly engages with the platform
    /// </summary>
    ActiveUser,

    /// <summary>
    /// Power user who is highly engaged and influential
    /// </summary>
    PowerUser,

    /// <summary>
    /// User at risk of churning or becoming inactive
    /// </summary>
    AtRiskUser,

    /// <summary>
    /// User who has become inactive or left the platform
    /// </summary>
    ChurnedUser,

    /// <summary>
    /// User who exhibits toxic behavior patterns
    /// </summary>
    ToxicUser
}