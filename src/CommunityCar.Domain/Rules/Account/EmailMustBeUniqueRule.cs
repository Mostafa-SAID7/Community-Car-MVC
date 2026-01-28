using CommunityCar.Domain.Rules;

namespace CommunityCar.Domain.Rules.Account;

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