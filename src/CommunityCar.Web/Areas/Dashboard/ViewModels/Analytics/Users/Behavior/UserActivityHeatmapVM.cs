namespace CommunityCar.Application.Features.Dashboard.Analytics.Users.Behavior;

/// <summary>
/// User activity heatmap view model
/// </summary>
public class UserActivityHeatmapVM
{
    public Guid UserId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Dictionary<DateTime, int> ActivityData { get; set; } = new();
    public DateTime Date { get; set; }
    public int Hour { get; set; }
    public int ActivityCount { get; set; }
    public double IntensityScore { get; set; }
    public Dictionary<string, int> ActivityTypes { get; set; } = new();
}