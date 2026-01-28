using CommunityCar.Domain.Events;

namespace CommunityCar.Domain.Events.Account.Registration;

public class UserRegistrationFailedEvent : IDomainEvent
{
    public UserRegistrationFailedEvent(
        string email,
        string username,
        string failureReason,
        string registrationMethod = "Email",
        string ipAddress = null,
        Dictionary<string, object> additionalData = null)
    {
        Email = email;
        Username = username;
        FailureReason = failureReason ?? throw new ArgumentNullException(nameof(failureReason));
        RegistrationMethod = registrationMethod;
        IpAddress = ipAddress;
        AdditionalData = additionalData ?? new Dictionary<string, object>();
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public string Email { get; }
    public string Username { get; }
    public string FailureReason { get; }
    public string RegistrationMethod { get; }
    public string IpAddress { get; }
    public Dictionary<string, object> AdditionalData { get; }
}