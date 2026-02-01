namespace CommunityCar.Application.Features.Account.ViewModels.Gamification;

public class AchievementVM
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public int RequiredProgress { get; set; }
    public int Points { get; set; }
    public string? RewardBadgeCode { get; set; }
}