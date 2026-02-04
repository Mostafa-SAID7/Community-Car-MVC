namespace CommunityCar.Application.Features.Dashboard.Overview.ViewModels;

public class OverviewVM
{
    public string TimeRange { get; set; } = "Last 30 days";
    public StatsVM Stats { get; set; } = new();
    public List<RecentActivityVM> RecentActivity { get; set; } = new();
    public List<TopContentVM> TopContent { get; set; } = new();
    public List<ActiveUserVM> ActiveUsers { get; set; } = new();
    public List<ChartDataVM> UserGrowthChart { get; set; } = new();
    public List<ChartDataVM> ContentChart { get; set; } = new();
    public List<ChartDataVM> EngagementChart { get; set; } = new();
    public SystemHealthVM SystemHealth { get; set; } = new();
    
    // Date range properties
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}