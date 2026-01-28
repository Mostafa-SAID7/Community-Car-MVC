using CommunityCar.Domain.Events.Account.Registration;

namespace CommunityCar.Domain.Events.Account.Factories.Registration;

public static class UserRegisteredEventFactory
{
    public static UserRegisteredEvent CreateUserRegistered(
        Guid userId,
        string email,
        string username,
        string firstName,
        string lastName,
        string registrationMethod = "Email",
        string ipAddress = null,
        string userAgent = null,
        bool requiresEmailVerification = true)
    {
        return new UserRegisteredEvent(
            userId,
            email,
            username,
            firstName,
            lastName,
            DateTime.UtcNow,
            registrationMethod,
            ipAddress,
            userAgent,
            requiresEmailVerification);
    }
}