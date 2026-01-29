using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.Enums.Account;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding.Account.Activity;

public class UserActivitySeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<UserActivitySeeder> _logger;

    public UserActivitySeeder(ApplicationDbContext context, UserManager<User> userManager, ILogger<UserActivitySeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _context.UserActivities.AnyAsync()) return;

        _logger.LogInformation("Seeding user activities...");

        var users = await _userManager.Users.Take(20).ToListAsync(); // Seed activities for first 20 users
        var activities = new List<UserActivity>();
        var random = new Random();

        var activityTypes = new[]
        {
            ActivityType.Login,
            ActivityType.PostCreated,
            ActivityType.CommentAdded,
            ActivityType.ProfileUpdated,
            ActivityType.PhotoUploaded,
            ActivityType.ReviewPosted,
            ActivityType.QuestionAsked,
            ActivityType.AnswerProvided,
            ActivityType.EventAttended,
            ActivityType.GroupJoined
        };

        var entityTypes = new[] { "Post", "Comment", "Review", "Question", "Answer", "Event", "Group", "Photo" };
        var descriptions = new[]
        {
            "User logged into the platform",
            "Created a new post about car maintenance",
            "Added a helpful comment to a discussion",
            "Updated profile information",
            "Uploaded photos of their vehicle",
            "Posted a detailed car review",
            "Asked a question about engine repair",
            "Provided an answer to a community question",
            "Attended a local car meet event",
            "Joined a car enthusiast group"
        };

        foreach (var user in users)
        {
            // Generate 10-30 activities per user
            var activityCount = random.Next(10, 31);
            
            for (int i = 0; i < activityCount; i++)
            {
                var activityType = activityTypes[random.Next(activityTypes.Length)];
                var entityType = entityTypes[random.Next(entityTypes.Length)];
                var description = descriptions[random.Next(descriptions.Length)];
                var activityDate = DateTime.UtcNow.AddDays(-random.Next(1, 90)); // Activities from last 90 days

                var activity = new UserActivity(
                    user.Id,
                    activityType,
                    entityType,
                    Guid.NewGuid(), // Random entity ID
                    $"Sample {entityType}",
                    description,
                    "127.0.0.1",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                    "Unknown",
                    activityDate,
                    random.Next(1, 300), // Duration in seconds
                    true
                );

                activities.Add(activity);
            }
        }

        await _context.UserActivities.AddRangeAsync(activities);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Successfully seeded {activities.Count} user activities.");
    }
}