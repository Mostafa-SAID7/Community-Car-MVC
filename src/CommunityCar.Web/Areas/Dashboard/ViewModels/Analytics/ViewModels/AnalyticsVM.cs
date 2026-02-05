namespace CommunityCar.Application.Features.Dashboard.Analytics.ViewModels;

public class AnalyticsVM
{
    public DateTime StartDate { get; set; } = DateTime.UtcNow.AddDays(-30);
    public DateTime EndDate { get; set; } = DateTime.UtcNow;
    public string ContentType { get; set; } = "all";
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int NewUsers { get; set; }
    public int PageViews { get; set; }
    public int Sessions { get; set; }
    public double BounceRate { get; set; }
    public double AverageSessionDuration { get; set; }
    public double ConversionRate { get; set; }
    public string? FilterType { get; set; }
    public string? FilterValue { get; set; }
    public int PageSize { get; set; } = 20;
    public int Page { get; set; } = 1;
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; } = "desc";
    
    // Missing properties for dashboard views
    public int TotalViews { get; set; }
    public int UniqueVisitors { get; set; }
    public double GrowthRate { get; set; }
    public double EngagementRate { get; set; }
}