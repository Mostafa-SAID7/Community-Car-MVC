using CommunityCar.Domain.Events.Account.Authentication;

namespace CommunityCar.Domain.Events.Account.Factories.Authentication;

public static class UserLoggedOutEventFactory
{
    public static UserLoggedOutEvent CreateUserLoggedOut(
        Guid userId,
        string email,
        string logoutReason = "UserInitiated",
        TimeSpan? sessionDuration = null)
    {
        return new UserLoggedOutEvent(
            userId,
            email,
            DateTime.UtcNow,
            logoutReason,
            sessionDuration);
    }
}