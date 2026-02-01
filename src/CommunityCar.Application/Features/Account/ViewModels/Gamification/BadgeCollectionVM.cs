namespace CommunityCar.Application.Features.Account.ViewModels.Gamification;

public class BadgeCollectionVM
{
    public Guid UserId { get; set; }
    public int TotalBadges { get; set; }
    public int DisplayedBadges { get; set; }
    public Dictionary<string, int> BadgesByCategory { get; set; } = new();
    public Dictionary<string, int> BadgesByRarity { get; set; } = new();
    public List<UserBadgeVM> DisplayedBadgesList { get; set; } = new();
    public List<UserBadgeVM> AllBadges { get; set; } = new();
    public List<UserBadgeVM> RecentBadges { get; set; } = new();
}