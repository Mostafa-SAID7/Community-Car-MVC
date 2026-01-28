using CommunityCar.Domain.Events;
using CommunityCar.Domain.Enums.Account;

namespace CommunityCar.Domain.Events.Account.Security;

public class UserUnlockedEvent : IDomainEvent
{
    public UserUnlockedEvent(
        Guid userId,
        string email,
        string username,
        DateTime unlockDate,
        Guid? unlockedBy = null,
        string unlockReason = null,
        LockoutType previousLockoutType = LockoutType.Temporary,
        TimeSpan? lockoutDuration = null)
    {
        UserId = userId;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Username = username ?? throw new ArgumentNullException(nameof(username));
        UnlockDate = unlockDate;
        UnlockedBy = unlockedBy;
        UnlockReason = unlockReason;
        PreviousLockoutType = previousLockoutType;
        LockoutDuration = lockoutDuration;
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public string Email { get; }
    public string Username { get; }
    public DateTime UnlockDate { get; }
    public Guid? UnlockedBy { get; }
    public string UnlockReason { get; }
    public LockoutType PreviousLockoutType { get; }
    public TimeSpan? LockoutDuration { get; }
    public bool IsAutomatic => !UnlockedBy.HasValue;
    public bool IsManual => UnlockedBy.HasValue;
}
