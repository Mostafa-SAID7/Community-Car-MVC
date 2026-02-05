namespace CommunityCar.Application.Features.Dashboard.Analytics.Users.Segments;

/// <summary>
/// User segment trend view model
/// </summary>
public class UserSegmentTrendVM
{
    public int SegmentId { get; set; }
    public string SegmentName { get; set; } = string.Empty;
    public string SegmentDescription { get; set; } = string.Empty;
    public int UserCount { get; set; }
    public double GrowthRate { get; set; }
    public DateTime TrendDate { get; set; }
    public string TrendDirection { get; set; } = string.Empty; // Growing, Declining, Stable
    public Dictionary<string, object> SegmentCharacteristics { get; set; } = new();
}