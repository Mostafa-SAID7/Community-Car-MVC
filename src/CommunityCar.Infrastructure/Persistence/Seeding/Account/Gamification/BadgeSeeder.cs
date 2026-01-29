using CommunityCar.Domain.Entities.Account.Gamification;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding.Account.Gamification;

public class BadgeSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BadgeSeeder> _logger;

    public BadgeSeeder(ApplicationDbContext context, ILogger<BadgeSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _context.UserBadges.AnyAsync()) return;

        _logger.LogInformation("Seeding user badges...");

        var badges = new List<UserBadge>
        {
            new UserBadge(Guid.NewGuid(), "First Post", "Posted your first content", "first-post", "bronze", 1),
            new UserBadge(Guid.NewGuid(), "Helpful Member", "Received 10 helpful votes", "helpful", "silver", 10),
            new UserBadge(Guid.NewGuid(), "Expert Contributor", "Contributed 50 high-quality posts", "expert", "gold", 50),
            new UserBadge(Guid.NewGuid(), "Community Leader", "Helped 100 community members", "leader", "platinum", 100),
            new UserBadge(Guid.NewGuid(), "Car Enthusiast", "Active member for 1 year", "enthusiast", "bronze", 365),
            new UserBadge(Guid.NewGuid(), "Restoration Master", "Completed 5 restoration projects", "restoration", "gold", 5),
            new UserBadge(Guid.NewGuid(), "Track Day Hero", "Attended 10 track events", "track-day", "silver", 10),
            new UserBadge(Guid.NewGuid(), "Knowledge Sharer", "Answered 25 questions", "knowledge", "silver", 25),
            new UserBadge(Guid.NewGuid(), "Photo Pro", "Uploaded 100 quality photos", "photographer", "gold", 100),
            new UserBadge(Guid.NewGuid(), "Early Adopter", "One of the first 1000 members", "early-adopter", "platinum", 1)
        };

        await _context.UserBadges.AddRangeAsync(badges);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Successfully seeded {badges.Count} user badges.");
    }
}