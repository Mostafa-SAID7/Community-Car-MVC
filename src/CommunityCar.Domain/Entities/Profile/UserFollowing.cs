using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Profile;

public class UserFollowing : BaseEntity
{
    public Guid FollowerId { get; private set; }
    public Guid FollowedUserId { get; private set; }
    public DateTime FollowedAt { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string? FollowReason { get; private set; } // Why they followed (mutual friends, interests, etc.)
    public bool NotificationsEnabled { get; private set; } = true;
    public DateTime? LastInteractionAt { get; private set; }
    public int InteractionCount { get; private set; } = 0;

    public UserFollowing(Guid followerId, Guid followedUserId, string? followReason = null)
    {
        FollowerId = followerId;
        FollowedUserId = followedUserId;
        FollowedAt = DateTime.UtcNow;
        FollowReason = followReason;
    }

    private UserFollowing() { }

    public void Unfollow()
    {
        IsActive = false;
        Audit(UpdatedBy);
    }

    public void Refollow()
    {
        IsActive = true;
        FollowedAt = DateTime.UtcNow;
        Audit(UpdatedBy);
    }

    public void SetNotifications(bool enabled)
    {
        NotificationsEnabled = enabled;
        Audit(UpdatedBy);
    }

    public void RecordInteraction()
    {
        InteractionCount++;
        LastInteractionAt = DateTime.UtcNow;
        Audit(UpdatedBy);
    }

    public void UpdateFollowReason(string reason)
    {
        FollowReason = reason;
        Audit(UpdatedBy);
    }

    public bool IsRecentFollower(int daysThreshold = 7)
    {
        return DateTime.UtcNow.Subtract(FollowedAt).TotalDays <= daysThreshold;
    }

    public bool IsEngaged(int daysThreshold = 30)
    {
        return LastInteractionAt.HasValue && 
               DateTime.UtcNow.Subtract(LastInteractionAt.Value).TotalDays <= daysThreshold;
    }
}