using CommunityCar.Domain.Entities.Account.Gamification;
using CommunityCar.Domain.Enums.Account;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding.Account.Gamification;

public class BadgeSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BadgeSeeder> _logger;

    private readonly Guid _userId;

    public BadgeSeeder(ApplicationDbContext context, ILogger<BadgeSeeder> logger, Guid userId)
    {
        _context = context;
        _logger = logger;
        _userId = userId;
    }

    public async Task SeedAsync()
    {
        if (await _context.UserBadges.AnyAsync()) return;

        _logger.LogInformation("Seeding user badges...");

        var badges = new List<UserBadge>
        {
            new UserBadge(_userId, "first-post", "First Post", "Posted your first content", "/icons/badges/first-post.png", BadgeCategory.Content, BadgeRarity.Common, 1),
            new UserBadge(_userId, "helpful", "Helpful Member", "Received 10 helpful votes", "/icons/badges/helpful.png", BadgeCategory.Engagement, BadgeRarity.Uncommon, 10),
            new UserBadge(_userId, "expert", "Expert Contributor", "Contributed 50 high-quality posts", "/icons/badges/expert.png", BadgeCategory.Content, BadgeRarity.Rare, 50),
            new UserBadge(_userId, "leader", "Community Leader", "Helped 100 community members", "/icons/badges/leader.png", BadgeCategory.Community, BadgeRarity.Epic, 100),
            new UserBadge(_userId, "enthusiast", "Car Enthusiast", "Active member for 1 year", "/icons/badges/enthusiast.png", BadgeCategory.Engagement, BadgeRarity.Common, 365),
            new UserBadge(_userId, "restoration", "Restoration Master", "Completed 5 restoration projects", "/icons/badges/restoration.png", BadgeCategory.Automotive, BadgeRarity.Rare, 5),
            new UserBadge(_userId, "track-day", "Track Day Hero", "Attended 10 track events", "/icons/badges/track.png", BadgeCategory.Automotive, BadgeRarity.Uncommon, 10),
            new UserBadge(_userId, "knowledge", "Knowledge Sharer", "Answered 25 questions", "/icons/badges/knowledge.png", BadgeCategory.Engagement, BadgeRarity.Uncommon, 25),
            new UserBadge(_userId, "photographer", "Photo Pro", "Uploaded 100 quality photos", "/icons/badges/photographer.png", BadgeCategory.Content, BadgeRarity.Rare, 100),
            new UserBadge(_userId, "early-adopter", "Early Adopter", "One of the first 1000 members", "/icons/badges/early.png", BadgeCategory.Special, BadgeRarity.Legendary, 1)
        };

        await _context.UserBadges.AddRangeAsync(badges);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Successfully seeded {badges.Count} user badges.");
    }
}