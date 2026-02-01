namespace CommunityCar.Application.Features.Account.ViewModels.Gamification;

public class UpdateAchievementProgressRequest
{
    public Guid UserId { get; set; }
    public string AchievementId { get; set; } = string.Empty;
    public double Progress { get; set; }
}