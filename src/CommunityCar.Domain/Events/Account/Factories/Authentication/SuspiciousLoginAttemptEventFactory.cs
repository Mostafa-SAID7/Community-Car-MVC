using CommunityCar.Domain.Events.Account.Authentication;
using CommunityCar.Domain.Enums.Account;

namespace CommunityCar.Domain.Events.Account.Factories.Authentication;

public static class SuspiciousLoginAttemptEventFactory
{
    public static SuspiciousLoginAttemptEvent CreateSuspiciousLoginAttempt(
        Guid userId,
        string email,
        string suspiciousActivity,
        string ipAddress = null,
        string userAgent = null,
        string location = null,
        LoginRiskLevel riskLevel = LoginRiskLevel.Medium,
        Dictionary<string, object> suspiciousIndicators = null)
    {
        return new SuspiciousLoginAttemptEvent(
            userId,
            email,
            DateTime.UtcNow,
            suspiciousActivity,
            ipAddress,
            userAgent,
            location,
            riskLevel,
            suspiciousIndicators);
    }
}