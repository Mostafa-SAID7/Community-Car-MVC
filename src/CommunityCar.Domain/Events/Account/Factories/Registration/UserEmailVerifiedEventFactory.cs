using CommunityCar.Domain.Events.Account.Registration;

namespace CommunityCar.Domain.Events.Account.Factories.Registration;

public static class UserEmailVerifiedEventFactory
{
    public static UserEmailVerifiedEvent CreateEmailVerified(
        Guid userId,
        string email,
        string verificationToken = null)
    {
        return new UserEmailVerifiedEvent(
            userId,
            email,
            DateTime.UtcNow,
            verificationToken);
    }
}