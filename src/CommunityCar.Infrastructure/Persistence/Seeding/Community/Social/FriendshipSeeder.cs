using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.Entities.Community.Friends;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding.Community.Social;

public class FriendshipSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<FriendshipSeeder> _logger;

    public FriendshipSeeder(
        ApplicationDbContext context,
        UserManager<User> userManager,
        ILogger<FriendshipSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _context.Friendships.AnyAsync()) return;

        _logger.LogInformation("Seeding friendships...");

        var users = await _userManager.Users.ToListAsync();
        var friendships = new List<Friendship>();
        var random = new Random();

        // Create random friendships - each user will have 5-15 friends
        foreach (var user in users)
        {
            var friendCount = random.Next(5, 16); // 5 to 15 friends
            var potentialFriends = users.Where(u => u.Id != user.Id).ToList();
            
            for (int i = 0; i < Math.Min(friendCount, potentialFriends.Count); i++)
            {
                var friendIndex = random.Next(potentialFriends.Count);
                var friend = potentialFriends[friendIndex];
                potentialFriends.RemoveAt(friendIndex);

                // Check if friendship already exists (in either direction)
                var existingFriendship = friendships.Any(f => 
                    (f.RequesterId == user.Id && f.ReceiverId == friend.Id) ||
                    (f.RequesterId == friend.Id && f.ReceiverId == user.Id));

                if (!existingFriendship)
                {
                    var friendship = new Friendship(user.Id, friend.Id);
                    
                    // 80% chance the friendship is accepted
                    if (random.Next(100) < 80)
                    {
                        friendship.Accept();
                    }
                    
                    friendships.Add(friendship);
                }
            }
        }

        await _context.Friendships.AddRangeAsync(friendships);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Successfully seeded {friendships.Count} friendships.");
    }
}