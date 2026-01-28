using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Account.Profile;

public class UserProfileView : BaseEntity
{
    public Guid ViewerId { get; private set; }
    public Guid ProfileUserId { get; private set; }
    public DateTime ViewedAt { get; private set; }
    public string? ViewerIpAddress { get; private set; }
    public string? ViewerUserAgent { get; private set; }
    public string? ViewerLocation { get; private set; }
    public string? ReferrerUrl { get; private set; }
    public TimeSpan ViewDuration { get; private set; }
    public bool IsAnonymous { get; private set; }
    public string? ViewSource { get; private set; } // "search", "followers", "direct", "recommendation", etc.

    public UserProfileView(Guid viewerId, Guid profileUserId, string? ipAddress = null, string? userAgent = null, string? location = null, string? referrerUrl = null, string? viewSource = null)
    {
        ViewerId = viewerId;
        ProfileUserId = profileUserId;
        ViewedAt = DateTime.UtcNow;
        ViewerIpAddress = ipAddress;
        ViewerUserAgent = userAgent;
        ViewerLocation = location;
        ReferrerUrl = referrerUrl;
        ViewSource = viewSource;
        IsAnonymous = viewerId == Guid.Empty;
        ViewDuration = TimeSpan.Zero;
    }

    // For anonymous views
    public UserProfileView(Guid profileUserId, string? ipAddress = null, string? userAgent = null, string? location = null, string? referrerUrl = null, string? viewSource = null)
    {
        ViewerId = Guid.Empty;
        ProfileUserId = profileUserId;
        ViewedAt = DateTime.UtcNow;
        ViewerIpAddress = ipAddress;
        ViewerUserAgent = userAgent;
        ViewerLocation = location;
        ReferrerUrl = referrerUrl;
        ViewSource = viewSource;
        IsAnonymous = true;
        ViewDuration = TimeSpan.Zero;
    }

    private UserProfileView() { }

    public void UpdateViewDuration(TimeSpan duration)
    {
        ViewDuration = duration;
        Audit(UpdatedBy);
    }

    public void UpdateLocation(string location)
    {
        ViewerLocation = location;
        Audit(UpdatedBy);
    }

    public bool IsRecentView(int minutesThreshold = 30)
    {
        return DateTime.UtcNow.Subtract(ViewedAt).TotalMinutes <= minutesThreshold;
    }

    public bool IsLongView(int secondsThreshold = 30)
    {
        return ViewDuration.TotalSeconds >= secondsThreshold;
    }

    public bool IsSameViewer(Guid viewerId, string? ipAddress = null)
    {
        if (!IsAnonymous)
            return ViewerId == viewerId;
        
        return !string.IsNullOrEmpty(ipAddress) && ViewerIpAddress == ipAddress;
    }
}