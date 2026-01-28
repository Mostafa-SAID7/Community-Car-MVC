using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Account.Authentication;

public class UserSession : BaseEntity
{
    public Guid UserId { get; private set; }
    public string SessionId { get; private set; }
    public string? DeviceInfo { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public string? Location { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }
    public DateTime LastActivityAt { get; private set; }
    public bool IsActive { get; private set; }
    public TimeSpan Duration => (EndedAt ?? DateTime.UtcNow) - StartedAt;

    public UserSession(
        Guid userId,
        string sessionId,
        string? deviceInfo = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? location = null)
    {
        UserId = userId;
        SessionId = sessionId;
        DeviceInfo = deviceInfo;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Location = location;
        StartedAt = DateTime.UtcNow;
        LastActivityAt = DateTime.UtcNow;
        IsActive = true;
    }

    private UserSession() { }

    public void UpdateActivity()
    {
        LastActivityAt = DateTime.UtcNow;
        Audit(UpdatedBy);
    }

    public void EndSession()
    {
        EndedAt = DateTime.UtcNow;
        IsActive = false;
        Audit(UpdatedBy);
    }

    public void UpdateLocation(string? location)
    {
        Location = location;
        Audit(UpdatedBy);
    }

    public bool IsExpired(TimeSpan timeout)
    {
        return DateTime.UtcNow - LastActivityAt > timeout;
    }

    public void ExpireSession()
    {
        if (IsActive)
        {
            EndedAt = DateTime.UtcNow;
            IsActive = false;
            Audit(UpdatedBy);
        }
    }
}