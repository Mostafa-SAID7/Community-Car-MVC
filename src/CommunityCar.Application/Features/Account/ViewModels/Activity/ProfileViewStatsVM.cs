namespace CommunityCar.Application.Features.Account.ViewModels.Activity;

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