namespace CommunityCar.Application.Features.Analytics.ViewModels;

public class UserPrivacySettingsVM
{
    public Guid UserId { get; set; }
    public bool AllowActivityTracking { get; set; }
    public bool AllowInterestTracking { get; set; }
    public bool AllowLocationTracking { get; set; }
    public bool AllowPersonalizedRecommendations { get; set; }
    public bool AllowDataSharing { get; set; }
    public bool AllowAnalytics { get; set; }
    public DateTime LastUpdated { get; set; }
}