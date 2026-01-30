using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.Entities.Account.Profile;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding.Account.Social;

public class UserFollowingSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<UserFollowingSeeder> _logger;

    public UserFollowingSeeder(ApplicationDbContext context, UserManager<User> userManager, ILogger<UserFollowingSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _context.UserFollowings.AnyAsync()) return;

        _logger.LogInformation("Seeding user followings...");

        var users = await _userManager.Users.ToListAsync();
        var followings = new List<UserFollowing>();
        var random = new Random();

        var followReasons = new[]
        {
            "Great car content",
            "Helpful restoration tips",
            "Amazing photography",
            "Expert mechanical advice",
            "Inspiring builds",
            "Local car enthusiast",
            "Track day buddy",
            "Classic car expert",
            "Modification guru",
            "Community leader"
        };

        // Create random following relationships
        foreach (var user in users.Take(50)) // Process first 50 users
        {
            var followingCount = random.Next(5, 20); // Each user follows 5-20 others
            var potentialFollows = users.Where(u => u.Id != user.Id).ToList();

            for (int i = 0; i < Math.Min(followingCount, potentialFollows.Count); i++)
            {
                var followIndex = random.Next(potentialFollows.Count);
                var followedUser = potentialFollows[followIndex];
                potentialFollows.RemoveAt(followIndex);

                // Check if following relationship already exists
                var existingFollowing = followings.Any(f => 
                    f.FollowerId == user.Id && f.FollowedUserId == followedUser.Id);

                if (!existingFollowing)
                {
                    var reason = followReasons[random.Next(followReasons.Length)];
                    var followedAt = DateTime.UtcNow.AddDays(-random.Next(1, 365)); // Following from last year
                    var notificationsEnabled = random.Next(100) < 80; // 80% enable notifications
                    
                    var following = new UserFollowing(
                        user.Id,
                        followedUser.Id,
                        reason
                    );

                    followings.Add(following);
                }
            }
        }

        await _context.UserFollowings.AddRangeAsync(followings);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Successfully seeded {followings.Count} user followings.");
    }
}