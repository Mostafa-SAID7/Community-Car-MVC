using CommunityCar.Domain.Events;

namespace CommunityCar.Domain.Events.Account.Profile;

public class UserUsernameChangedEvent : IDomainEvent
{
    public UserUsernameChangedEvent(
        Guid userId,
        string email,
        string oldUsername,
        string newUsername,
        DateTime changeDate,
        Guid? changedBy = null)
    {
        UserId = userId;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        OldUsername = oldUsername ?? throw new ArgumentNullException(nameof(oldUsername));
        NewUsername = newUsername ?? throw new ArgumentNullException(nameof(newUsername));
        ChangeDate = changeDate;
        ChangedBy = changedBy;
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public string Email { get; }
    public string OldUsername { get; }
    public string NewUsername { get; }
    public DateTime ChangeDate { get; }
    public Guid? ChangedBy { get; }
    public bool IsSelfInitiated => !ChangedBy.HasValue || ChangedBy.Value == UserId;
}
