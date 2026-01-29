namespace CommunityCar.Application.Features.Account.DTOs.Gamification;

#region User Badge DTOs

public class UserBadgeDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid BadgeId { get; set; }
    public string BadgeName { get; set; } = string.Empty;
    public string BadgeDescription { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Rarity { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public DateTime AwardedAt { get; set; }
    public bool IsDisplayed { get; set; }
    public int DisplayOrder { get; set; }
}

public class AwardBadgeRequest
{
    public Guid UserId { get; set; }
    public Guid BadgeId { get; set; }
    public DateTime? AwardedAt { get; set; }
    public string? Reason { get; set; }
}

public class UpdateBadgeDisplayRequest
{
    public Guid UserId { get; set; }
    public Guid BadgeId { get; set; }
    public bool IsDisplayed { get; set; }
    public int DisplayOrder { get; set; }
}

public class RevokeBadgeRequest
{
    public Guid UserId { get; set; }
    public Guid BadgeId { get; set; }
    public string? Reason { get; set; }
}

#endregion