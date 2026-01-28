namespace CommunityCar.Domain.Events.Account;

/// <summary>
/// Domain event raised when a user follows another user
/// </summary>
public class UserFollowedEvent : IDomainEvent
{
    public UserFollowedEvent(Guid followerId, Guid followedUserId, string? followReason)
    {
        FollowerId = followerId;
        FollowedUserId = followedUserId;
        FollowReason = followReason;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid FollowerId { get; }
    public Guid FollowedUserId { get; }
    public string? FollowReason { get; }
    public DateTime OccurredOn { get; }
}