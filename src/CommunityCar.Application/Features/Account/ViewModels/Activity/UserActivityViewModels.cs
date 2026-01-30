namespace CommunityCar.Application.Features.Account.ViewModels.Activity;

public class CreateActivityRequest
{
    public Guid UserId { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

public class RecordProfileViewRequest
{
    public Guid ProfileUserId { get; set; }
    public Guid? ViewerId { get; set; }
    public bool IsAnonymous { get; set; }
    public string? ViewSource { get; set; }
    public string? Device { get; set; }
}

public class TimelineActivityVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public string ActivityIcon { get; set; } = string.Empty;
    public string ActivityColor { get; set; } = string.Empty;
}

public class ActivityDashboardVM
{
    public Guid UserId { get; set; }
    public List<TimelineActivityVM> RecentActivities { get; set; } = new();
    public Dictionary<string, int> ActivitiesByType { get; set; } = new();
    public int TotalActivities { get; set; }
    public DateTime? LastActivityDate { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool HasMore { get; set; }
}

public class ProfileViewVM
{
    public Guid Id { get; set; }
    public Guid ViewerId { get; set; }
    public Guid ProfileUserId { get; set; }
    public string ViewerName { get; set; } = string.Empty;
    public string? ViewerProfilePicture { get; set; }
    public string? ViewerLocation { get; set; }
    public string? Location => ViewerLocation;
    public DateTime ViewedAt { get; set; }
    public string ViewedTimeAgo { get; set; } = string.Empty;
    public string TimeAgo => ViewedTimeAgo;
    public bool IsAnonymous { get; set; }
    public string? Device { get; set; } 
    public string? DeviceType => Device;
    public string? ViewSource { get; set; }
    public bool IsMutual { get; set; }
}


public class ProfileViewStatsVM
{
    public Guid ProfileUserId { get; set; }
    public int TotalViews { get; set; }
    public int UniqueViewers { get; set; }
    public int ViewsToday { get; set; }
    public int ViewsThisWeek { get; set; }
    public int ViewsThisMonth { get; set; }
    public Dictionary<DateTime, int> DailyViews { get; set; } = new();
    
    // Properties required by Service
    public List<ProfileViewVM> RecentViews { get; set; } = new();
    public List<ProfileViewerVM> TopViewers { get; set; } = new();
    public Dictionary<string, int> ViewsBySource { get; set; } = new();
    public Dictionary<DateTime, int> ViewsByDate { get; set; } = new();
    public double AverageViewsPerDay { get; set; }

    // Properties required by View (Aliases or New)
    public int WeekViews => ViewsThisWeek;
    public int TodayViews => ViewsToday;
    public double AverageViewDuration { get; set; }
    public DateTime? LastViewedAt { get; set; }
    public string? MostCommonViewSource { get; set; }
    public Dictionary<string, int> ViewSourceBreakdown => ViewsBySource;
}

public class ProfileViewAnalyticsVM
{
    public ProfileViewStatsVM Stats { get; set; } = new();
    public List<ProfileViewerVM> TopViewers { get; set; } = new();
    public List<ProfileViewVM> RecentViews { get; set; } = new();
}

public class WhoViewedMyProfileVM
{
    public IEnumerable<ProfileViewerVM> Viewers { get; set; } = new List<ProfileViewerVM>();
    public int TotalViewers { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
    public ProfileViewStatsVM Stats { get; set; } = new();
}

public class ProfileViewerVM
{
    public Guid ViewerId { get; set; }
    public string ViewerName { get; set; } = string.Empty;
    public string? ViewerProfilePicture { get; set; }
    public int ViewCount { get; set; }
    public DateTime LastViewedAt { get; set; }
    public string LastViewedTimeAgo { get; set; } = string.Empty;
    public bool IsFollowing { get; set; }
    public TimeSpan AverageViewDuration { get; set; }
    public DateTime FirstViewedAt { get; set; }
    public bool IsMutualFollowing { get; set; }
    public string? ViewerLocation { get; set; }
}
