using CommunityCar.Domain.Events;

namespace CommunityCar.Domain.Events.Account.Lifecycle;

public class UserActivatedEvent : IDomainEvent
{
    public UserActivatedEvent(
        Guid userId,
        DateTime activationDate,
        Guid? activatedBy = null,
        string activationReason = null)
    {
        UserId = userId;
        ActivationDate = activationDate;
        ActivatedBy = activatedBy;
        ActivationReason = activationReason;
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public DateTime ActivationDate { get; }
    public Guid? ActivatedBy { get; }
    public string ActivationReason { get; }
}