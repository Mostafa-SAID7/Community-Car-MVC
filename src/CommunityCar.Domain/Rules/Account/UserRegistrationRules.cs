namespace CommunityCar.Domain.Rules.Account;

/// <summary>
/// Business rules for user registration
/// </summary>
public static class UserRegistrationRules
{
    /// <summary>
    /// Rule: Email must be unique
    /// </summary>
    public class EmailMustBeUniqueRule : IBusinessRule
    {
        private readonly string _email;
        private readonly Func<string, Task<bool>> _emailExistsCheck;

        public EmailMustBeUniqueRule(string email, Func<string, Task<bool>> emailExistsCheck)
        {
            _email = email;
            _emailExistsCheck = emailExistsCheck;
        }

        public string Message => "Email address is already registered.";

        public async Task<bool> IsBrokenAsync()
        {
            return await _emailExistsCheck(_email);
        }
    }

    /// <summary>
    /// Rule: Username must be unique
    /// </summary>
    public class UsernameMustBeUniqueRule : IBusinessRule
    {
        private readonly string _username;
        private readonly Func<string, Task<bool>> _usernameExistsCheck;

        public UsernameMustBeUniqueRule(string username, Func<string, Task<bool>> usernameExistsCheck)
        {
            _username = username;
            _usernameExistsCheck = usernameExistsCheck;
        }

        public string Message => "Username is already taken.";

        public async Task<bool> IsBrokenAsync()
        {
            return await _usernameExistsCheck(_username);
        }
    }

    /// <summary>
    /// Rule: Password must meet complexity requirements
    /// </summary>
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

    /// <summary>
    /// Rule: User must be at least 13 years old
    /// </summary>
    public class MinimumAgeRule : IBusinessRule
    {
        private readonly DateTime? _dateOfBirth;

        public MinimumAgeRule(DateTime? dateOfBirth)
        {
            _dateOfBirth = dateOfBirth;
        }

        public string Message => "User must be at least 13 years old to register.";

        public Task<bool> IsBrokenAsync()
        {
            if (!_dateOfBirth.HasValue)
                return Task.FromResult(false); // Optional field

            var age = DateTime.Today.Year - _dateOfBirth.Value.Year;
            if (_dateOfBirth.Value.Date > DateTime.Today.AddYears(-age))
                age--;

            return Task.FromResult(age < 13);
        }
    }
}