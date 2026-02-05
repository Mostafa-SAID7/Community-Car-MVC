namespace CommunityCar.Application.Features.Dashboard.Analytics.Content;

/// <summary>
/// Traffic source analytics view model
/// </summary>
public class TrafficSourceVM
{
    public string Source { get; set; } = string.Empty;
    public string Medium { get; set; } = string.Empty;
    public int Sessions { get; set; }
    public int Users { get; set; }
    public int PageViews { get; set; }
    public decimal BounceRate { get; set; }
    public TimeSpan AverageSessionDuration { get; set; }
    public decimal ConversionRate { get; set; }
    public decimal Percentage { get; set; }
}