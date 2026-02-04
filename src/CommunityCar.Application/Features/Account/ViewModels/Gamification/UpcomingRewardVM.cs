namespace CommunityCar.Application.Features.Account.ViewModels.Gamification;

/// <summary>
/// ViewModel for upcoming rewards
/// </summary>
public class UpcomingRewardVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public int PointsRequired { get; set; }
    public int PointsRemaining { get; set; }
    public string Type { get; set; } = string.Empty; // Badge, Achievement, Perk
    public DateTime? EstimatedUnlockDate { get; set; }
}