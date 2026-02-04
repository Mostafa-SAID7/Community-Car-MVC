namespace CommunityCar.Application.Features.Account.ViewModels.Gamification;

/// <summary>
/// ViewModel for user progression information
/// </summary>
public class UserProgressionVM
{
    public Guid UserId { get; set; }
    public int CurrentLevel { get; set; }
    public int CurrentPoints { get; set; }
    public int PointsToNextLevel { get; set; }
    public int TotalPointsForNextLevel { get; set; }
    public double ProgressPercentage { get; set; }
    public string CurrentLevelName { get; set; } = string.Empty;
    public string NextLevelName { get; set; } = string.Empty;
    public List<string> CurrentLevelBenefits { get; set; } = new();
    public List<string> NextLevelBenefits { get; set; } = new();
    public DateTime LastLevelUp { get; set; }
    public List<RecentAchievementVM> RecentAchievements { get; set; } = new();
    public List<UpcomingRewardVM> UpcomingRewards { get; set; } = new();
}