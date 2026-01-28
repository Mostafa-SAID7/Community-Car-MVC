using CommunityCar.Domain.Events.Account.Authentication;

namespace CommunityCar.Domain.Events.Account.Factories.Authentication;

public static class UserLoginFailedEventFactory
{
    public static UserLoginFailedEvent CreateLoginFailed(
        Guid? userId,
        string email,
        string username,
        string failureReason,
        string ipAddress = null,
        string userAgent = null,
        int totalFailedAttempts = 1,
        bool isAccountLocked = false)
    {
        return new UserLoginFailedEvent(
            userId,
            email,
            username,
            DateTime.UtcNow,
            failureReason,
            ipAddress,
            userAgent,
            totalFailedAttempts,
            isAccountLocked);
    }
}