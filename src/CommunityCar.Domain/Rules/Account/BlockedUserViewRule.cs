using CommunityCar.Domain.Rules;

namespace CommunityCar.Domain.Rules.Account;

public class BlockedUserViewRule : IBusinessRule
{
    private readonly bool _isBlocked;

    public BlockedUserViewRule(bool isBlocked)
    {
        _isBlocked = isBlocked;
    }

    public string Message => "Blocked users cannot view this profile.";

    public Task<bool> IsBrokenAsync()
    {
        return Task.FromResult(_isBlocked);
    }
}