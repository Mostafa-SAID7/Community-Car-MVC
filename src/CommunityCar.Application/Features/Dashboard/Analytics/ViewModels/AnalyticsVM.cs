namespace CommunityCar.Application.Features.Dashboard.Analytics.ViewModels;

public class AnalyticsVM
{
    public string ContentType { get; set; } = "All";
    public UserAnalyticsVM UserAnalytics { get; set; } = new();
    public ContentAnalyticsVM ContentAnalytics { get; set; } = new();
    public TrafficAnalyticsVM TrafficAnalytics { get; set; } = new();
    public List<ChartDataVM> OverviewCharts { get; set; } = new();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
}