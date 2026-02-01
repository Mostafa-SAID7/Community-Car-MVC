namespace CommunityCar.Application.Features.Account.ViewModels.Gamification;

public class GrantAchievementRequest
{
    public Guid UserId { get; set; }
    public string AchievementId { get; set; } = string.Empty;
}