using CommunityCar.Domain.Rules;

namespace CommunityCar.Domain.Rules.Account;

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