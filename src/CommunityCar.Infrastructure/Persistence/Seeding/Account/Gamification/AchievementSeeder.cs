using CommunityCar.Domain.Entities.Account.Gamification;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding.Account.Gamification;

public class AchievementSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AchievementSeeder> _logger;

    public AchievementSeeder(ApplicationDbContext context, ILogger<AchievementSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _context.UserAchievements.AnyAsync()) return;

        _logger.LogInformation("Seeding user achievements...");

        var achievements = new List<UserAchievement>
        {
            new UserAchievement(Guid.NewGuid(), "Welcome to Community Car", "Completed profile setup", "profile-complete", 10),
            new UserAchievement(Guid.NewGuid(), "First Steps", "Made your first post", "first-post", 25),
            new UserAchievement(Guid.NewGuid(), "Getting Social", "Made 5 friends in the community", "social-butterfly", 50),
            new UserAchievement(Guid.NewGuid(), "Helpful Hand", "Received 10 helpful votes on your answers", "helpful-member", 100),
            new UserAchievement(Guid.NewGuid(), "Knowledge Seeker", "Asked 10 thoughtful questions", "curious-mind", 75),
            new UserAchievement(Guid.NewGuid(), "Problem Solver", "Provided 25 helpful answers", "problem-solver", 150),
            new UserAchievement(Guid.NewGuid(), "Community Champion", "Active for 30 consecutive days", "daily-driver", 200),
            new UserAchievement(Guid.NewGuid(), "Photo Enthusiast", "Uploaded 50 car photos", "shutterbug", 125),
            new UserAchievement(Guid.NewGuid(), "Review Master", "Written 10 detailed car reviews", "reviewer", 175),
            new UserAchievement(Guid.NewGuid(), "Event Organizer", "Organized your first community event", "organizer", 300),
            new UserAchievement(Guid.NewGuid(), "Restoration Expert", "Documented a complete restoration project", "restoration-guru", 500),
            new UserAchievement(Guid.NewGuid(), "Track Veteran", "Attended 5 track day events", "track-warrior", 250),
            new UserAchievement(Guid.NewGuid(), "Mentor", "Helped 10 new members get started", "mentor", 400),
            new UserAchievement(Guid.NewGuid(), "Content Creator", "Created 100 posts and comments", "content-king", 350),
            new UserAchievement(Guid.NewGuid(), "Community Legend", "Reached 1000 reputation points", "legend", 1000)
        };

        await _context.UserAchievements.AddRangeAsync(achievements);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Successfully seeded {achievements.Count} user achievements.");
    }
}