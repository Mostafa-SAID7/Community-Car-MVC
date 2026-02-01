namespace CommunityCar.Application.Features.Account.ViewModels.Gamification;

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