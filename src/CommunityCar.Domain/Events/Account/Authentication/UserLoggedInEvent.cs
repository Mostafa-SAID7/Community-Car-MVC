using CommunityCar.Domain.Events;

namespace CommunityCar.Domain.Events.Account.Authentication;

public class UserLoggedInEvent : IDomainEvent
{
    public UserLoggedInEvent(
        Guid userId,
        string email,
        string username,
        DateTime loginDate,
        string ipAddress = null,
        string userAgent = null,
        string location = null,
        string loginMethod = "Password",
        bool isMfaVerified = false,
        Dictionary<string, object> metadata = null)
    {
        UserId = userId;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Username = username ?? throw new ArgumentNullException(nameof(username));
        LoginDate = loginDate;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Location = location;
        LoginMethod = loginMethod;
        IsMfaVerified = isMfaVerified;
        Metadata = metadata ?? new Dictionary<string, object>();
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public string Email { get; }
    public string Username { get; }
    public DateTime LoginDate { get; }
    public string IpAddress { get; }
    public string UserAgent { get; }
    public string Location { get; }
    public string LoginMethod { get; }
    public bool IsMfaVerified { get; }
    public Dictionary<string, object> Metadata { get; }
    public bool IsSocialLogin => !string.Equals(LoginMethod, "Password", StringComparison.OrdinalIgnoreCase);
}