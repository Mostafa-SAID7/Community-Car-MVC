using CommunityCar.Domain.Events;

namespace CommunityCar.Domain.Events.Account.Profile;

public class UserProfileCompletedEvent : IDomainEvent
{
    public UserProfileCompletedEvent(
        Guid userId,
        DateTime completionDate,
        Dictionary<string, object> profileData = null)
    {
        UserId = userId;
        CompletionDate = completionDate;
        ProfileData = profileData ?? new Dictionary<string, object>();
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public DateTime CompletionDate { get; }
    public Dictionary<string, object> ProfileData { get; }
}
