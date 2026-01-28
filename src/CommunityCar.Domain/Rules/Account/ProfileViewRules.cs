namespace CommunityCar.Domain.Rules.Account;

/// <summary>
/// Business rules for profile viewing
/// </summary>
public static class ProfileViewRules
{
    /// <summary>
    /// Rule: Users cannot view their own profile for tracking purposes
    /// </summary>
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

    /// <summary>
    /// Rule: Private profiles can only be viewed by followers
    /// </summary>
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

    /// <summary>
    /// Rule: Blocked users cannot view profiles
    /// </summary>
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

    /// <summary>
    /// Rule: Prevent spam by limiting view frequency
    /// </summary>
    public class ViewFrequencyLimitRule : IBusinessRule
    {
        private readonly DateTime? _lastViewTime;
        private readonly int _minimumMinutesBetweenViews;

        public ViewFrequencyLimitRule(DateTime? lastViewTime, int minimumMinutesBetweenViews = 5)
        {
            _lastViewTime = lastViewTime;
            _minimumMinutesBetweenViews = minimumMinutesBetweenViews;
        }

        public string Message => $"Please wait at least {_minimumMinutesBetweenViews} minutes between profile views.";

        public Task<bool> IsBrokenAsync()
        {
            if (!_lastViewTime.HasValue)
                return Task.FromResult(false);

            var timeSinceLastView = DateTime.UtcNow - _lastViewTime.Value;
            return Task.FromResult(timeSinceLastView.TotalMinutes < _minimumMinutesBetweenViews);
        }
    }
}