namespace CommunityCar.Application.Features.Account.DTOs.Gamification;

#region Gamification Analytics DTOs

public class AchievementAnalyticsDTO
{
    public Guid UserId { get; set; }
    public int TotalAchievements { get; set; }
    public int UnlockedAchievements { get; set; }
    public int InProgressAchievements { get; set; }
    public Dictionary<string, int> AchievementsByType { get; set; } = new();
    public List<UserAchievementDTO> RecentAchievements { get; set; } = new();
}

public class BadgeAnalyticsDTO
{
    public Guid UserId { get; set; }
    public int TotalBadges { get; set; }
    public int DisplayedBadges { get; set; }
    public Dictionary<string, int> BadgesByCategory { get; set; } = new();
    public Dictionary<string, int> BadgesByRarity { get; set; } = new();
    public List<UserBadgeDTO> RecentBadges { get; set; } = new();
}

public class GamificationOverviewDTO
{
    public Guid UserId { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int ExperienceToNextLevel { get; set; }
    public double LevelProgress { get; set; }
    public int TotalAchievements { get; set; }
    public int UnlockedAchievements { get; set; }
    public int TotalBadges { get; set; }
    public int UserRank { get; set; }
    public List<UserAchievementDTO> RecentAchievements { get; set; } = new();
    public List<UserBadgeDTO> FeaturedBadges { get; set; } = new();
}

#endregion