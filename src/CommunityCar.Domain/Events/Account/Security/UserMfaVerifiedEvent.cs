using CommunityCar.Domain.Events;

namespace CommunityCar.Domain.Events.Account.Security;

public class UserMfaVerifiedEvent : IDomainEvent
{
    public UserMfaVerifiedEvent(
        Guid userId,
        string email,
        DateTime verificationDate,
        string mfaMethod,
        string ipAddress = null)
    {
        UserId = userId;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        VerificationDate = verificationDate;
        MfaMethod = mfaMethod ?? throw new ArgumentNullException(nameof(mfaMethod));
        IpAddress = ipAddress;
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public string Email { get; }
    public DateTime VerificationDate { get; }
    public string MfaMethod { get; }
    public string IpAddress { get; }
}
