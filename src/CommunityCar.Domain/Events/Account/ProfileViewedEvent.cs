namespace CommunityCar.Domain.Events.Account;

/// <summary>
/// Domain event raised when a user profile is viewed
/// </summary>
public class ProfileViewedEvent : IDomainEvent
{
    public ProfileViewedEvent(Guid profileUserId, Guid? viewerId, string? viewSource, bool isAnonymous)
    {
        ProfileUserId = profileUserId;
        ViewerId = viewerId;
        ViewSource = viewSource;
        IsAnonymous = isAnonymous;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid ProfileUserId { get; }
    public Guid? ViewerId { get; }
    public string? ViewSource { get; }
    public bool IsAnonymous { get; }
    public DateTime OccurredOn { get; }
}