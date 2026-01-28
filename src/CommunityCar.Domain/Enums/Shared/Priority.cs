namespace CommunityCar.Domain.Enums.Shared;

/// <summary>
/// Represents priority levels used across different domains
/// </summary>
public enum Priority
{
    /// <summary>
    /// Lowest priority
    /// </summary>
    Low = 1,
    
    /// <summary>
    /// Normal priority (default)
    /// </summary>
    Normal = 2,
    
    /// <summary>
    /// High priority
    /// </summary>
    High = 3,
    
    /// <summary>
    /// Critical priority (highest)
    /// </summary>
    Critical = 4,
    
    /// <summary>
    /// Urgent priority (immediate attention required)
    /// </summary>
    Urgent = 5
}