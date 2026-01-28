using CommunityCar.Domain.Events;

namespace CommunityCar.Domain.Events.Account.Security;

public class UserMfaDisabledEvent : IDomainEvent
{
    public UserMfaDisabledEvent(
        Guid userId,
        string email,
        DateTime disabledDate,
        string mfaMethod,
        Guid? disabledBy = null,
        string disableReason = null)
    {
        UserId = userId;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        DisabledDate = disabledDate;
        MfaMethod = mfaMethod ?? throw new ArgumentNullException(nameof(mfaMethod));
        DisabledBy = disabledBy;
        DisableReason = disableReason;
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public string Email { get; }
    public DateTime DisabledDate { get; }
    public string MfaMethod { get; }
    public Guid? DisabledBy { get; }
    public string DisableReason { get; }
    public bool IsSelfDisabled => !DisabledBy.HasValue || DisabledBy.Value == UserId;
}
