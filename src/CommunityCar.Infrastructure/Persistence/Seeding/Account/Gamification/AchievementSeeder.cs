using CommunityCar.Domain.Entities.Account.Gamification;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding.Account.Gamification;

public class AchievementSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AchievementSeeder> _logger;

    private readonly Guid _userId;

    public AchievementSeeder(ApplicationDbContext context, ILogger<AchievementSeeder> logger, Guid userId)
    {
        _context = context;
        _logger = logger;
        _userId = userId;
    }

    public async Task SeedAsync()
    {
        if (await _context.UserAchievements.AnyAsync()) return;

        _logger.LogInformation("Seeding user achievements...");

        var achievements = new List<UserAchievement>
        {
            new UserAchievement(_userId, "Welcome to Community Car", "Completed profile setup", "profile-complete", 1, 10),
            new UserAchievement(_userId, "First Steps", "Made your first post", "first-post", 1, 25),
            new UserAchievement(_userId, "Getting Social", "Made 5 friends in the community", "social-butterfly", 5, 50),
            new UserAchievement(_userId, "Helpful Hand", "Received 10 helpful votes on your answers", "helpful-member", 10, 100),
            new UserAchievement(_userId, "Knowledge Seeker", "Asked 10 thoughtful questions", "curious-mind", 10, 75),
            new UserAchievement(_userId, "Problem Solver", "Provided 25 helpful answers", "problem-solver", 25, 150),
            new UserAchievement(_userId, "Community Champion", "Active for 30 consecutive days", "daily-driver", 30, 200),
            new UserAchievement(_userId, "Photo Enthusiast", "Uploaded 50 car photos", "shutterbug", 50, 125),
            new UserAchievement(_userId, "Review Master", "Written 10 detailed car reviews", "reviewer", 10, 175),
            new UserAchievement(_userId, "Event Organizer", "Organized your first community event", "organizer", 1, 300),
            new UserAchievement(_userId, "Restoration Expert", "Documented a complete restoration project", "restoration-guru", 1, 500),
            new UserAchievement(_userId, "Track Veteran", "Attended 5 track day events", "track-warrior", 5, 250),
            new UserAchievement(_userId, "Mentor", "Helped 10 new members get started", "mentor", 10, 400),
            new UserAchievement(_userId, "Content Creator", "Created 100 posts and comments", "content-king", 100, 350),
            new UserAchievement(_userId, "Community Legend", "Reached 1000 reputation points", "legend", 1000, 1000)
        };

        await _context.UserAchievements.AddRangeAsync(achievements);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Successfully seeded {achievements.Count} user achievements.");
    }
}