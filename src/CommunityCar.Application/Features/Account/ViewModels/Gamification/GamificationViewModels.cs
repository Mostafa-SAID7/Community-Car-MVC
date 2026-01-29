namespace CommunityCar.Application.Features.Account.ViewModels.Gamification;

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
    public string? Trend { get; set; } // Up, Down, Steady
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
