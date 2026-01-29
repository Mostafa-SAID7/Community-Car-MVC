namespace CommunityCar.Application.Features.Account.ViewModels.Social;

public class UserProfileViewVM
{
    public Guid Id { get; set; }
    public Guid ViewerId { get; set; }
    public Guid ProfileUserId { get; set; }
    public string ViewerName { get; set; } = string.Empty;
    public string? ViewerProfilePicture { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTime ViewedAt { get; set; }
    public string ViewedTimeAgo { get; set; } = string.Empty;
    public bool IsAnonymous { get; set; }
    public string DeviceType { get; set; } = string.Empty;
}

public class ProfileViewAnalyticsVM
{
    public Guid ProfileUserId { get; set; }
    public int TotalViews { get; set; }
    public int UniqueViewers { get; set; }
    public int ViewsToday { get; set; }
    public int ViewsThisWeek { get; set; }
    public int ViewsThisMonth { get; set; }
    public Dictionary<DateTime, int> ViewsByDate { get; set; } = new();
    public List<UserProfileViewVM> RecentViews { get; set; } = new();
    public List<TopViewerVM> TopViewers { get; set; } = new();
    public double AverageViewsPerDay { get; set; }
}

public class TopViewerVM
{
    public Guid ViewerId { get; set; }
    public string ViewerName { get; set; } = string.Empty;
    public string? ViewerProfilePicture { get; set; }
    public int ViewCount { get; set; }
    public DateTime LastViewedAt { get; set; }
    public bool IsFollowing { get; set; }
}