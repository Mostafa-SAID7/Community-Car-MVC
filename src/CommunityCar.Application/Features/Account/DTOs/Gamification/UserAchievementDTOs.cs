namespace CommunityCar.Application.Features.Account.DTOs.Gamification;

#region User Achievement DTOs

public class UserAchievementDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid AchievementId { get; set; }
    public string AchievementName { get; set; } = string.Empty;
    public string AchievementDescription { get; set; } = string.Empty;
    public string AchievementType { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public double Progress { get; set; }
    public bool IsUnlocked { get; set; }
    public DateTime? UnlockedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GrantAchievementRequest
{
    public Guid UserId { get; set; }
    public Guid AchievementId { get; set; }
    public DateTime? UnlockedAt { get; set; }
}

public class UpdateAchievementProgressRequest
{
    public Guid UserId { get; set; }
    public Guid AchievementId { get; set; }
    public double Progress { get; set; }
}

public class RevokeAchievementRequest
{
    public Guid UserId { get; set; }
    public Guid AchievementId { get; set; }
    public string? Reason { get; set; }
}

#endregion