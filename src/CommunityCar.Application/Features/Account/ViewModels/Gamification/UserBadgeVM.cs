namespace CommunityCar.Application.Features.Account.ViewModels.Gamification;

public class UserBadgeVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string BadgeId { get; set; } = string.Empty;
    public string BadgeName { get; set; } = string.Empty;
    public string BadgeDescription { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Rarity { get; set; } = string.Empty;
    public DateTime AwardedAt { get; set; }
    public string AwardedTimeAgo { get; set; } = string.Empty;
    public bool IsDisplayed { get; set; }
    public int DisplayOrder { get; set; }
    public string RarityColor { get; set; } = string.Empty;
    public string CategoryIcon { get; set; } = string.Empty;
}