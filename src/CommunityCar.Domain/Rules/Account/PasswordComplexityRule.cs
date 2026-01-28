using CommunityCar.Domain.Rules;

namespace CommunityCar.Domain.Rules.Account;

public class PasswordComplexityRule : IBusinessRule
{
    private readonly string _password;

    public PasswordComplexityRule(string password)
    {
        _password = password;
    }

    public string Message => "Password must be at least 8 characters long and contain uppercase, lowercase, number, and special character.";

    public Task<bool> IsBrokenAsync()
    {
        if (string.IsNullOrEmpty(_password) || _password.Length < 8)
            return Task.FromResult(true);

        bool hasUpper = _password.Any(char.IsUpper);
        bool hasLower = _password.Any(char.IsLower);
        bool hasDigit = _password.Any(char.IsDigit);
        bool hasSpecial = _password.Any(c => !char.IsLetterOrDigit(c));

        return Task.FromResult(!(hasUpper && hasLower && hasDigit && hasSpecial));
    }
}