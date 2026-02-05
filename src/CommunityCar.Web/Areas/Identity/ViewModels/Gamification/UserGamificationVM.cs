using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Areas.Identity.ViewModels.Gamification;

public class UserGamificationVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public int Level { get; set; }
    public string LevelName { get; set; } = string.Empty;
    public int PointsToNextLevel { get; set; }
    public int PointsInCurrentLevel { get; set; }
    public double ProgressToNextLevel { get; set; }
    public int BadgesEarned { get; set; }
    public int AchievementsUnlocked { get; set; }
    public int QuestsCompleted { get; set; }
    public int QuestsActive { get; set; }
    public int Streak { get; set; }
    public DateTime LastActivityDate { get; set; }
    public List<BadgeVM> RecentBadges { get; set; } = new();
    public List<AchievementVM> RecentAchievements { get; set; } = new();
    public List<QuestVM> ActiveQuests { get; set; } = new();
    public Dictionary<string, int> PointsByCategory { get; set; } = new();
}
