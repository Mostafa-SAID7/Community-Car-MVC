using CommunityCar.Domain.Rules;
using CommunityCar.Domain.Policies.Account.Core;

namespace CommunityCar.Domain.Rules.Account;

public class PasswordStrengthRule : IBusinessRule
{
    private readonly string _password;
    private readonly int _minLength;
    private readonly bool _requireUppercase;
    private readonly bool _requireLowercase;
    private readonly bool _requireDigit;
    private readonly bool _requireSpecialChar;

    public string Message { get; private set; } = string.Empty;

    public PasswordStrengthRule(
        string password,
        int minLength,
        bool requireUppercase,
        bool requireLowercase,
        bool requireDigit,
        bool requireSpecialChar)
    {
        _password = password ?? string.Empty;
        _minLength = minLength;
        _requireUppercase = requireUppercase;
        _requireLowercase = requireLowercase;
        _requireDigit = requireDigit;
        _requireSpecialChar = requireSpecialChar;
    }

    public Task<bool> IsBrokenAsync()
    {
        var errors = new List<string>();

        if (_password.Length < _minLength)
            errors.Add($"Password must be at least {_minLength} characters long");

        if (_requireUppercase && !_password.Any(char.IsUpper))
            errors.Add("Password must contain at least one uppercase letter");

        if (_requireLowercase && !_password.Any(char.IsLower))
            errors.Add("Password must contain at least one lowercase letter");

        if (_requireDigit && !_password.Any(char.IsDigit))
            errors.Add("Password must contain at least one digit");

        if (_requireSpecialChar && !_password.Any(c => !char.IsLetterOrDigit(c)))
            errors.Add("Password must contain at least one special character");

        Message = string.Join("; ", errors);
        return Task.FromResult(errors.Any());
    }

    public int CalculateStrengthScore()
    {
        int score = 0;

        // Length scoring
        if (_password.Length >= 8) score += 1;
        if (_password.Length >= 12) score += 1;
        if (_password.Length >= 16) score += 1;

        // Character variety scoring
        if (_password.Any(char.IsUpper)) score += 1;
        if (_password.Any(char.IsLower)) score += 1;
        if (_password.Any(char.IsDigit)) score += 1;
        if (_password.Any(c => !char.IsLetterOrDigit(c))) score += 1;

        // Pattern scoring
        if (!HasRepeatingCharacters()) score += 1;
        if (!HasSequentialCharacters()) score += 1;

        return Math.Min(score, 10); // Cap at 10
    }

    public PasswordStrength GetStrengthLevel()
    {
        var score = CalculateStrengthScore();
        return score switch
        {
            <= 2 => PasswordStrength.VeryWeak,
            <= 4 => PasswordStrength.Weak,
            <= 6 => PasswordStrength.Fair,
            <= 7 => PasswordStrength.Good,
            <= 8 => PasswordStrength.Strong,
            _ => PasswordStrength.VeryStrong
        };
    }

    private bool HasRepeatingCharacters()
    {
        for (int i = 0; i < _password.Length - 2; i++)
        {
            if (_password[i] == _password[i + 1] && _password[i + 1] == _password[i + 2])
                return true;
        }
        return false;
    }

    private bool HasSequentialCharacters()
    {
        for (int i = 0; i < _password.Length - 2; i++)
        {
            if (_password[i] + 1 == _password[i + 1] && _password[i + 1] + 1 == _password[i + 2])
                return true;
        }
        return false;
    }
}