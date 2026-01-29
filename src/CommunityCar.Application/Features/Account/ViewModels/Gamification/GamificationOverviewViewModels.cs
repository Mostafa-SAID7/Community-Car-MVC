namespace CommunityCar.Application.Features.Account.ViewModels.Gamification;

public class GamificationOverviewVM
{
    public Guid UserId { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int ExperienceToNextLevel { get; set; }
    public double LevelProgress { get; set; }
    public int TotalAchievements { get; set; }
    public int UnlockedAchievements { get; set; }
    public int TotalBadges { get; set; }
    public int Rank { get; set; }
    public List<UserAchievementVM> RecentAchievements { get; set; } = new();
    public List<UserBadgeVM> FeaturedBadges { get; set; } = new();
}