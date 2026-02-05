namespace CommunityCar.Web.Areas.Identity.ViewModels.Gamification;

/// <summary>
/// ViewModel for recent achievements
/// </summary>
public class RecentAchievementVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public int PointsAwarded { get; set; }
    public DateTime AchievedDate { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Rarity { get; set; } = string.Empty; // Common, Rare, Epic, Legendary
}
