using CommunityCar.Domain.Events.Account.Registration;

namespace CommunityCar.Domain.Events.Account.Factories.Registration;

public static class UserRegistrationFailedEventFactory
{
    public static UserRegistrationFailedEvent CreateRegistrationFailed(
        string email,
        string username,
        string failureReason,
        string registrationMethod = "Email",
        string ipAddress = null,
        Dictionary<string, object> additionalData = null)
    {
        return new UserRegistrationFailedEvent(
            email,
            username,
            failureReason,
            registrationMethod,
            ipAddress,
            additionalData);
    }
}