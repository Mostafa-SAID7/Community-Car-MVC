using CommunityCar.Domain.Events;

namespace CommunityCar.Domain.Events.Account.Security;

public class UserMfaVerificationFailedEvent : IDomainEvent
{
    public UserMfaVerificationFailedEvent(
        Guid userId,
        string email,
        DateTime attemptDate,
        string mfaMethod,
        string failureReason,
        string ipAddress = null,
        int failedAttempts = 1)
    {
        UserId = userId;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        AttemptDate = attemptDate;
        MfaMethod = mfaMethod ?? throw new ArgumentNullException(nameof(mfaMethod));
        FailureReason = failureReason ?? throw new ArgumentNullException(nameof(failureReason));
        IpAddress = ipAddress;
        FailedAttempts = failedAttempts;
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public string Email { get; }
    public DateTime AttemptDate { get; }
    public string MfaMethod { get; }
    public string FailureReason { get; }
    public string IpAddress { get; }
    public int FailedAttempts { get; }
}
