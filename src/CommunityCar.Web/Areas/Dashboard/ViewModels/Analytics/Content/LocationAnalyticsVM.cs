namespace CommunityCar.Application.Features.Dashboard.Analytics.Content;

/// <summary>
/// Location analytics view model
/// </summary>
public class LocationAnalyticsVM
{
    public string Country { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int Sessions { get; set; }
    public int Users { get; set; }
    public int PageViews { get; set; }
    public decimal BounceRate { get; set; }
    public TimeSpan AverageSessionDuration { get; set; }
    public decimal Percentage { get; set; }
}