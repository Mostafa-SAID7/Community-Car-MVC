using CommunityCar.Domain.Entities.Auth;
using CommunityCar.Domain.Entities.Community.QA;
using CommunityCar.Domain.Entities.Community.Friends;
using CommunityCar.Domain.Entities.Chats;
using CommunityCar.Domain.Entities.Profile;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding;

public class DataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(ApplicationDbContext context, UserManager<User> userManager, ILogger<DataSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            await SeedUsersAsync();
            await SeedUserProfilesAsync();
            await SeedFriendshipsAsync();
            await SeedConversationsAsync();
            await SeedQAAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

    private async Task SeedUsersAsync()
    {
        if (await _userManager.Users.CountAsync() >= 100) return;

        _logger.LogInformation("Seeding 100 users...");

        var users = new List<User>();
        var random = new Random();

        // Sample data for generating realistic users
        var firstNames = new[] { "Ahmed", "Fatima", "Omar", "Aisha", "Hassan", "Zeinab", "Khalid", "Nour", "Youssef", "Mariam", 
                                "Ali", "Layla", "Mahmoud", "Dina", "Amr", "Yasmin", "Tamer", "Rana", "Karim", "Hala",
                                "John", "Sarah", "Michael", "Emma", "David", "Olivia", "James", "Sophia", "Robert", "Isabella",
                                "William", "Mia", "Richard", "Charlotte", "Joseph", "Amelia", "Thomas", "Harper", "Christopher", "Evelyn",
                                "Daniel", "Abigail", "Matthew", "Emily", "Anthony", "Elizabeth", "Mark", "Sofia", "Donald", "Avery" };

        var lastNames = new[] { "Ahmed", "Hassan", "Ali", "Mohamed", "Ibrahim", "Mahmoud", "Youssef", "Omar", "Khalil", "Farouk",
                               "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
                               "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin" };

        var cities = new[] { "Cairo", "Alexandria", "Giza", "Sharm El Sheikh", "Hurghada", "Luxor", "Aswan", "Port Said",
                            "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego",
                            "London", "Paris", "Berlin", "Madrid", "Rome", "Amsterdam", "Vienna", "Prague", "Budapest", "Warsaw" };

        var countries = new[] { "Egypt", "United States", "United Kingdom", "Germany", "France", "Spain", "Italy", "Netherlands", 
                               "Austria", "Czech Republic", "Hungary", "Poland", "Canada", "Australia", "Japan", "South Korea" };

        var carBrands = new[] { "Toyota", "Honda", "Ford", "BMW", "Mercedes", "Audi", "Volkswagen", "Nissan", "Hyundai", "Kia" };

        // Create the initial seed user if it doesn't exist
        var seedUser = await _userManager.FindByEmailAsync("seed@communitycar.com");
        if (seedUser == null)
        {
            seedUser = new User("seed@communitycar.com", "seed@communitycar.com")
            {
                FullName = "Seed User",
                EmailConfirmed = true,
                City = "Cairo",
                Country = "Egypt",
                Bio = "Community Car platform administrator and seed user."
            };
            await _userManager.CreateAsync(seedUser, "Password123!");
            users.Add(seedUser);
        }

        // Generate 99 additional users
        for (int i = 1; i <= 99; i++)
        {
            var firstName = firstNames[random.Next(firstNames.Length)];
            var lastName = lastNames[random.Next(lastNames.Length)];
            var email = $"user{i:D3}@communitycar.com";
            var userName = $"user{i:D3}";
            var city = cities[random.Next(cities.Length)];
            var country = countries[random.Next(countries.Length)];
            var carBrand = carBrands[random.Next(carBrands.Length)];

            var user = new User(email, userName)
            {
                FullName = $"{firstName} {lastName}",
                EmailConfirmed = true,
                City = city,
                Country = country,
                Bio = $"Car enthusiast from {city}. I love my {carBrand} and enjoy sharing automotive knowledge with the community.",
                PhoneNumberConfirmed = random.Next(2) == 1,
                PhoneNumber = random.Next(2) == 1 ? $"+1{random.Next(100, 999)}{random.Next(100, 999)}{random.Next(1000, 9999)}" : null
            };

            var result = await _userManager.CreateAsync(user, "Password123!");
            if (result.Succeeded)
            {
                users.Add(user);
                _logger.LogInformation($"Created user: {user.UserName} ({user.FullName})");
            }
            else
            {
                _logger.LogWarning($"Failed to create user {userName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        _logger.LogInformation($"Successfully seeded {users.Count} users.");
    }

    private async Task SeedQAAsync()
    {
        if (await _context.Questions.AnyAsync()) return;

        var user = await _userManager.FindByEmailAsync("seed@communitycar.com");
        if (user == null) return;

        var questions = new List<Question>
        {
            new Question("How do I change my oil?", "I have a 2020 Honda Civic and I want to change the oil myself. What tools do I need?", user.Id),
            new Question("Best tires for snow?", "I live in Canada and need recommendations for winter tires.", user.Id),
            new Question("Check engine light is on", "My check engine light came on yesterday. The car seems to run fine. What should I do?", user.Id),
            new Question("How to improve fuel economy?", "With gas prices rising, I want to get better mileage. Any tips?", user.Id),
            new Question("Is it worth fixing a 20-year-old car?", "I have a 2005 Corolla with 200k miles. It needs a new transmission. Should I fix it?", user.Id)
        };

        await _context.Questions.AddRangeAsync(questions);
        await _context.SaveChangesAsync();

        var answers = new List<Answer>
        {
            new Answer("You'll need a 17mm wrench, an oil filter wrench, a drain pan, and obviously the oil and filter.", questions[0].Id, user.Id),
            new Answer("Michelin X-Ice are fantastic. Bridgestone Blizzaks are also very good but wear out faster.", questions[1].Id, user.Id),
            new Answer("Go to an auto parts store, they usually scan it for free.", questions[2].Id, user.Id)
        };

        await _context.Answers.AddRangeAsync(answers);
        await _context.SaveChangesAsync();
    }

    private async Task SeedUserProfilesAsync()
    {
        if (await _context.UserProfiles.AnyAsync()) return;

        _logger.LogInformation("Seeding user profiles...");

        var users = await _userManager.Users.ToListAsync();
        var profiles = new List<UserProfile>();

        foreach (var user in users)
        {
            var nameParts = user.FullName.Split(' ', 2);
            var firstName = nameParts[0];
            var lastName = nameParts.Length > 1 ? nameParts[1] : "";

            var profile = new UserProfile(user.Id, firstName, lastName);
            profile.UpdateInfo(user.Bio ?? "", user.City ?? "", user.Country ?? "");
            profiles.Add(profile);
        }

        await _context.UserProfiles.AddRangeAsync(profiles);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Successfully seeded {profiles.Count} user profiles.");
    }

    private async Task SeedFriendshipsAsync()
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

    private async Task SeedConversationsAsync()
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
