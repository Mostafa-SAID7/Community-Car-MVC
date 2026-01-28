using CommunityCar.Domain.Policies;
using CommunityCar.Domain.Policies.Account.Core;

namespace CommunityCar.Domain.Policies.Account.Authentication;

/// <summary>
/// Policy that requires Multi-Factor Authentication for access
/// </summary>
public class RequireMfaPolicy : IAccessPolicy<MfaRequiredResource>
{
    private readonly Func<Guid, Task<bool>> _isMfaEnabledChecker;
    private readonly Func<Guid, Task<bool>> _isMfaVerifiedChecker;
    private readonly Func<Guid, Task<DateTime?>> _getLastMfaVerificationChecker;
    private readonly TimeSpan _mfaSessionDuration;

    public RequireMfaPolicy(
        Func<Guid, Task<bool>> isMfaEnabledChecker,
        Func<Guid, Task<bool>> isMfaVerifiedChecker,
        Func<Guid, Task<DateTime?>> getLastMfaVerificationChecker,
        TimeSpan? mfaSessionDuration = null)
    {
        _isMfaEnabledChecker = isMfaEnabledChecker ?? throw new ArgumentNullException(nameof(isMfaEnabledChecker));
        _isMfaVerifiedChecker = isMfaVerifiedChecker ?? throw new ArgumentNullException(nameof(isMfaVerifiedChecker));
        _getLastMfaVerificationChecker = getLastMfaVerificationChecker ?? throw new ArgumentNullException(nameof(getLastMfaVerificationChecker));
        _mfaSessionDuration = mfaSessionDuration ?? TimeSpan.FromHours(8); // Default 8 hours
    }

    public bool CanAccess(Guid userId, MfaRequiredResource resource)
    {
        return CanAccessAsync(userId, resource).GetAwaiter().GetResult();
    }

    public async Task<bool> CanAccessAsync(Guid userId, MfaRequiredResource resource)
    {
        if (userId == Guid.Empty)
            return false;

        // Check if MFA is enabled for the user
        var isMfaEnabled = await _isMfaEnabledChecker(userId);
        if (!isMfaEnabled)
        {
            // If MFA is required for this resource but not enabled, deny access
            return !resource.RequiresMfa;
        }

        // Check if MFA is currently verified
        var isMfaVerified = await _isMfaVerifiedChecker(userId);
        if (!isMfaVerified)
            return false;

        // Check if MFA verification is still valid (within session duration)
        var lastMfaVerification = await _getLastMfaVerificationChecker(userId);
        if (!lastMfaVerification.HasValue)
            return false;

        var timeSinceVerification = DateTime.UtcNow - lastMfaVerification.Value;
        var isSessionValid = timeSinceVerification <= _mfaSessionDuration;

        // For high-security resources, require fresh MFA verification
        if (resource.RequiresFreshMfa)
        {
            var freshMfaWindow = TimeSpan.FromMinutes(5); // Fresh MFA within 5 minutes
            return timeSinceVerification <= freshMfaWindow;
        }

        return isSessionValid;
    }

    /// <summary>
    /// Checks if MFA setup is required for the user
    /// </summary>
    public async Task<bool> RequiresMfaSetupAsync(Guid userId, MfaRequirement requirement)
    {
        var isMfaEnabled = await _isMfaEnabledChecker(userId);
        
        return requirement switch
        {
            MfaRequirement.Optional => false,
            MfaRequirement.Recommended => !isMfaEnabled,
            MfaRequirement.Required => !isMfaEnabled,
            MfaRequirement.Enforced => !isMfaEnabled,
            _ => false
        };
    }

    /// <summary>
    /// Gets the remaining MFA session time
    /// </summary>
    public async Task<TimeSpan?> GetRemainingMfaSessionAsync(Guid userId)
    {
        var lastMfaVerification = await _getLastMfaVerificationChecker(userId);
        if (!lastMfaVerification.HasValue)
            return null;

        var timeSinceVerification = DateTime.UtcNow - lastMfaVerification.Value;
        var remainingTime = _mfaSessionDuration - timeSinceVerification;

        return remainingTime > TimeSpan.Zero ? remainingTime : null;
    }
}