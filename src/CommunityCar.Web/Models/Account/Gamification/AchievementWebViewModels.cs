namespace CommunityCar.Web.Models.Account.Gamification;

public class UserAchievementWebVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid AchievementId { get; set; }
    public string AchievementName { get; set; } = string.Empty;
    public string AchievementDescription { get; set; } = string.Empty;
    public string AchievementType { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public double Progress { get; set; }
    public int ProgressPercentage { get; set; }
    public bool IsUnlocked { get; set; }
    public DateTime? UnlockedAt { get; set; }
    public string? UnlockedTimeAgo { get; set; }
    public string StatusText { get; set; } = string.Empty;
    public string StatusColor { get; set; } = string.Empty;
}

public class AchievementDashboardWebVM
{
    public Guid UserId { get; set; }
    public int TotalAchievements { get; set; }
    public int UnlockedAchievements { get; set; }
    public int InProgressAchievements { get; set; }
    public double CompletionPercentage { get; set; }
    public Dictionary<string, int> AchievementsByType { get; set; } = new();
    public List<UserAchievementWebVM> RecentAchievements { get; set; } = new();
    public List<UserAchievementWebVM> InProgressAchievements { get; set; } = new();
    public List<UserAchievementWebVM> AvailableAchievements { get; set; } = new();
}