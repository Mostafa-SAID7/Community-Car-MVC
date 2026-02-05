namespace CommunityCar.Web.Areas.Identity.ViewModels.Gamification;

public class UpdateAchievementProgressRequest
{
    public Guid UserId { get; set; }
    public string AchievementId { get; set; } = string.Empty;
    public double Progress { get; set; }
}
