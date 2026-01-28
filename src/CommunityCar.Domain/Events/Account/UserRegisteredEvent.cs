namespace CommunityCar.Domain.Events.Account;

/// <summary>
/// Domain event raised when a new user registers
/// </summary>
public class UserRegisteredEvent : IDomainEvent
{
    public UserRegisteredEvent(Guid userId, string email, string fullName, DateTime registeredAt)
    {
        UserId = userId;
        Email = email;
        FullName = fullName;
        RegisteredAt = registeredAt;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public string Email { get; }
    public string FullName { get; }
    public DateTime RegisteredAt { get; }
    public DateTime OccurredOn { get; }
}