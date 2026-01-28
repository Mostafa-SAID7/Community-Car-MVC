using CommunityCar.Domain.Events;

namespace CommunityCar.Domain.Events.Account.Security;

public class UserMfaEnabledEvent : IDomainEvent
{
    public UserMfaEnabledEvent(
        Guid userId,
        string email,
        DateTime enabledDate,
        string mfaMethod,
        Guid? enabledBy = null)
    {
        UserId = userId;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        EnabledDate = enabledDate;
        MfaMethod = mfaMethod ?? throw new ArgumentNullException(nameof(mfaMethod));
        EnabledBy = enabledBy;
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public string Email { get; }
    public DateTime EnabledDate { get; }
    public string MfaMethod { get; }
    public Guid? EnabledBy { get; }
    public bool IsSelfEnabled => !EnabledBy.HasValue || EnabledBy.Value == UserId;
}
