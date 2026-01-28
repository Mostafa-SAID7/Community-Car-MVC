using CommunityCar.Domain.Events;

namespace CommunityCar.Domain.Events.Account.Lifecycle;

public class UserDeactivatedEvent : IDomainEvent
{
    public UserDeactivatedEvent(
        Guid userId,
        DateTime deactivationDate,
        Guid? deactivatedBy = null,
        string deactivationReason = null)
    {
        UserId = userId;
        DeactivationDate = deactivationDate;
        DeactivatedBy = deactivatedBy;
        DeactivationReason = deactivationReason;
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public DateTime DeactivationDate { get; }
    public Guid? DeactivatedBy { get; }
    public string DeactivationReason { get; }
}