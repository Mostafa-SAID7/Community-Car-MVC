using CommunityCar.Domain.Rules;

namespace CommunityCar.Domain.Rules.Account;

public class CannotViewOwnProfileRule : IBusinessRule
{
    private readonly Guid _viewerId;
    private readonly Guid _profileUserId;

    public CannotViewOwnProfileRule(Guid viewerId, Guid profileUserId)
    {
        _viewerId = viewerId;
        _profileUserId = profileUserId;
    }

    public string Message => "Users cannot view their own profile for tracking purposes.";

    public Task<bool> IsBrokenAsync()
    {
        return Task.FromResult(_viewerId == _profileUserId);
    }
}