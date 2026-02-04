namespace CommunityCar.Application.Features.Account.ViewModels.Activity;

/// <summary>
/// ViewModel for profile view statistics
/// </summary>
public class ProfileViewStatsVM
{
    public Guid UserId { get; set; }
    public Guid ProfileUserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public int TotalViews { get; set; }
    public int ViewsToday { get; set; }
    public int ViewsThisWeek { get; set; }
    public int ViewsThisMonth { get; set; }
    public int UniqueViewers { get; set; }
    public DateTime LastViewedDate { get; set; }
    public Dictionary<DateTime, int> ViewsChart { get; set; } = new();
    public Dictionary<DateTime, int> ViewsByDate { get; set; } = new();
    public Dictionary<DateTime, int> DailyViews { get; set; } = new();
    public List<string> TopViewerLocations { get; set; } = new();
    public List<ProfileViewerVM> RecentViews { get; set; } = new();
    public List<ProfileViewerVM> TopViewers { get; set; } = new();
    public double AverageViewsPerDay { get; set; }
    public Dictionary<string, int> ViewsBySource { get; set; } = new();
}