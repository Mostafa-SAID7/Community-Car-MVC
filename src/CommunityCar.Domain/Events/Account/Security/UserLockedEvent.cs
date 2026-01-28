using CommunityCar.Domain.Events;
using CommunityCar.Domain.Enums.Account;

namespace CommunityCar.Domain.Events.Account.Security;

public class UserLockedEvent : IDomainEvent
{
    public UserLockedEvent(
        Guid userId,
        string email,
        string username,
        DateTime lockoutDate,
        LockoutType lockoutType,
        string lockoutReason,
        DateTime? lockoutEndTime = null,
        Guid? lockedBy = null,
        string ipAddress = null,
        int failedAttempts = 0,
        Dictionary<string, object> additionalData = null)
    {
        UserId = userId;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Username = username ?? throw new ArgumentNullException(nameof(username));
        LockoutDate = lockoutDate;
        LockoutType = lockoutType;
        LockoutReason = lockoutReason ?? throw new ArgumentNullException(nameof(lockoutReason));
        LockoutEndTime = lockoutEndTime;
        LockedBy = lockedBy;
        IpAddress = ipAddress;
        FailedAttempts = failedAttempts;
        AdditionalData = additionalData ?? new Dictionary<string, object>();
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public string Email { get; }
    public string Username { get; }
    public DateTime LockoutDate { get; }
    public LockoutType LockoutType { get; }
    public string LockoutReason { get; }
    public DateTime? LockoutEndTime { get; }
    public Guid? LockedBy { get; }
    public string IpAddress { get; }
    public int FailedAttempts { get; }
    public Dictionary<string, object> AdditionalData { get; }
    public bool IsPermanent => LockoutType == LockoutType.Permanent;
    public bool IsAutomatic => !LockedBy.HasValue;
    public TimeSpan? LockoutDuration => LockoutEndTime.HasValue ? LockoutEndTime.Value - LockoutDate : null;
}
