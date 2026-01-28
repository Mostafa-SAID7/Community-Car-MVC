using CommunityCar.Domain.Events;

namespace CommunityCar.Domain.Events.Account.Registration;

public class UserEmailVerifiedEvent : IDomainEvent
{
    public UserEmailVerifiedEvent(
        Guid userId,
        string email,
        DateTime verificationDate,
        string verificationToken = null)
    {
        UserId = userId;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        VerificationDate = verificationDate;
        VerificationToken = verificationToken;
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public string Email { get; }
    public DateTime VerificationDate { get; }
    public string VerificationToken { get; }
}