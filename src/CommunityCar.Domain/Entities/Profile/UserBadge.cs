using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Profile;

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
    public bool IsDisplayed { get; private set; }

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
}

public enum BadgeCategory
{
    Community = 1,
    Content = 2,
    Engagement = 3,
    Achievement = 4,
    Special = 5,
    Automotive = 6
}

public enum BadgeRarity
{
    Common = 1,
    Uncommon = 2,
    Rare = 3,
    Epic = 4,
    Legendary = 5
}