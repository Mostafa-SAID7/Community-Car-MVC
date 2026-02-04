using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Dashboard.Analytics.ViewModels;

public class TrafficAnalyticsVM
{
    public int TotalPageViews { get; set; }
    public int UniqueVisitors { get; set; }
    public double BounceRate { get; set; }
    public double AverageSessionDuration { get; set; }
    public int PagesPerSession { get; set; }
    public List<ChartDataVM> TrafficChart { get; set; } = new();
    public List<TopPageVM> TopPages { get; set; } = new();
    public List<ChartDataVM> TrafficSourceChart { get; set; } = new();
    public List<ChartDataVM> DeviceChart { get; set; } = new();
    public List<ChartDataVM> LocationChart { get; set; } = new();
}