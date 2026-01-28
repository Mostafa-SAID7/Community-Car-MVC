using CommunityCar.Domain.Events.Account.Security;
using CommunityCar.Domain.Enums.Account;

namespace CommunityCar.Domain.Events.Account.Factories.Security;

public static class UserUnlockedEventFactory
{
    public static UserUnlockedEvent CreateUserUnlocked(
        Guid userId,
        string email,
        string username,
        Guid? unlockedBy = null,
        string unlockReason = null,
        LockoutType previousLockoutType = LockoutType.Temporary,
        TimeSpan? lockoutDuration = null)
    {
        return new UserUnlockedEvent(
            userId,
            email,
            username,
            DateTime.UtcNow,
            unlockedBy,
            unlockReason,
            previousLockoutType,
            lockoutDuration);
    }
}