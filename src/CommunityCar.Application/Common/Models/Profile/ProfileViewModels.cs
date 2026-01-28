namespace CommunityCar.Application.Common.Models.Profile;

public class ProfileViewVM
{
    public Guid Id { get; set; }
    public Guid ViewerId { get; set; }
    public Guid ProfileUserId { get; set; }
    public DateTime ViewedAt { get; set; }
    public string? ViewerName { get; set; }
    public string? ViewerProfilePicture { get; set; }
    public string? ViewerLocation { get; set; }
    public string? ViewSource { get; set; }
    public TimeSpan ViewDuration { get; set; }
    public bool IsAnonymous { get; set; }
    public string? ReferrerUrl { get; set; }
}

public class ProfileViewStatsVM
{
    public int TotalViews { get; set; }
    public int UniqueViewers { get; set; }
    public int TodayViews { get; set; }
    public int WeekViews { get; set; }
    public int MonthViews { get; set; }
    public double AverageViewDuration { get; set; }
    public DateTime? LastViewedAt { get; set; }
    public string? MostCommonViewSource { get; set; }
    public Dictionary<string, int> ViewSourceBreakdown { get; set; } = new();
    public Dictionary<DateTime, int> ViewTrends { get; set; } = new();
}

public class ProfileViewerVM
{
    public Guid ViewerId { get; set; }
    public string? ViewerName { get; set; }
    public string? ViewerProfilePicture { get; set; }
    public string? ViewerLocation { get; set; }
    public int ViewCount { get; set; }
    public DateTime LastViewedAt { get; set; }
    public DateTime FirstViewedAt { get; set; }
    public bool IsFollowing { get; set; }
    public bool IsMutualFollowing { get; set; }
    public TimeSpan AverageViewDuration { get; set; }
}

public class ProfileViewAnalyticsVM
{
    public ProfileViewStatsVM Stats { get; set; } = new();
    public IEnumerable<ProfileViewerVM> TopViewers { get; set; } = new List<ProfileViewerVM>();
    public IEnumerable<ProfileViewVM> RecentViews { get; set; } = new List<ProfileViewVM>();
    public Dictionary<string, int> ViewsByHour { get; set; } = new();
    public Dictionary<string, int> ViewsByDay { get; set; } = new();
    public Dictionary<string, int> ViewsByLocation { get; set; } = new();
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