namespace CommunityCar.Application.Features.Dashboard.Analytics.Users.Behavior;

/// <summary>
/// User activity heatmap view model
/// </summary>
public class UserActivityHeatmapVM
{
    public DateTime Date { get; set; }
    public int Hour { get; set; }
    public int ActivityCount { get; set; }
    public double IntensityScore { get; set; }
    public Dictionary<string, int> ActivityTypes { get; set; } = new();
}