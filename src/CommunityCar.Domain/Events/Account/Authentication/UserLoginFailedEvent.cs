using CommunityCar.Domain.Events;

namespace CommunityCar.Domain.Events.Account.Authentication;

public class UserLoginFailedEvent : IDomainEvent
{
    public UserLoginFailedEvent(
        Guid? userId,
        string email,
        string username,
        DateTime attemptDate,
        string failureReason,
        string ipAddress = null,
        string userAgent = null,
        int totalFailedAttempts = 1,
        bool isAccountLocked = false)
    {
        UserId = userId;
        Email = email;
        Username = username;
        AttemptDate = attemptDate;
        FailureReason = failureReason ?? throw new ArgumentNullException(nameof(failureReason));
        IpAddress = ipAddress;
        UserAgent = userAgent;
        TotalFailedAttempts = totalFailedAttempts;
        IsAccountLocked = isAccountLocked;
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public Guid? UserId { get; }
    public string Email { get; }
    public string Username { get; }
    public DateTime AttemptDate { get; }
    public string FailureReason { get; }
    public string IpAddress { get; }
    public string UserAgent { get; }
    public int TotalFailedAttempts { get; }
    public bool IsAccountLocked { get; }
}