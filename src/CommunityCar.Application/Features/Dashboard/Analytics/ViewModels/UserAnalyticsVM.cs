namespace CommunityCar.Application.Features.Dashboard.Analytics.ViewModels;

public class UserAnalyticsVM
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int NewUsers { get; set; }
    public int ReturnUsers { get; set; }
    public double UserGrowthRate { get; set; }
    public double UserRetentionRate { get; set; }
    public double AverageSessionDuration { get; set; }
    public int AveragePageViews { get; set; }
    public List<ChartDataVM> UserGrowthChart { get; set; } = new();
    public List<ChartDataVM> UserActivityChart { get; set; } = new();
    public List<ChartDataVM> UserDemographicsChart { get; set; } = new();
}