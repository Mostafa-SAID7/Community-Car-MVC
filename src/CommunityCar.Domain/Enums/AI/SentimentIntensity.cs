namespace CommunityCar.Domain.Enums.AI;

/// <summary>
/// Represents the intensity level of detected sentiment
/// </summary>
public enum SentimentIntensity
{
    /// <summary>
    /// Very low intensity - minimal sentiment detected
    /// </summary>
    VeryLow,

    /// <summary>
    /// Low intensity - weak sentiment detected
    /// </summary>
    Low,

    /// <summary>
    /// Medium intensity - moderate sentiment detected
    /// </summary>
    Medium,

    /// <summary>
    /// High intensity - strong sentiment detected
    /// </summary>
    High,

    /// <summary>
    /// Very high intensity - extremely strong sentiment detected
    /// </summary>
    VeryHigh
}