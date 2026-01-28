using CommunityCar.Domain.Events.Account.Profile;

namespace CommunityCar.Domain.Events.Account.Factories.Profile;

public static class UserProfileUpdatedEventFactory
{
    public static UserProfileUpdatedEvent CreateProfileUpdated(
        Guid userId,
        string email,
        Dictionary<string, object> changedFields,
        Guid? updatedBy = null,
        string updateReason = null)
    {
        return new UserProfileUpdatedEvent(
            userId,
            email,
            DateTime.UtcNow,
            changedFields,
            updatedBy,
            updateReason);
    }
}