namespace CommunityCar.Web.Areas.Identity.ViewModels.Gamification;

public class AwardBadgeRequest
{
    public Guid UserId { get; set; }
    public string BadgeId { get; set; } = string.Empty;
    public string? Reason { get; set; }
}
