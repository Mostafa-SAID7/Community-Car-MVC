using CommunityCar.Domain.Events.Account;
using CommunityCar.Domain.Enums.Account;
using CommunityCar.Domain.Policies;
using CommunityCar.Domain.Rules.Account;

namespace CommunityCar.Domain.Policies.Account.Security;

public class LockoutPolicy : IAccessPolicy<LockoutRequest>
{
    private readonly LockoutPolicySettings _settings;
    private readonly Func<Guid, Task<LockoutInfo>> _getLockoutInfoChecker;
    private readonly Func<Guid, Task<IEnumerable<string>>> _getUserRoles;

    public LockoutPolicy(
        LockoutPolicySettings settings = null,
        Func<Guid, Task<LockoutInfo>> getLockoutInfoChecker = null,
        Func<Guid, Task<IEnumerable<string>>> getUserRoles = null)
    {
        _settings = settings ?? LockoutPolicySettings.Default;
        _getLockoutInfoChecker = getLockoutInfoChecker ?? throw new ArgumentNullException(nameof(getLockoutInfoChecker));
        _getUserRoles = getUserRoles;
    }

    public bool CanAccess(Guid userId, LockoutRequest resource)
    {
        return CanAccessAsync(userId, resource).GetAwaiter().GetResult();
    }

    public async Task<bool> CanAccessAsync(Guid userId, LockoutRequest request)
    {
        var lockoutInfo = await _getLockoutInfoChecker(userId);
        var lockoutRule = new AccountLockoutRule(
            lockoutInfo.FailedAttempts,
            _settings.MaxFailedAttempts,
            lockoutInfo.LastFailedAttempt,
            lockoutInfo.LockoutEndTime,
            _settings.LockoutDuration,
            lockoutInfo.IsPermanentlyLocked);

        return !await lockoutRule.IsBrokenAsync();
    }

    /// <summary>
    /// Determines if an account should be locked based on failed attempts
    /// </summary>
    public async Task<LockoutDecision> EvaluateLockoutAsync(Guid userId, string ipAddress = null, string userAgent = null)
    {
        var lockoutInfo = await _getLockoutInfoChecker(userId);
        var decision = new LockoutDecision { UserId = userId };

        // Check current lockout status
        var lockoutRule = new AccountLockoutRule(
            lockoutInfo.FailedAttempts,
            _settings.MaxFailedAttempts,
            lockoutInfo.LastFailedAttempt,
            lockoutInfo.LockoutEndTime,
            _settings.LockoutDuration,
            lockoutInfo.IsPermanentlyLocked);

        if (await lockoutRule.IsBrokenAsync())
        {
            decision.ShouldLockout = true;
            decision.LockoutType = lockoutInfo.IsPermanentlyLocked ? LockoutType.Permanent : LockoutType.Temporary;
            decision.LockoutEndTime = lockoutInfo.LockoutEndTime;
            decision.Reason = lockoutRule.Message;
            return decision;
        }

        // Check if user should be exempt from lockout
        if (await IsExemptFromLockoutAsync(userId))
        {
            decision.ShouldLockout = false;
            decision.IsExempt = true;
            decision.Reason = "User is exempt from lockout policies";
            return decision;
        }

        // Evaluate progressive lockout
        if (lockoutInfo.FailedAttempts >= _settings.MaxFailedAttempts)
        {
            decision.ShouldLockout = true;
            decision.LockoutType = ShouldPermanentlyLock(lockoutInfo) ? LockoutType.Permanent : LockoutType.Temporary;
            decision.LockoutEndTime = CalculateLockoutEndTime(lockoutInfo);
            decision.Reason = $"Account locked due to {lockoutInfo.FailedAttempts} failed login attempts";
        }

        // Check for suspicious activity
        if (!string.IsNullOrEmpty(ipAddress) && !string.IsNullOrEmpty(userAgent))
        {
            var suspiciousRule = new SuspiciousLoginRule(
                ipAddress,
                userAgent,
                lockoutInfo.LastKnownIpAddress,
                lockoutInfo.LastKnownUserAgent,
                lockoutInfo.LastSuccessfulLogin,
                lockoutInfo.IsFromNewLocation,
                lockoutInfo.IsFromNewDevice);

            if (await suspiciousRule.IsBrokenAsync())
            {
                var riskLevel = suspiciousRule.GetRiskLevel();
                if (riskLevel >= LoginRiskLevel.High && _settings.LockoutOnSuspiciousActivity)
                {
                    decision.ShouldLockout = true;
                    decision.LockoutType = LockoutType.Security;
                    decision.LockoutEndTime = DateTime.UtcNow.Add(_settings.SecurityLockoutDuration);
                    decision.Reason = "Account locked due to suspicious login activity";
                }
            }
        }

        return decision;
    }

    /// <summary>
    /// Checks if user is exempt from lockout policies
    /// </summary>
    public async Task<bool> IsExemptFromLockoutAsync(Guid userId)
    {
        if (_getUserRoles == null)
            return false;

        var userRoles = await _getUserRoles(userId);
        var exemptRoles = _settings.ExemptRoles;

        return userRoles.Any(role => exemptRoles.Contains(role, StringComparer.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Calculates when the lockout should end
    /// </summary>
    public DateTime CalculateLockoutEndTime(LockoutInfo lockoutInfo)
    {
        var baseMultiplier = Math.Max(1, lockoutInfo.FailedAttempts - _settings.MaxFailedAttempts + 1);
        var multiplier = Math.Min(baseMultiplier, _settings.MaxLockoutMultiplier);
        
        var lockoutDuration = TimeSpan.FromMinutes(_settings.LockoutDuration.TotalMinutes * multiplier);
        return DateTime.UtcNow.Add(lockoutDuration);
    }

    /// <summary>
    /// Determines if account should be permanently locked
    /// </summary>
    public bool ShouldPermanentlyLock(LockoutInfo lockoutInfo)
    {
        return lockoutInfo.FailedAttempts >= _settings.PermanentLockoutThreshold ||
               lockoutInfo.LockoutCount >= _settings.MaxLockoutCount;
    }

    /// <summary>
    /// Unlocks an account if conditions are met
    /// </summary>
    public async Task<UnlockResult> UnlockAccountAsync(Guid userId, Guid adminUserId, string reason)
    {
        var lockoutInfo = await _getLockoutInfoChecker(userId);
        var result = new UnlockResult { UserId = userId };

        if (!lockoutInfo.IsLocked)
        {
            result.Success = false;
            result.Message = "Account is not currently locked";
            return result;
        }

        if (lockoutInfo.IsPermanentlyLocked && !await CanUnlockPermanentLockoutAsync(adminUserId))
        {
            result.Success = false;
            result.Message = "Insufficient privileges to unlock permanently locked account";
            return result;
        }

        result.Success = true;
        result.Message = "Account unlocked successfully";
        result.UnlockedBy = adminUserId;
        result.UnlockReason = reason;
        result.UnlockTime = DateTime.UtcNow;

        return result;
    }

    private async Task<bool> CanUnlockPermanentLockoutAsync(Guid adminUserId)
    {
        if (_getUserRoles == null)
            return false;

        var adminRoles = await _getUserRoles(adminUserId);
        var requiredRoles = new[] { "SuperAdmin", "SystemAdmin" };

        return adminRoles.Any(role => requiredRoles.Contains(role, StringComparer.OrdinalIgnoreCase));
    }
}

/// <summary>
/// Lockout policy configuration settings
/// </summary>
public class LockoutPolicySettings
{
    public int MaxFailedAttempts { get; set; } = 5;
    public TimeSpan LockoutDuration { get; set; } = TimeSpan.FromMinutes(30);
    public int MaxLockoutMultiplier { get; set; } = 10;
    public int PermanentLockoutThreshold { get; set; } = 20;
    public int MaxLockoutCount { get; set; } = 5;
    public bool LockoutOnSuspiciousActivity { get; set; } = true;
    public TimeSpan SecurityLockoutDuration { get; set; } = TimeSpan.FromHours(24);
    public string[] ExemptRoles { get; set; } = { "SystemAdmin" };
    public bool EnableProgressiveLockout { get; set; } = true;
    public TimeSpan LockoutResetPeriod { get; set; } = TimeSpan.FromDays(30);

    public static LockoutPolicySettings Default => new LockoutPolicySettings();

    public static LockoutPolicySettings Strict => new LockoutPolicySettings
    {
        MaxFailedAttempts = 3,
        LockoutDuration = TimeSpan.FromHours(1),
        MaxLockoutMultiplier = 5,
        PermanentLockoutThreshold = 10,
        MaxLockoutCount = 3,
        LockoutOnSuspiciousActivity = true,
        SecurityLockoutDuration = TimeSpan.FromHours(48),
        ExemptRoles = Array.Empty<string>(),
        EnableProgressiveLockout = true,
        LockoutResetPeriod = TimeSpan.FromDays(7)
    };

    public static LockoutPolicySettings Relaxed => new LockoutPolicySettings
    {
        MaxFailedAttempts = 10,
        LockoutDuration = TimeSpan.FromMinutes(15),
        MaxLockoutMultiplier = 3,
        PermanentLockoutThreshold = 50,
        MaxLockoutCount = 10,
        LockoutOnSuspiciousActivity = false,
        SecurityLockoutDuration = TimeSpan.FromHours(6),
        ExemptRoles = new[] { "SystemAdmin", "SuperAdmin", "Admin" },
        EnableProgressiveLockout = false,
        LockoutResetPeriod = TimeSpan.FromDays(90)
    };
}

public class LockoutRequest
{
    public Guid UserId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
}

public class LockoutInfo
{
    public Guid UserId { get; set; }
    public int FailedAttempts { get; set; }
    public DateTime? LastFailedAttempt { get; set; }
    public DateTime? LockoutEndTime { get; set; }
    public bool IsPermanentlyLocked { get; set; }
    public bool IsLocked => IsPermanentlyLocked || (LockoutEndTime.HasValue && DateTime.UtcNow < LockoutEndTime.Value);
    public int LockoutCount { get; set; }
    public DateTime? LastSuccessfulLogin { get; set; }
    public string LastKnownIpAddress { get; set; } = string.Empty;
    public string LastKnownUserAgent { get; set; } = string.Empty;
    public bool IsFromNewLocation { get; set; }
    public bool IsFromNewDevice { get; set; }
}

public class LockoutDecision
{
    public Guid UserId { get; set; }
    public bool ShouldLockout { get; set; }
    public LockoutType LockoutType { get; set; }
    public DateTime? LockoutEndTime { get; set; }
    public string Reason { get; set; } = string.Empty;
    public bool IsExempt { get; set; }
}

public class UnlockResult
{
    public Guid UserId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? UnlockedBy { get; set; }
    public string UnlockReason { get; set; } = string.Empty;
    public DateTime? UnlockTime { get; set; }
}