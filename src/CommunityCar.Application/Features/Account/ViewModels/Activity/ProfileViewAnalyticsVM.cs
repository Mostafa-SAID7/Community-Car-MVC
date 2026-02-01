namespace CommunityCar.Application.Features.Account.ViewModels.Activity;

public class ProfileViewAnalyticsVM
{
    public ProfileViewStatsVM Stats { get; set; } = new();
    public List<ProfileViewerVM> TopViewers { get; set; } = new();
    public List<ProfileViewVM> RecentViews { get; set; } = new();
}