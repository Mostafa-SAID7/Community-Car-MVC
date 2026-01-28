using CommunityCar.Domain.Events.Account.Profile;

namespace CommunityCar.Domain.Events.Account.Factories.Profile;

public static class UserEmailChangedEventFactory
{
    public static UserEmailChangedEvent CreateEmailChanged(
        Guid userId,
        string oldEmail,
        string newEmail,
        bool requiresVerification = true,
        Guid? changedBy = null)
    {
        return new UserEmailChangedEvent(
            userId,
            oldEmail,
            newEmail,
            DateTime.UtcNow,
            requiresVerification,
            changedBy);
    }
}