using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Analytics.Content;

/// <summary>
/// ViewModel for traffic analytics
/// </summary>
public class TrafficAnalyticsVM
{
    public int TotalPageViews { get; set; }
    public int UniquePageViews { get; set; }
    public int PageViews { get; set; }
    public int UniqueVisitors { get; set; }
    public int Sessions { get; set; }
    public decimal BounceRate { get; set; }
    public TimeSpan AverageSessionDuration { get; set; }
    public int PagesPerSession { get; set; }
    public List<TopContentAnalyticsVM> TopPages { get; set; } = new();
    public List<TrafficSourceVM> TrafficSources { get; set; } = new();
    public List<DeviceAnalyticsVM> DeviceBreakdown { get; set; } = new();
    public List<LocationAnalyticsVM> LocationBreakdown { get; set; } = new();
    public List<ChartDataVM> TrafficData { get; set; } = new();
}




