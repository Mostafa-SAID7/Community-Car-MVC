using CommunityCar.Domain.Events.Account.Lifecycle;

namespace CommunityCar.Domain.Events.Account.Factories.Lifecycle;

public static class UserActivatedEventFactory
{
    public static UserActivatedEvent CreateUserActivated(
        Guid userId,
        Guid? activatedBy = null,
        string activationReason = null)
    {
        return new UserActivatedEvent(
            userId,
            DateTime.UtcNow,
            activatedBy,
            activationReason);
    }
}