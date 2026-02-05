namespace CommunityCar.Application.Features.Dashboard.Analytics.Users.Preferences;

/// <summary>
/// User preference insights view model
/// </summary>
public class UserPreferenceInsightsVM
{
    public int TotalUsers { get; set; }
    public Dictionary<string, int> PreferenceDistribution { get; set; } = new();
    public List<PreferenceTrendVM> TopPreferences { get; set; } = new();
    public List<PreferenceTrendVM> EmergingPreferences { get; set; } = new();
    public double DiversityIndex { get; set; }
    public DateTime LastUpdated { get; set; }
}