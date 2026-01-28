namespace CommunityCar.Domain.Enums.Account;

/// <summary>
/// Represents the trust level of a user based on reputation and activity
/// </summary>
public enum UserTrustLevel
{
    /// <summary>
    /// Unverified user - just registered, no verification
    /// </summary>
    Unverified = 0,
    
    /// <summary>
    /// New user - verified but limited activity
    /// </summary>
    New = 1,
    
    /// <summary>
    /// Regular user - established presence with moderate activity
    /// </summary>
    Regular = 2,
    
    /// <summary>
    /// Established user - high reputation and consistent activity
    /// </summary>
    Established = 3,
    
    /// <summary>
    /// Trusted user - highest trust level with extensive positive history
    /// </summary>
    Trusted = 4
}