using CommunityCar.Domain.Events;

namespace CommunityCar.Domain.Events.Account.Lifecycle;

public class UserCreatedEvent : IDomainEvent
{
    public UserCreatedEvent(
        Guid userId,
        string email,
        string username,
        string firstName,
        string lastName,
        DateTime createdDate,
        Guid? createdBy = null,
        string creationMethod = "Registration",
        bool isActive = true,
        IEnumerable<string> initialRoles = null,
        Dictionary<string, object> metadata = null)
    {
        UserId = userId;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Username = username ?? throw new ArgumentNullException(nameof(username));
        FirstName = firstName;
        LastName = lastName;
        CreatedDate = createdDate;
        CreatedBy = createdBy;
        CreationMethod = creationMethod;
        IsActive = isActive;
        InitialRoles = initialRoles?.ToList() ?? new List<string>();
        Metadata = metadata ?? new Dictionary<string, object>();
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public string Email { get; }
    public string Username { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public DateTime CreatedDate { get; }
    public Guid? CreatedBy { get; }
    public string CreationMethod { get; }
    public bool IsActive { get; }
    public List<string> InitialRoles { get; }
    public Dictionary<string, object> Metadata { get; }
    public string FullName => $"{FirstName} {LastName}".Trim();
    public bool IsAdminCreated => CreatedBy.HasValue;
    public bool HasInitialRoles => InitialRoles.Any();
}