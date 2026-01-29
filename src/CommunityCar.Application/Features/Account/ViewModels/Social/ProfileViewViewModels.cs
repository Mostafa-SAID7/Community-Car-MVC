namespace CommunityCar.Application.Features.Account.ViewModels.Social;

public class ProfileViewVM
{
    public Guid Id { get; set; }
    public Guid ViewerId { get; set; }
    public Guid ProfileUserId { get; set; }
    public string ViewerName { get; set; } = string.Empty;
    public string? ViewerProfilePicture { get; set; }
    public string Location { get; set; } = string.Empty;
    public string ViewerLocation { get; set; } = string.Empty;
    public DateTime ViewedAt { get; set; }
    public string ViewedTimeAgo { get; set; } = string.Empty;
    public bool IsAnonymous { get; set; }
    public string DeviceType { get; set; } = string.Empty;
    public string ViewSource { get; set; } = string.Empty;
    public double? ViewDuration { get; set; }
    public string? ReferrerUrl { get; set; }
}

public class ProfileViewStatsVM
{
    public Guid ProfileUserId { get; set; }
    public int TotalViews { get; set; }
    public int UniqueViewers { get; set; }
    public int ViewsToday { get; set; }
    public int ViewsThisWeek { get; set; }
    public int ViewsThisMonth { get; set; }
    public int TodayViews => ViewsToday; // Aliases for compatibility
    public int WeekViews => ViewsThisWeek;
    public int MonthViews => ViewsThisMonth;
    public Dictionary<DateTime, int> ViewsByDate { get; set; } = new();
    public List<ProfileViewVM> RecentViews { get; set; } = new();
    public List<ProfileViewerVM> TopViewers { get; set; } = new();
    public double AverageViewsPerDay { get; set; }
    public string MostCommonViewSource { get; set; } = string.Empty;
    public Dictionary<string, int> ViewSourceBreakdown { get; set; } = new();
}

public class ProfileViewerVM
{
    public Guid ViewerId { get; set; }
    public string ViewerName { get; set; } = string.Empty;
    public string? ViewerProfilePicture { get; set; }
    public int ViewCount { get; set; }
    public DateTime LastViewedAt { get; set; }
    public bool IsFollowing { get; set; }
}