using CommunityCar.Application.Features.Shared.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Areas.Identity.ViewModels.Gamification;

public class GamificationStatsVM
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalPointsAwarded { get; set; }
    public int TotalBadgesEarned { get; set; }
    public int TotalAchievementsUnlocked { get; set; }
    public int TotalQuestsCompleted { get; set; }
    public int ActiveQuests { get; set; }
    public double AverageUserLevel { get; set; }
    public double EngagementRate { get; set; }
    public DateTime LastUpdated { get; set; }
    public List<ChartDataVM> PointsDistribution { get; set; } = new();
    public List<ChartDataVM> LevelDistribution { get; set; } = new();
    public List<ChartDataVM> ActivityTrend { get; set; } = new();
    public Dictionary<string, int> PopularBadges { get; set; } = new();
    public Dictionary<string, int> QuestCompletionRates { get; set; } = new();
}
