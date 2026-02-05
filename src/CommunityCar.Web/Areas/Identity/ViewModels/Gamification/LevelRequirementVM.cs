namespace CommunityCar.Web.Areas.Identity.ViewModels.Gamification;

/// <summary>
/// ViewModel for level requirements
/// </summary>
public class LevelRequirementVM
{
    public int Level { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PointsRequired { get; set; }
    public string IconUrl { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public List<string> Benefits { get; set; } = new();
    public List<RequirementVM> Requirements { get; set; } = new();
    public bool IsUnlocked { get; set; }
    public DateTime? UnlockedDate { get; set; }
}
