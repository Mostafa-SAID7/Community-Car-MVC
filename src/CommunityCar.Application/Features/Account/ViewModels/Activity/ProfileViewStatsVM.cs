namespace CommunityCar.Application.Features.Account.ViewModels.Activity;

/// <summary>
/// ViewModel for profile view statistics
/// </summary>
public class ProfileViewStatsVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public int TotalViews { get; set; }
    public int ViewsToday { get; set; }
    public int ViewsThisWeek { get; set; }
    public int ViewsThisMonth { get; set; }
    public DateTime LastViewedDate { get; set; }
    public Dictionary<DateTime, int> ViewsChart { get; set; } = new();
    public List<string> TopViewerLocations { get; set; } = new();
}