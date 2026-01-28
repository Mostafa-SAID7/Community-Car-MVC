using CommunityCar.Domain.Events;
using CommunityCar.Domain.Enums.Account;

namespace CommunityCar.Domain.Events.Account.Authentication;

public class SuspiciousLoginAttemptEvent : IDomainEvent
{
    public SuspiciousLoginAttemptEvent(
        Guid userId,
        string email,
        DateTime attemptDate,
        string suspiciousActivity,
        string ipAddress = null,
        string userAgent = null,
        string location = null,
        LoginRiskLevel riskLevel = LoginRiskLevel.Medium,
        Dictionary<string, object> suspiciousIndicators = null)
    {
        UserId = userId;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        AttemptDate = attemptDate;
        SuspiciousActivity = suspiciousActivity ?? throw new ArgumentNullException(nameof(suspiciousActivity));
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Location = location;
        RiskLevel = riskLevel;
        SuspiciousIndicators = suspiciousIndicators ?? new Dictionary<string, object>();
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public string Email { get; }
    public DateTime AttemptDate { get; }
    public string SuspiciousActivity { get; }
    public string IpAddress { get; }
    public string UserAgent { get; }
    public string Location { get; }
    public LoginRiskLevel RiskLevel { get; }
    public Dictionary<string, object> SuspiciousIndicators { get; }
}