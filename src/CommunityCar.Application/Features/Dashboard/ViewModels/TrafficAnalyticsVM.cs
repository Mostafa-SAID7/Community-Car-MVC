namespace CommunityCar.Application.Features.Dashboard.ViewModels;

public class TrafficAnalyticsVM
{
    public int PageViews { get; set; }
    public int UniquePageViews { get; set; }
    public decimal BounceRate { get; set; }
    public TimeSpan AverageSessionDuration { get; set; }
    public List<ChartDataVM> TrafficData { get; set; } = new();
    public List<TopPageVM> TopPages { get; set; } = new();
}