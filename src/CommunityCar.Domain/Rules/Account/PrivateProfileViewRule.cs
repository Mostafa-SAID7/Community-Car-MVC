using CommunityCar.Domain.Rules;

namespace CommunityCar.Domain.Rules.Account;

public class PrivateProfileViewRule : IBusinessRule
{
    private readonly bool _isProfilePublic;
    private readonly bool _isFollowing;
    private readonly Guid _viewerId;
    private readonly Guid _profileUserId;

    public PrivateProfileViewRule(bool isProfilePublic, bool isFollowing, Guid viewerId, Guid profileUserId)
    {
        _isProfilePublic = isProfilePublic;
        _isFollowing = isFollowing;
        _viewerId = viewerId;
        _profileUserId = profileUserId;
    }

    public string Message => "Private profiles can only be viewed by followers.";

    public Task<bool> IsBrokenAsync()
    {
        // Profile owner can always view their own profile
        if (_viewerId == _profileUserId)
            return Task.FromResult(false);

        // Public profiles can be viewed by anyone
        if (_isProfilePublic)
            return Task.FromResult(false);

        // Private profiles require following relationship
        return Task.FromResult(!_isFollowing);
    }
}