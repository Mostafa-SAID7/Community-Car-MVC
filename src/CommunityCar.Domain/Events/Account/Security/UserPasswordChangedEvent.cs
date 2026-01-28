using CommunityCar.Domain.Events;

namespace CommunityCar.Domain.Events.Account.Security;

public class UserPasswordChangedEvent : IDomainEvent
{
    public UserPasswordChangedEvent(
        Guid userId,
        string email,
        DateTime changeDate,
        Guid? changedBy = null,
        string changeReason = "UserRequested",
        bool wasExpired = false,
        bool wasReset = false)
    {
        UserId = userId;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        ChangeDate = changeDate;
        ChangedBy = changedBy;
        ChangeReason = changeReason;
        WasExpired = wasExpired;
        WasReset = wasReset;
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public string Email { get; }
    public DateTime ChangeDate { get; }
    public Guid? ChangedBy { get; }
    public string ChangeReason { get; }
    public bool WasExpired { get; }
    public bool WasReset { get; }
    public bool IsSelfInitiated => !ChangedBy.HasValue || ChangedBy.Value == UserId;
}
