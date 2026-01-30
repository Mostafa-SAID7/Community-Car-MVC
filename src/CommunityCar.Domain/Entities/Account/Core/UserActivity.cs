using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums.Account;

namespace CommunityCar.Domain.Entities.Account.Core;

public class UserActivity : BaseEntity
{
    public Guid UserId { get; private set; }
    public ActivityType ActivityType { get; private set; }
    public string EntityType { get; private set; } = string.Empty;
    public Guid? EntityId { get; private set; }
    public string? EntityTitle { get; private set; }
    public string? Description { get; private set; }
    public string? Metadata { get; private set; } // JSON for additional data
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public string? Location { get; private set; }
    public DateTime ActivityDate { get; private set; }
    public int Duration { get; private set; } // in seconds
    public bool IsTracked { get; private set; } = true;
    public int PointsAwarded { get; private set; }

    public UserActivity(
        Guid userId,
        ActivityType activityType,
        string entityType,
        Guid? entityId = null,
        string? entityTitle = null,
        string? description = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? location = null,
        DateTime? activityDate = null,
        int duration = 0,
        bool isTracked = true,
        int pointsAwarded = 0)
    {
        UserId = userId;
        ActivityType = activityType;
        EntityType = entityType;
        EntityId = entityId;
        EntityTitle = entityTitle;
        Description = description;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Location = location;
        ActivityDate = activityDate ?? DateTime.UtcNow;
        Duration = duration;
        IsTracked = isTracked;
        PointsAwarded = pointsAwarded;
    }

    public static UserActivity Create(Guid userId, ActivityType activityType, string entityType, Guid? entityId = null, string? description = null)
    {
        return new UserActivity(userId, activityType, entityType, entityId, null, description);
    }

    private UserActivity() { }

    public void SetMetadata(string metadata)
    {
        Metadata = metadata;
        Audit(UpdatedBy);
    }

    public void SetLocation(string ipAddress, string? userAgent = null, string? location = null)
    {
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Location = location;
        Audit(UpdatedBy);
    }

    public void SetDuration(int durationSeconds)
    {
        Duration = durationSeconds;
        Audit(UpdatedBy);
    }

    public void SetTracked(bool isTracked)
    {
        IsTracked = isTracked;
        Audit(UpdatedBy);
    }

    public void UpdateDescription(string description)
    {
        Description = description;
        Audit(UpdatedBy);
    }
}