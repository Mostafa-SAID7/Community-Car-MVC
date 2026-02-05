namespace CommunityCar.Web.Areas.Identity.ViewModels.Gamification;

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
