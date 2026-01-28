using CommunityCar.Domain.Events.Account.Security;

namespace CommunityCar.Domain.Events.Account.Factories.Security;

public static class PasswordResetRequestedEventFactory
{
    public static PasswordResetRequestedEvent CreatePasswordResetRequested(
        Guid userId,
        string email,
        string resetToken,
        string ipAddress = null,
        DateTime? expirationDate = null)
    {
        return new PasswordResetRequestedEvent(
            userId,
            email,
            DateTime.UtcNow,
            resetToken,
            ipAddress,
            expirationDate);
    }
}