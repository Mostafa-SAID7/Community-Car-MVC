using CommunityCar.Domain.Events.Account.Security;

namespace CommunityCar.Domain.Events.Account.Factories.Security;

public static class UserPasswordChangedEventFactory
{
    public static UserPasswordChangedEvent CreatePasswordChanged(
        Guid userId,
        string email,
        Guid? changedBy = null,
        string changeReason = "UserRequested",
        bool wasExpired = false,
        bool wasReset = false)
    {
        return new UserPasswordChangedEvent(
            userId,
            email,
            DateTime.UtcNow,
            changedBy,
            changeReason,
            wasExpired,
            wasReset);
    }
}