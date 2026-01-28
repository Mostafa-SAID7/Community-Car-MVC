using CommunityCar.Domain.Policies;
using CommunityCar.Domain.Policies.Account.Core;
using CommunityCar.Domain.Rules.Account;

namespace CommunityCar.Domain.Policies.Account.Security;

/// <summary>
/// Policy for password requirements and validation
/// </summary>
public class PasswordPolicy : IAccessPolicy<PasswordRequest>
{
    private readonly PasswordPolicySettings _settings;
    private readonly Func<Guid, Task<IEnumerable<string>>> _getPasswordHistoryChecker;
    private readonly Func<string, Task<bool>> _isCommonPasswordChecker;

    public PasswordPolicy(
        PasswordPolicySettings settings = null,
        Func<Guid, Task<IEnumerable<string>>> getPasswordHistoryChecker = null,
        Func<string, Task<bool>> isCommonPasswordChecker = null)
    {
        _settings = settings ?? PasswordPolicySettings.Default;
        _getPasswordHistoryChecker = getPasswordHistoryChecker;
        _isCommonPasswordChecker = isCommonPasswordChecker;
    }

    public bool CanAccess(Guid userId, PasswordRequest resource)
    {
        return CanAccessAsync(userId, resource).GetAwaiter().GetResult();
    }

    public async Task<bool> CanAccessAsync(Guid userId, PasswordRequest request)
    {
        var validationResult = await ValidatePasswordAsync(userId, request.NewPassword, request.CurrentPassword);
        return validationResult.IsValid;
    }

    /// <summary>
    /// Validates a password against all policy rules
    /// </summary>
    public async Task<PasswordValidationResult> ValidatePasswordAsync(Guid userId, string newPassword, string currentPassword = null)
    {
        var result = new PasswordValidationResult();
        var errors = new List<string>();

        // Basic strength validation
        var strengthRule = new PasswordStrengthRule(
            newPassword,
            _settings.MinLength,
            _settings.RequireUppercase,
            _settings.RequireLowercase,
            _settings.RequireDigit,
            _settings.RequireSpecialChar);

        if (await strengthRule.IsBrokenAsync())
        {
            errors.Add(strengthRule.Message);
        }

        // Check password history
        if (_getPasswordHistoryChecker != null && _settings.PreventPasswordReuse > 0)
        {
            var passwordHistory = await _getPasswordHistoryChecker(userId);
            if (passwordHistory.Take(_settings.PreventPasswordReuse).Contains(HashPassword(newPassword)))
            {
                errors.Add($"Cannot reuse any of the last {_settings.PreventPasswordReuse} passwords.");
            }
        }

        // Check against common passwords
        if (_isCommonPasswordChecker != null && await _isCommonPasswordChecker(newPassword))
        {
            errors.Add("Password is too common. Please choose a more unique password.");
        }

        // Check similarity to current password
        if (!string.IsNullOrEmpty(currentPassword) && _settings.PreventSimilarPasswords)
        {
            if (AreSimilarPasswords(currentPassword, newPassword))
            {
                errors.Add("New password is too similar to the current password.");
            }
        }

        // Check for personal information (if user data is available)
        if (_settings.PreventPersonalInfo)
        {
            // This would require user data to check against
            // Implementation would check against username, email, name, etc.
        }

        // Check password age requirements
        if (_settings.MinPasswordAge > TimeSpan.Zero && !string.IsNullOrEmpty(currentPassword))
        {
            // This would require last password change date
            // For now, we'll skip this check
        }

        result.IsValid = !errors.Any();
        result.Errors = errors;
        result.StrengthScore = strengthRule.CalculateStrengthScore();
        result.StrengthLevel = strengthRule.GetStrengthLevel();

        return result;
    }

    /// <summary>
    /// Checks if password change is required
    /// </summary>
    public async Task<bool> IsPasswordChangeRequiredAsync(Guid userId, DateTime? lastPasswordChange)
    {
        if (!lastPasswordChange.HasValue)
            return true; // Force change if no history

        if (_settings.MaxPasswordAge == TimeSpan.Zero)
            return false; // No expiration

        var passwordAge = DateTime.UtcNow - lastPasswordChange.Value;
        return passwordAge > _settings.MaxPasswordAge;
    }

    /// <summary>
    /// Gets password expiration warning
    /// </summary>
    public async Task<PasswordExpirationInfo> GetPasswordExpirationInfoAsync(Guid userId, DateTime? lastPasswordChange)
    {
        if (!lastPasswordChange.HasValue || _settings.MaxPasswordAge == TimeSpan.Zero)
        {
            return new PasswordExpirationInfo { IsExpiring = false };
        }

        var passwordAge = DateTime.UtcNow - lastPasswordChange.Value;
        var timeUntilExpiration = _settings.MaxPasswordAge - passwordAge;

        var isExpiring = timeUntilExpiration <= _settings.ExpirationWarningPeriod;
        var isExpired = timeUntilExpiration <= TimeSpan.Zero;

        return new PasswordExpirationInfo
        {
            IsExpiring = isExpiring,
            IsExpired = isExpired,
            TimeUntilExpiration = isExpired ? TimeSpan.Zero : timeUntilExpiration,
            ExpirationDate = lastPasswordChange.Value.Add(_settings.MaxPasswordAge)
        };
    }

    private bool AreSimilarPasswords(string oldPassword, string newPassword)
    {
        if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
            return false;

        // Simple similarity check - can be enhanced with more sophisticated algorithms
        var similarity = CalculateLevenshteinDistance(oldPassword.ToLower(), newPassword.ToLower());
        var maxLength = Math.Max(oldPassword.Length, newPassword.Length);
        var similarityPercentage = 1.0 - (double)similarity / maxLength;

        return similarityPercentage > 0.7; // 70% similarity threshold
    }

    private int CalculateLevenshteinDistance(string source, string target)
    {
        if (string.IsNullOrEmpty(source)) return target?.Length ?? 0;
        if (string.IsNullOrEmpty(target)) return source.Length;

        var distance = new int[source.Length + 1, target.Length + 1];

        for (int i = 0; i <= source.Length; i++)
            distance[i, 0] = i;
        for (int j = 0; j <= target.Length; j++)
            distance[0, j] = j;

        for (int i = 1; i <= source.Length; i++)
        {
            for (int j = 1; j <= target.Length; j++)
            {
                var cost = source[i - 1] == target[j - 1] ? 0 : 1;
                distance[i, j] = Math.Min(
                    Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
                    distance[i - 1, j - 1] + cost);
            }
        }

        return distance[source.Length, target.Length];
    }

    private string HashPassword(string password)
    {
        // This should use the same hashing algorithm as your authentication system
        // For now, returning a simple hash - replace with actual implementation
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
    }
}

/// <summary>
/// Password policy configuration settings
/// </summary>
public class PasswordPolicySettings
{
    public int MinLength { get; set; } = 8;
    public int MaxLength { get; set; } = 128;
    public bool RequireUppercase { get; set; } = true;
    public bool RequireLowercase { get; set; } = true;
    public bool RequireDigit { get; set; } = true;
    public bool RequireSpecialChar { get; set; } = true;
    public int PreventPasswordReuse { get; set; } = 5; // Last 5 passwords
    public bool PreventSimilarPasswords { get; set; } = true;
    public bool PreventPersonalInfo { get; set; } = true;
    public TimeSpan MinPasswordAge { get; set; } = TimeSpan.FromDays(1);
    public TimeSpan MaxPasswordAge { get; set; } = TimeSpan.FromDays(90);
    public TimeSpan ExpirationWarningPeriod { get; set; } = TimeSpan.FromDays(14);

    public static PasswordPolicySettings Default => new PasswordPolicySettings();

    public static PasswordPolicySettings Strict => new PasswordPolicySettings
    {
        MinLength = 12,
        RequireUppercase = true,
        RequireLowercase = true,
        RequireDigit = true,
        RequireSpecialChar = true,
        PreventPasswordReuse = 10,
        PreventSimilarPasswords = true,
        PreventPersonalInfo = true,
        MinPasswordAge = TimeSpan.FromDays(1),
        MaxPasswordAge = TimeSpan.FromDays(60),
        ExpirationWarningPeriod = TimeSpan.FromDays(7)
    };

    public static PasswordPolicySettings Relaxed => new PasswordPolicySettings
    {
        MinLength = 6,
        RequireUppercase = false,
        RequireLowercase = true,
        RequireDigit = true,
        RequireSpecialChar = false,
        PreventPasswordReuse = 3,
        PreventSimilarPasswords = false,
        PreventPersonalInfo = false,
        MinPasswordAge = TimeSpan.Zero,
        MaxPasswordAge = TimeSpan.Zero, // No expiration
        ExpirationWarningPeriod = TimeSpan.Zero
    };
}

public class PasswordRequest
{
    public string NewPassword { get; set; } = string.Empty;
    public string CurrentPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class PasswordValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public int StrengthScore { get; set; }
    public PasswordStrength StrengthLevel { get; set; }
}

public class PasswordExpirationInfo
{
    public bool IsExpiring { get; set; }
    public bool IsExpired { get; set; }
    public TimeSpan TimeUntilExpiration { get; set; }
    public DateTime ExpirationDate { get; set; }
}