using CommunityCar.Domain.Rules;

namespace CommunityCar.Domain.Rules.Account;

public class UniqueEmailRule : IBusinessRule
{
    private readonly string _email;
    private readonly Func<string, Task<bool>> _emailExistsChecker;
    private readonly Guid? _excludeUserId;

    public UniqueEmailRule(string email, Func<string, Task<bool>> emailExistsChecker, Guid? excludeUserId = null)
    {
        _email = email?.Trim().ToLowerInvariant() ?? string.Empty;
        _emailExistsChecker = emailExistsChecker ?? throw new ArgumentNullException(nameof(emailExistsChecker));
        _excludeUserId = excludeUserId;
    }

    public string Message => "Email address is already in use by another account.";

    public async Task<bool> IsBrokenAsync()
    {
        if (string.IsNullOrWhiteSpace(_email))
            return true; // Empty email is considered broken

        // Check if email already exists (excluding current user if specified)
        var emailExists = await _emailExistsChecker(_email);
        
        // If we're updating an existing user, we need additional logic
        if (_excludeUserId.HasValue && emailExists)
        {
            // This would require additional repository method to check if email belongs to excluded user
            // For now, we'll assume the emailExistsChecker handles exclusion logic
            return emailExists;
        }

        return emailExists;
    }
}