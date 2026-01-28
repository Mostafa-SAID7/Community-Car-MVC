using CommunityCar.Domain.Events;

namespace CommunityCar.Domain.Events.Account.Security;

public class PasswordResetCompletedEvent : IDomainEvent
{
    public PasswordResetCompletedEvent(
        Guid userId,
        string email,
        DateTime completionDate,
        string resetToken,
        string ipAddress = null)
    {
        UserId = userId;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        CompletionDate = completionDate;
        ResetToken = resetToken ?? throw new ArgumentNullException(nameof(resetToken));
        IpAddress = ipAddress;
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public string Email { get; }
    public DateTime CompletionDate { get; }
    public string ResetToken { get; }
    public string IpAddress { get; }
}
