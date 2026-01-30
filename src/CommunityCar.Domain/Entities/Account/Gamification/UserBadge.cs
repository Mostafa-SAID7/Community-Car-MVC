using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums.Account;

namespace CommunityCar.Domain.Entities.Account.Gamification;

public class UserBadge : BaseEntity
{
    public Guid UserId { get; private set; }
    public string BadgeId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string? NameAr { get; private set; }
    public string? DescriptionAr { get; private set; }
    public string IconUrl { get; private set; }
    public BadgeCategory Category { get; private set; }
    public BadgeRarity Rarity { get; private set; }
    public int Points { get; private set; }
    public DateTime EarnedAt { get; private set; }
    public DateTime AwardedAt => EarnedAt; // Alias for repository
    public bool IsDisplayed { get; private set; }
    public int DisplayOrder { get; private set; }

    public UserBadge(Guid userId, string badgeId, string name, string description, string iconUrl, 
                     BadgeCategory category, BadgeRarity rarity, int points)
    {
        UserId = userId;
        BadgeId = badgeId;
        Name = name;
        Description = description;
        IconUrl = iconUrl;
        Category = category;
        Rarity = rarity;
        Points = points;
        EarnedAt = DateTime.UtcNow;
        IsDisplayed = true;
    }

    public static UserBadge Create(Guid userId, Guid badgeId, DateTime awardedAt)
    {
        // Note: Creating a stub UserBadge with Guid-based BadgeId and default values
        // because the repository expects a Guid badgeId in Create.
        return new UserBadge(userId, badgeId.ToString(), "Awarded Badge", "Description", "", BadgeCategory.Special, BadgeRarity.Common, 0)
        {
            EarnedAt = awardedAt
        };
    }

    public void UpdateArabicContent(string? nameAr, string? descriptionAr)
    {
        NameAr = nameAr;
        DescriptionAr = descriptionAr;
        Audit(UpdatedBy);
    }

    // EF Core constructor
    private UserBadge() { }

    public void ToggleDisplay()
    {
        IsDisplayed = !IsDisplayed;
        Audit(UpdatedBy);
    }

    public void SetDisplayStatus(bool isDisplayed)
    {
        IsDisplayed = isDisplayed;
        Audit(UpdatedBy);
    }

    public void SetDisplayOrder(int order)
    {
        DisplayOrder = order;
        Audit(UpdatedBy);
    }
}