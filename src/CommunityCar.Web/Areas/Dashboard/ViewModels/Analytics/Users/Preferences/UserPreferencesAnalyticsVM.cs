namespace CommunityCar.Application.Features.Dashboard.Analytics.Users.Preferences;

/// <summary>
/// User preferences analytics view model
/// </summary>
public class UserPreferencesAnalyticsVM
{
    public Guid UserId { get; set; }
    public string PreferredLanguage { get; set; } = string.Empty;
    public string PreferredTheme { get; set; } = string.Empty;
    public Dictionary<string, bool> NotificationPreferences { get; set; } = new();
    public Dictionary<string, bool> ContentPreferences { get; set; } = new();
    public int TotalUsers { get; set; }
    public int UsersWithPreferences { get; set; }
    public double PreferenceCompletionRate { get; set; }
    public Dictionary<string, int> CategoryPreferences { get; set; } = new();
    public Dictionary<string, int> TagPreferences { get; set; } = new();
    public Dictionary<string, int> ContentTypePreferences { get; set; } = new();
    public List<PreferenceTrendVM> TrendingPreferences { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}