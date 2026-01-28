using CommunityCar.Domain.Events.Account.Lifecycle;

namespace CommunityCar.Domain.Events.Account.Factories.Lifecycle;

public static class UserDeactivatedEventFactory
{
    public static UserDeactivatedEvent CreateUserDeactivated(
        Guid userId,
        Guid? deactivatedBy = null,
        string deactivationReason = null)
    {
        return new UserDeactivatedEvent(
            userId,
            DateTime.UtcNow,
            deactivatedBy,
            deactivationReason);
    }
}