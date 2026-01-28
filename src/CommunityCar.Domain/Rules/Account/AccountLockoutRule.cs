using CommunityCar.Domain.Rules;

namespace CommunityCar.Domain.Rules.Account;

public class AccountLockoutRule : IBusinessRule
{
    private readonly int _failedAttempts;
    private readonly int _maxFailedAttempts;
    private readonly DateTime? _lastFailedAttempt;
    private readonly DateTime? _lockoutEndTime;
    private readonly TimeSpan _lockoutDuration;
    private readonly bool _isPermanentlyLocked;

    public string Message { get; private set; } = string.Empty;

    public AccountLockoutRule(
        int failedAttempts,
        int maxFailedAttempts,
        DateTime? lastFailedAttempt,
        DateTime? lockoutEndTime,
        TimeSpan lockoutDuration,
        bool isPermanentlyLocked)
    {
        _failedAttempts = failedAttempts;
        _maxFailedAttempts = maxFailedAttempts;
        _lastFailedAttempt = lastFailedAttempt;
        _lockoutEndTime = lockoutEndTime;
        _lockoutDuration = lockoutDuration;
        _isPermanentlyLocked = isPermanentlyLocked;
    }

    public Task<bool> IsBrokenAsync()
    {
        // Check if permanently locked
        if (_isPermanentlyLocked)
        {
            Message = "Account is permanently locked";
            return Task.FromResult(true);
        }

        // Check if currently in lockout period
        if (_lockoutEndTime.HasValue && DateTime.UtcNow < _lockoutEndTime.Value)
        {
            var remainingTime = _lockoutEndTime.Value - DateTime.UtcNow;
            Message = $"Account is locked for {remainingTime.TotalMinutes:F0} more minutes";
            return Task.FromResult(true);
        }

        // Check if should be locked due to failed attempts
        if (_failedAttempts >= _maxFailedAttempts)
        {
            Message = $"Account locked due to {_failedAttempts} failed login attempts";
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public bool ShouldLockAccount()
    {
        return _failedAttempts >= _maxFailedAttempts && !_isPermanentlyLocked;
    }

    public DateTime CalculateLockoutEndTime()
    {
        return DateTime.UtcNow.Add(_lockoutDuration);
    }

    public int GetRemainingAttempts()
    {
        return Math.Max(0, _maxFailedAttempts - _failedAttempts);
    }
}