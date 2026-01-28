using CommunityCar.Domain.Events;

namespace CommunityCar.Domain.Events.Account.Security;

public class PasswordResetRequestedEvent : IDomainEvent
{
    public PasswordResetRequestedEvent(
        Guid userId,
        string email,
        DateTime requestDate,
        string resetToken,
        string ipAddress = null,
        DateTime? expirationDate = null)
    {
        UserId = userId;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        RequestDate = requestDate;
        ResetToken = resetToken ?? throw new ArgumentNullException(nameof(resetToken));
        IpAddress = ipAddress;
        ExpirationDate = expirationDate;
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public string Email { get; }
    public DateTime RequestDate { get; }
    public string ResetToken { get; }
    public string IpAddress { get; }
    public DateTime? ExpirationDate { get; }
}
