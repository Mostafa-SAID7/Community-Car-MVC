using CommunityCar.Domain.Events.Account.Lifecycle;

namespace CommunityCar.Domain.Events.Account.Factories.Lifecycle;

public static class UserCreatedEventFactory
{
    public static UserCreatedEvent CreateUserCreated(
        Guid userId,
        string email,
        string username,
        string firstName,
        string lastName,
        Guid? createdBy = null,
        string creationMethod = "Registration",
        bool isActive = true,
        IEnumerable<string> initialRoles = null,
        Dictionary<string, object> metadata = null)
    {
        return new UserCreatedEvent(
            userId,
            email,
            username,
            firstName,
            lastName,
            DateTime.UtcNow,
            createdBy,
            creationMethod,
            isActive,
            initialRoles,
            metadata);
    }
}