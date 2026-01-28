using CommunityCar.Domain.Events;

namespace CommunityCar.Domain.Events.Account.Authentication;

public class UserLoggedOutEvent : IDomainEvent
{
    public UserLoggedOutEvent(
        Guid userId,
        string email,
        DateTime logoutDate,
        string logoutReason = "UserInitiated",
        TimeSpan? sessionDuration = null)
    {
        UserId = userId;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        LogoutDate = logoutDate;
        LogoutReason = logoutReason;
        SessionDuration = sessionDuration;
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public string Email { get; }
    public DateTime LogoutDate { get; }
    public string LogoutReason { get; }
    public TimeSpan? SessionDuration { get; }
    public bool IsAutomatic => !string.Equals(LogoutReason, "UserInitiated", StringComparison.OrdinalIgnoreCase);
}