namespace CommunityCar.Application.Common.Models.Account;

#region Gamification DTOs

public class UserBadgeDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string Color { get; set; } = "#007bff";
    public DateTime EarnedAt { get; set; }
    public bool IsRare { get; set; }
}

public class BadgeDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string Color { get; set; } = "#007bff";
    public string Category { get; set; } = string.Empty;
    public int RequiredPoints { get; set; }
    public bool IsRare { get; set; }
    public bool IsEarned { get; set; }
}

public class UserAchievementDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public int Points { get; set; }
    public DateTime EarnedAt { get; set; }
    public string Category { get; set; } = string.Empty;
}

public class AchievementDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public int Points { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Criteria { get; set; } = string.Empty;
    public bool IsEarned { get; set; }
    public DateTime? EarnedAt { get; set; }
}

public class UserGamificationStatsDTO
{
    public int TotalPoints { get; set; }
    public int Level { get; set; }
    public int BadgesCount { get; set; }
    public int AchievementsCount { get; set; }
    public int Rank { get; set; }
    public int PointsToNextLevel { get; set; }
    public double LevelProgress { get; set; }
    public List<UserBadgeDTO> RecentBadges { get; set; } = new();
    public List<UserAchievementDTO> RecentAchievements { get; set; } = new();
}

public class PointTransactionDTO
{
    public Guid Id { get; set; }
    public int Points { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? RelatedEntityType { get; set; }
    public Guid? RelatedEntityId { get; set; }
}

public class LeaderboardEntryDTO
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public int Points { get; set; }
    public int Level { get; set; }
    public int Rank { get; set; }
    public int BadgesCount { get; set; }
    public int AchievementsCount { get; set; }
}

#endregion