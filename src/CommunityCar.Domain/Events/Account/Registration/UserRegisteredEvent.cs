using CommunityCar.Domain.Events;

namespace CommunityCar.Domain.Events.Account.Registration;

public class UserRegisteredEvent : IDomainEvent
{
    public UserRegisteredEvent(
        Guid userId,
        string email,
        string username,
        string firstName,
        string lastName,
        DateTime registrationDate,
        string registrationMethod = "Email",
        string ipAddress = null,
        string userAgent = null,
        bool requiresEmailVerification = true)
    {
        UserId = userId;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Username = username ?? throw new ArgumentNullException(nameof(username));
        FirstName = firstName;
        LastName = lastName;
        RegistrationDate = registrationDate;
        RegistrationMethod = registrationMethod;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        RequiresEmailVerification = requiresEmailVerification;
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public string Email { get; }
    public string Username { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public DateTime RegistrationDate { get; }
    public string RegistrationMethod { get; }
    public string IpAddress { get; }
    public string UserAgent { get; }
    public bool RequiresEmailVerification { get; }
    public string FullName => $"{FirstName} {LastName}".Trim();
    public bool IsSocialRegistration => !string.Equals(RegistrationMethod, "Email", StringComparison.OrdinalIgnoreCase);
}