using CommunityCar.Domain.Events.Account.Security;
using CommunityCar.Domain.Enums.Account;

namespace CommunityCar.Domain.Events.Account.Factories.Security;

public static class UserLockedEventFactory
{
    public static UserLockedEvent CreateUserLocked(
        Guid userId,
        string email,
        string username,
        LockoutType lockoutType,
        string lockoutReason,
        DateTime? lockoutEndTime = null,
        Guid? lockedBy = null,
        string ipAddress = null,
        int failedAttempts = 0,
        Dictionary<string, object> additionalData = null)
    {
        return new UserLockedEvent(
            userId,
            email,
            username,
            DateTime.UtcNow,
            lockoutType,
            lockoutReason,
            lockoutEndTime,
            lockedBy,
            ipAddress,
            failedAttempts,
            additionalData);
    }
}