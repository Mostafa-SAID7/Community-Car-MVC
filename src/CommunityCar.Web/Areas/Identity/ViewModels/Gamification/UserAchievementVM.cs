namespace CommunityCar.Web.Areas.Identity.ViewModels.Gamification;

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
