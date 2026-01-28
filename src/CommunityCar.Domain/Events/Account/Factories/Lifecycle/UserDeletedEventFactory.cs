using CommunityCar.Domain.Events.Account.Lifecycle;

namespace CommunityCar.Domain.Events.Account.Factories.Lifecycle;

public static class UserDeletedEventFactory
{
    public static UserDeletedEvent CreateUserDeleted(
        Guid userId,
        string email,
        string username,
        Guid? deletedBy = null,
        string deletionReason = null,
        bool isSoftDelete = true)
    {
        return new UserDeletedEvent(
            userId,
            email,
            username,
            DateTime.UtcNow,
            deletedBy,
            deletionReason,
            isSoftDelete);
    }
}