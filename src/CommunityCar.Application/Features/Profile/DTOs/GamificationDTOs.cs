using CommunityCar.Domain.Entities.Profile;

namespace CommunityCar.Application.Features.Profile.DTOs;

public class UserBadgeDTO
{
    public Guid Id { get; set; }
    public string BadgeId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public BadgeCategory Category { get; set; }
    public BadgeRarity Rarity { get; set; }
    public int Points { get; set; }
    public DateTime EarnedAt { get; set; }
    public bool IsDisplayed { get; set; }
    public string RarityColor { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
}

public class UserAchievementDTO
{
    public Guid Id { get; set; }
    public string AchievementId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CurrentProgress { get; set; }
    public int RequiredProgress { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int RewardPoints { get; set; }
    public string? RewardBadgeId { get; set; }
    public double ProgressPercentage { get; set; }
}

public class UserGamificationStatsDTO
{
    public int TotalPoints { get; set; }
    public int Level { get; set; }
    public int PointsToNextLevel { get; set; }
    public int TotalBadges { get; set; }
    public int CompletedAchievements { get; set; }
    public int TotalAchievements { get; set; }
    public List<UserBadgeDTO> RecentBadges { get; set; } = new();
    public List<UserAchievementDTO> InProgressAchievements { get; set; } = new();
}

public class BadgeEarnedEvent
{
    public Guid UserId { get; set; }
    public string BadgeId { get; set; } = string.Empty;
    public string BadgeName { get; set; } = string.Empty;
    public int Points { get; set; }
    public DateTime EarnedAt { get; set; }
}