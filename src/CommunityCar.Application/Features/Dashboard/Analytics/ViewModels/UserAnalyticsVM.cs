namespace CommunityCar.Application.Features.Dashboard.Analytics.ViewModels;

public class UserAnalyticsVM
{
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public int NewUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int ReturnUsers { get; set; }
    public decimal RetentionRate { get; set; }
    public TimeSpan TimeSpentOnSite { get; set; }
    public int LoginCount { get; set; }
    public int InteractionsCount { get; set; }
    public int PageViews { get; set; }
    public string MostVisitedSection { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string BrowserType { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}