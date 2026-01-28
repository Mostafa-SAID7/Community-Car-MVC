using CommunityCar.Domain.Events.Account.Profile;

namespace CommunityCar.Domain.Events.Account.Factories.Profile;

public static class UserRolesChangedEventFactory
{
    public static UserRolesChangedEvent CreateRolesChanged(
        Guid userId,
        string email,
        IEnumerable<string> oldRoles,
        IEnumerable<string> newRoles,
        Guid changedBy,
        string changeReason = null)
    {
        return new UserRolesChangedEvent(
            userId,
            email,
            DateTime.UtcNow,
            oldRoles,
            newRoles,
            changedBy,
            changeReason);
    }
}