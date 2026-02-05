namespace CommunityCar.Application.Features.Dashboard.Analytics.Content;

/// <summary>
/// Device analytics view model
/// </summary>
public class DeviceAnalyticsVM
{
    public string DeviceType { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string OperatingSystem { get; set; } = string.Empty;
    public string Browser { get; set; } = string.Empty;
    public int Sessions { get; set; }
    public int Users { get; set; }
    public int PageViews { get; set; }
    public decimal BounceRate { get; set; }
    public TimeSpan AverageSessionDuration { get; set; }
    public decimal Percentage { get; set; }
}