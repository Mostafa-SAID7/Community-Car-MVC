using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.Entities.Community.Friends;
using CommunityCar.Domain.Entities.Chats;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding.Community.Social;

public class ConversationSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<ConversationSeeder> _logger;

    public ConversationSeeder(
        ApplicationDbContext context,
        UserManager<User> userManager,
        ILogger<ConversationSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _context.Conversations.AnyAsync()) return;

        _logger.LogInformation("Seeding conversations and messages...");

        var users = await _userManager.Users.ToListAsync();
        var acceptedFriendships = await _context.Friendships
            .Where(f => f.Status == FriendshipStatus.Accepted)
            .ToListAsync();

        var conversations = new List<Conversation>();
        var messages = new List<Message>();
        var random = new Random();

        var sampleMessages = new[]
        {
            "Hey! How's your car running?",
            "Did you see the latest car show?",
            "I'm thinking about upgrading my wheels, any suggestions?",
            "Thanks for the advice on the oil change!",
            "Have you tried that new car wash downtown?",
            "My engine is making a weird noise, any ideas?",
            "Just got back from a road trip, amazing experience!",
            "Do you know any good mechanics in the area?",
            "I saw your post about the car meet, sounds fun!",
            "What do you think about electric cars?",
            "My car passed inspection today! 🎉",
            "Need recommendations for winter tires",
            "Anyone know about this error code?",
            "Great weather for a drive today!",
            "Just installed new speakers in my car",
            "Traffic was crazy today, how was yours?",
            "Found a great deal on car parts online",
            "My car hit 100k miles today!",
            "Planning a weekend road trip, any route suggestions?",
            "Just washed and waxed my car, looks brand new!"
        };

        // Create conversations between friends (50% of accepted friendships get conversations)
        var friendshipsWithConversations = acceptedFriendships
            .Where(f => random.Next(100) < 50)
            .ToList();

        foreach (var friendship in friendshipsWithConversations)
        {
            var user1 = users.First(u => u.Id == friendship.RequesterId);
            var user2 = users.First(u => u.Id == friendship.ReceiverId);
            
            var conversation = new Conversation($"Chat between {user1.FullName} and {user2.FullName}", false);
            conversations.Add(conversation);

            // Add 3-10 messages to each conversation
            var messageCount = random.Next(3, 11);
            var participants = new[] { friendship.RequesterId, friendship.ReceiverId };

            for (int i = 0; i < messageCount; i++)
            {
                var senderId = participants[random.Next(2)];
                var messageContent = sampleMessages[random.Next(sampleMessages.Length)];
                
                var message = new Message(messageContent, conversation.Id, senderId);
                
                // 70% chance the message is read
                if (random.Next(100) < 70)
                {
                    message.MarkAsRead();
                }
                
                messages.Add(message);
            }
        }

        // Create a few group conversations
        for (int i = 0; i < 5; i++)
        {
            var groupName = $"Car Enthusiasts Group {i + 1}";
            var groupConversation = new Conversation(groupName, true);
            conversations.Add(groupConversation);

            // Add messages from random users
            var messageCount = random.Next(10, 21);
            for (int j = 0; j < messageCount; j++)
            {
                var randomUser = users[random.Next(users.Count)];
                var messageContent = sampleMessages[random.Next(sampleMessages.Length)];
                
                var message = new Message(messageContent, groupConversation.Id, randomUser.Id);
                
                // 60% chance the message is read in group chats
                if (random.Next(100) < 60)
                {
                    message.MarkAsRead();
                }
                
                messages.Add(message);
            }
        }

        await _context.Conversations.AddRangeAsync(conversations);
        await _context.SaveChangesAsync();

        await _context.Messages.AddRangeAsync(messages);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Successfully seeded {conversations.Count} conversations and {messages.Count} messages.");
    }
}