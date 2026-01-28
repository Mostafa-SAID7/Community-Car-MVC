using CommunityCar.Domain.Events;

namespace CommunityCar.Domain.Events.Account.Profile;

public class UserEmailChangedEvent : IDomainEvent
{
    public UserEmailChangedEvent(
        Guid userId,
        string oldEmail,
        string newEmail,
        DateTime changeDate,
        bool requiresVerification = true,
        Guid? changedBy = null)
    {
        UserId = userId;
        OldEmail = oldEmail ?? throw new ArgumentNullException(nameof(oldEmail));
        NewEmail = newEmail ?? throw new ArgumentNullException(nameof(newEmail));
        ChangeDate = changeDate;
        RequiresVerification = requiresVerification;
        ChangedBy = changedBy;
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public string OldEmail { get; }
    public string NewEmail { get; }
    public DateTime ChangeDate { get; }
    public bool RequiresVerification { get; }
    public Guid? ChangedBy { get; }
    public bool IsSelfInitiated => !ChangedBy.HasValue || ChangedBy.Value == UserId;
}
