using CommunityCar.Domain.Events.Account.Authentication;

namespace CommunityCar.Domain.Events.Account.Factories.Authentication;

public static class UserLoggedInEventFactory
{
    public static UserLoggedInEvent CreateUserLoggedIn(
        Guid userId,
        string email,
        string username,
        string ipAddress = null,
        string userAgent = null,
        string location = null,
        string loginMethod = "Password",
        bool isMfaVerified = false,
        Dictionary<string, object> metadata = null)
    {
        return new UserLoggedInEvent(
            userId,
            email,
            username,
            DateTime.UtcNow,
            ipAddress,
            userAgent,
            location,
            loginMethod,
            isMfaVerified,
            metadata);
    }
}