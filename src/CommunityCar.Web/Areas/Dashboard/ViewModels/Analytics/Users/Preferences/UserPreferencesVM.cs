namespace CommunityCar.Application.Features.Dashboard.Analytics.Users.Preferences;

/// <summary>
/// User preferences view model
/// </summary>
public class UserPreferencesVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Dictionary<string, object> Preferences { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public DateTime LastUpdated { get; set; }
    public int InteractionCount { get; set; }
    public double EngagementScore { get; set; }
}