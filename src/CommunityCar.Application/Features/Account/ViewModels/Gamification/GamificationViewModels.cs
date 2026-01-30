namespace CommunityCar.Application.Features.Account.ViewModels.Gamification;

#region Badge ViewModels

public class BadgeVM
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Rarity { get; set; } = string.Empty;
    public int Points { get; set; }
}

public class UserBadgeVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string BadgeId { get; set; } = string.Empty;
    public string BadgeName { get; set; } = string.Empty;
    public string BadgeDescription { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Rarity { get; set; } = string.Empty;
    public DateTime AwardedAt { get; set; }
    public string AwardedTimeAgo { get; set; } = string.Empty;
    public bool IsDisplayed { get; set; }
    public int DisplayOrder { get; set; }
    public string RarityColor { get; set; } = string.Empty;
    public string CategoryIcon { get; set; } = string.Empty;
}

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

public class AwardBadgeRequest
{
    public Guid UserId { get; set; }
    public string BadgeId { get; set; } = string.Empty;
    public string? Reason { get; set; }
}

#endregion

#region Achievement ViewModels

public class AchievementVM
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public int RequiredProgress { get; set; }
    public int Points { get; set; }
    public string? RewardBadgeCode { get; set; }
}

public class UserAchievementVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string AchievementId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AchievementType { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public double Progress { get; set; }
    public int ProgressPercentage { get; set; }
    public int Points { get; set; }
    public bool IsUnlocked => IsCompleted;
    public DateTime? UnlockedAt => CompletedAt;
    public string? UnlockedTimeAgo { get; set; }
    public string StatusText { get; set; } = string.Empty;
    public string StatusColor { get; set; } = string.Empty;
}

public class AchievementDashboardVM
{
    public Guid UserId { get; set; }
    public int TotalAchievements { get; set; }
    public int UnlockedAchievements { get; set; }
    public int InProgressAchievements { get; set; }
    public double CompletionPercentage { get; set; }
    public Dictionary<string, int> AchievementsByType { get; set; } = new();
    public List<UserAchievementVM> RecentAchievements { get; set; } = new();
    public List<UserAchievementVM> InProgressAchievementsList { get; set; } = new();
    public List<UserAchievementVM> AvailableAchievements { get; set; } = new();
}

public class GrantAchievementRequest
{
    public Guid UserId { get; set; }
    public string AchievementId { get; set; } = string.Empty;
}

public class UpdateAchievementProgressRequest
{
    public Guid UserId { get; set; }
    public string AchievementId { get; set; } = string.Empty;
    public double Progress { get; set; }
}

#endregion

#region Points & Leaderboard ViewModels

public class PointTransactionVM
{
    public int Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class LeaderboardEntryVM
{
    public int Rank { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public int Points { get; set; }
    public int Level { get; set; }
    public string? Trend { get; set; } 
}

public class GamificationOverviewVM
{
    public int TotalPoints { get; set; }
    public int Level { get; set; }
    public double LevelProgress { get; set; }
    public int PointsToNextLevel { get; set; }
    public int BadgesCount { get; set; }
    public int AchievementsCount { get; set; }
    public int Rank { get; set; }
    public List<PointTransactionVM> RecentTransactions { get; set; } = new();
}

#endregion
