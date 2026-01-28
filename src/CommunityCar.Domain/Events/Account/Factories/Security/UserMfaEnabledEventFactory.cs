using CommunityCar.Domain.Events.Account.Security;

namespace CommunityCar.Domain.Events.Account.Factories.Security;

public static class UserMfaEnabledEventFactory
{
    public static UserMfaEnabledEvent CreateMfaEnabled(
        Guid userId,
        string email,
        string mfaMethod,
        Guid? enabledBy = null)
    {
        return new UserMfaEnabledEvent(
            userId,
            email,
            DateTime.UtcNow,
            mfaMethod,
            enabledBy);
    }
}