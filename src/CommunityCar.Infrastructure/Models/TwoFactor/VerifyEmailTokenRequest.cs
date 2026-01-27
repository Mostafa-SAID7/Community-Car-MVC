namespace CommunityCar.Infrastructure.Models.TwoFactor;

/// <summary>
/// Request model for verifying email-based two-factor authentication token
/// </summary>
public class VerifyEmailTokenRequest
{
    /// <summary>
    /// User ID requesting verification
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Email token to verify
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Whether to remember this device
    /// </summary>
    public bool RememberDevice { get; set; }
}
