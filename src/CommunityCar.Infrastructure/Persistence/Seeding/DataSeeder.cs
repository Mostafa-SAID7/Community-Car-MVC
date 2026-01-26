using CommunityCar.Domain.Entities.Auth;
using CommunityCar.Domain.Entities.Community.QA;
using CommunityCar.Domain.Entities.Community.Friends;
using CommunityCar.Domain.Entities.Community.Guides;
using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Entities.Community.News;
using CommunityCar.Domain.Entities.Community.Reviews;
using CommunityCar.Domain.Entities.Chats;
using CommunityCar.Domain.Entities.Profile;
using CommunityCar.Domain.Enums;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Seeding.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding;

public class DataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<DataSeeder> _logger;
    private readonly IServiceProvider _serviceProvider;

    public DataSeeder(ApplicationDbContext context, UserManager<User> userManager, ILogger<DataSeeder> logger, IServiceProvider serviceProvider)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Starting database seeding...");

            // Seed core data first
            await SeedUsersAsync();
            await SeedUserProfilesAsync();
            
            // Seed shared entities (categories, tags)
            await SeedSharedEntitiesAsync();
            
            // Seed community content
            await SeedFriendshipsAsync();
            await SeedConversationsAsync();
            await SeedQAAsync();
            await SeedGuidesAsync();
            await SeedNewsAsync();
            await SeedReviewsAsync();
            await SeedMapsAsync();
            await SeedEventsAsync();
            await SeedGroupsAsync();
            await SeedPostsAsync();
            
            // Seed shared interactions (reactions, comments, etc.)
            await SeedSharedInteractionsAsync();

            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task SeedSharedEntitiesAsync()
    {
        try
        {
            _logger.LogInformation("Seeding shared entities...");

            // Seed categories
            var categorySeeder = new CategorySeeder(_context, _serviceProvider.GetRequiredService<ILogger<CategorySeeder>>());
            await categorySeeder.SeedAsync();

            // Seed tags
            var tagSeeder = new TagSeeder(_context, _serviceProvider.GetRequiredService<ILogger<TagSeeder>>());
            await tagSeeder.SeedAsync();

            _logger.LogInformation("Shared entities seeding completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding shared entities");
            throw;
        }
    }

    private async Task SeedSharedInteractionsAsync()
    {
        try
        {
            _logger.LogInformation("Seeding shared interactions...");

            var interactionSeeder = new SharedInteractionSeeder(_context, _serviceProvider.GetRequiredService<ILogger<SharedInteractionSeeder>>());
            await interactionSeeder.SeedAsync();

            _logger.LogInformation("Shared interactions seeding completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding shared interactions");
            throw;
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
            profile.UpdateBasicInfo(user.Bio ?? "", user.City ?? "", user.Country ?? "");
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

    private async Task SeedGuidesAsync()
    {
        if (await _context.Guides.AnyAsync()) return;

        _logger.LogInformation("Seeding guides...");

        var users = await _userManager.Users.ToListAsync();
        var guides = new List<Guide>();
        var random = new Random();

        var guideData = new[]
        {
            new { Title = "Complete Oil Change Guide", Category = "Maintenance", Difficulty = GuideDifficulty.Beginner, Minutes = 45,
                  Summary = "Learn how to change your car's oil step by step with this comprehensive guide.",
                  Content = @"# Complete Oil Change Guide

## What You'll Need
- Engine oil (check your manual for type and quantity)
- Oil filter
- Oil drain pan
- Socket wrench set
- Oil filter wrench
- Funnel
- Jack and jack stands
- Gloves

## Step-by-Step Instructions

### 1. Prepare Your Vehicle
- Warm up your engine for 2-3 minutes (warm oil drains better)
- Park on level ground
- Engage parking brake
- Turn off engine and wait 5 minutes

### 2. Drain the Old Oil
- Jack up the front of your car
- Locate the oil drain plug under the engine
- Position drain pan under the plug
- Remove drain plug with socket wrench
- Let oil drain completely (15-20 minutes)

### 3. Replace Oil Filter
- Locate oil filter (usually cylindrical)
- Use oil filter wrench to remove old filter
- Apply thin layer of new oil to new filter's rubber gasket
- Install new filter hand-tight plus 3/4 turn

### 4. Refill with New Oil
- Replace drain plug with new gasket
- Lower vehicle
- Remove oil filler cap
- Use funnel to add new oil
- Check dipstick and add oil as needed

### 5. Final Steps
- Start engine and let idle for 5 minutes
- Check for leaks
- Turn off engine and check oil level
- Dispose of old oil and filter properly

## Tips
- Always use the oil type specified in your owner's manual
- Change oil every 3,000-5,000 miles depending on driving conditions
- Keep records of oil changes for warranty purposes",
                  Tags = new[] { "oil-change", "maintenance", "diy", "beginner" },
                  Prerequisites = new[] { "Basic tool knowledge", "Access to jack and stands" },
                  Tools = new[] { "Socket wrench set", "Oil filter wrench", "Jack", "Drain pan" }
            },
            new { Title = "Brake Pad Replacement", Category = "Maintenance", Difficulty = GuideDifficulty.Intermediate, Minutes = 90,
                  Summary = "Replace your brake pads safely with this detailed guide.",
                  Content = @"# Brake Pad Replacement Guide

## Safety First
⚠️ **Warning**: Brake work is safety-critical. If you're not confident, consult a professional.

## Tools Required
- Jack and jack stands
- Lug wrench
- C-clamp or brake piston tool
- Socket set
- Brake cleaner
- New brake pads
- Brake grease

## Step-by-Step Process

### 1. Preparation
- Park on level ground
- Engage parking brake
- Loosen lug nuts (don't remove yet)
- Jack up vehicle and secure with stands

### 2. Remove Wheel and Caliper
- Remove lug nuts and wheel
- Locate brake caliper
- Remove caliper bolts
- Carefully lift caliper off rotor

### 3. Replace Pads
- Remove old brake pads
- Clean caliper and rotor with brake cleaner
- Compress caliper piston with C-clamp
- Install new pads with proper orientation

### 4. Reassembly
- Reinstall caliper
- Tighten bolts to specification
- Replace wheel and lug nuts
- Lower vehicle

### 5. Break-In Process
- Pump brake pedal before driving
- Test brakes at low speed
- Follow manufacturer's break-in procedure

## Important Notes
- Always replace pads in pairs (both sides)
- Check rotor condition
- Bleed brakes if necessary",
                  Tags = new[] { "brakes", "safety", "maintenance", "intermediate" },
                  Prerequisites = new[] { "Intermediate mechanical knowledge", "Proper safety equipment" },
                  Tools = new[] { "Jack and stands", "Socket set", "C-clamp", "Brake cleaner" }
            },
            new { Title = "Engine Diagnostics with OBD2", Category = "Diagnostics", Difficulty = GuideDifficulty.Beginner, Minutes = 30,
                  Summary = "Learn to diagnose engine problems using an OBD2 scanner.",
                  Content = @"# Engine Diagnostics with OBD2

## Understanding OBD2
OBD2 (On-Board Diagnostics) is a standardized system that monitors your vehicle's performance and emissions.

## What You Need
- OBD2 scanner or smartphone app
- Vehicle with OBD2 port (1996 or newer)

## Locating the OBD2 Port
- Usually under dashboard on driver's side
- May be behind a small cover
- Consult manual if you can't find it

## Using the Scanner

### 1. Connect Scanner
- Turn ignition to ON (don't start engine)
- Plug scanner into OBD2 port
- Turn on scanner

### 2. Read Codes
- Select 'Read Codes' or similar option
- Wait for scan to complete
- Note any error codes (P0XXX format)

### 3. Interpret Codes
- Look up codes online or in manual
- P0XXX codes are powertrain related
- B0XXX are body codes
- C0XXX are chassis codes

## Common Codes
- P0171: System too lean
- P0300: Random misfire
- P0420: Catalyst efficiency below threshold
- P0128: Coolant thermostat

## Next Steps
- Research specific codes
- Check for simple fixes first
- Consult professional if needed
- Clear codes after repairs",
                  Tags = new[] { "diagnostics", "obd2", "troubleshooting", "beginner" },
                  Prerequisites = new[] { "Basic understanding of car systems" },
                  Tools = new[] { "OBD2 scanner", "Smartphone (optional)" }
            },
            new { Title = "Tire Rotation and Balancing", Category = "Maintenance", Difficulty = GuideDifficulty.Beginner, Minutes = 60,
                  Summary = "Extend tire life with proper rotation and balancing techniques.",
                  Content = @"# Tire Rotation and Balancing

## Why Rotate Tires?
- Even wear patterns
- Extended tire life
- Better traction
- Improved fuel economy

## Rotation Patterns

### Front-Wheel Drive
- Front tires move straight back
- Rear tires cross to front

### Rear-Wheel Drive
- Rear tires move straight forward
- Front tires cross to rear

### All-Wheel Drive
- Use X-pattern rotation

## Tools Needed
- Jack and jack stands
- Lug wrench
- Torque wrench
- Tire pressure gauge

## Step-by-Step Process

### 1. Preparation
- Check tire pressure when cold
- Mark tire positions
- Gather tools

### 2. Lifting Vehicle
- Loosen all lug nuts slightly
- Jack up vehicle safely
- Use jack stands for safety

### 3. Rotation Process
- Remove all wheels
- Follow appropriate rotation pattern
- Check tires for damage

### 4. Reinstallation
- Mount tires in new positions
- Tighten lug nuts to specification
- Lower vehicle

### 5. Final Checks
- Set proper tire pressure
- Test drive at low speed
- Check for vibrations

## When to Balance
- New tires
- After rotation if vibration occurs
- Every 6,000-8,000 miles

## Professional Balancing
- Requires special equipment
- Weights added to rim
- Eliminates vibrations",
                  Tags = new[] { "tires", "rotation", "maintenance", "beginner" },
                  Prerequisites = new[] { "Basic tool knowledge" },
                  Tools = new[] { "Jack", "Lug wrench", "Torque wrench", "Tire gauge" }
            },
            new { Title = "Car Detailing Like a Pro", Category = "Detailing", Difficulty = GuideDifficulty.Intermediate, Minutes = 180,
                  Summary = "Professional car detailing techniques for a showroom finish.",
                  Content = @"# Professional Car Detailing Guide

## Supplies Needed
- Car wash soap
- Microfiber towels
- Clay bar kit
- Polish and wax
- Interior cleaner
- Vacuum cleaner
- Detailing brushes

## Exterior Detailing

### 1. Pre-Wash
- Rinse car thoroughly
- Remove loose dirt and debris
- Work from top to bottom

### 2. Washing
- Use two-bucket method
- Wash in shade
- Start from top, work down
- Rinse frequently

### 3. Clay Bar Treatment
- Spray clay lubricant
- Gently rub clay bar
- Removes embedded contaminants
- Work in small sections

### 4. Polishing
- Apply polish with microfiber
- Work in circular motions
- Remove oxidation and scratches
- Buff to shine

### 5. Waxing
- Apply thin, even coat
- Let haze over
- Buff with clean microfiber
- Provides protection

## Interior Detailing

### 1. Remove Everything
- Take out floor mats
- Remove personal items
- Clear all compartments

### 2. Vacuum Thoroughly
- Seats and cushions
- Floor and carpets
- Dashboard and vents
- Use attachments for crevices

### 3. Clean Surfaces
- Use appropriate cleaners
- Leather needs special care
- Don't forget cup holders
- Clean windows inside

### 4. Protect and Condition
- Apply protectant to dashboard
- Condition leather seats
- Replace air freshener

## Pro Tips
- Work in shade
- Keep surfaces cool
- Use quality products
- Take your time",
                  Tags = new[] { "detailing", "cleaning", "appearance", "intermediate" },
                  Prerequisites = new[] { "Patience", "Attention to detail" },
                  Tools = new[] { "Microfiber towels", "Clay bar", "Polish", "Wax", "Vacuum" }
            },
            new { Title = "Winter Car Preparation", Category = "Seasonal", Difficulty = GuideDifficulty.Beginner, Minutes = 120,
                  Summary = "Prepare your vehicle for winter driving conditions.",
                  Content = @"# Winter Car Preparation Guide

## Essential Winter Preparations

### 1. Tires
- Switch to winter tires if needed
- Check tread depth (4/32"" minimum)
- Maintain proper pressure (cold weather reduces pressure)
- Consider tire chains for severe conditions

### 2. Battery
- Test battery capacity
- Clean terminals
- Check charging system
- Cold weather reduces battery power by 30-50%

### 3. Fluids
- Use winter-grade oil
- Check antifreeze concentration
- Top off windshield washer fluid (winter formula)
- Check brake fluid

### 4. Heating System
- Test heater and defroster
- Replace cabin air filter
- Check coolant level
- Inspect hoses for leaks

### 5. Lights
- Clean all lights regularly
- Replace burned-out bulbs
- Consider LED upgrades
- Keep spare bulbs

## Emergency Kit
- Blanket and warm clothes
- Flashlight and batteries
- First aid kit
- Jumper cables
- Ice scraper and snow brush
- Sand or cat litter for traction
- Emergency food and water

## Driving Tips
- Allow extra time
- Increase following distance
- Accelerate and brake gently
- Keep gas tank at least half full
- Clear all snow from vehicle

## Monthly Checks
- Battery terminals
- Tire pressure
- Fluid levels
- Wiper blades
- Emergency kit supplies",
                  Tags = new[] { "winter", "preparation", "safety", "seasonal" },
                  Prerequisites = new[] { "Basic car knowledge" },
                  Tools = new[] { "Ice scraper", "Jumper cables", "Emergency kit" }
            }
        };

        // Create guides from sample data
        foreach (var data in guideData)
        {
            var randomAuthor = users[random.Next(users.Count)];
            var guide = new Guide(data.Title, data.Content, randomAuthor.Id, data.Summary, data.Category, data.Difficulty, data.Minutes);

            // Add tags
            foreach (var tag in data.Tags)
            {
                guide.AddTag(tag);
            }

            // Add prerequisites
            foreach (var prerequisite in data.Prerequisites)
            {
                guide.AddPrerequisite(prerequisite);
            }

            // Add required tools
            foreach (var tool in data.Tools)
            {
                guide.AddRequiredTool(tool);
            }

            // Publish the guide
            guide.Publish();

            // 30% chance to be verified
            if (random.Next(100) < 30)
            {
                guide.Verify();
            }

            // 10% chance to be featured (only if verified)
            if (guide.IsVerified && random.Next(100) < 10)
            {
                guide.Feature();
            }

            // Add some random engagement
            var viewCount = random.Next(50, 500);
            for (int i = 0; i < viewCount; i++)
            {
                guide.IncrementViewCount();
            }

            var bookmarkCount = random.Next(5, 50);
            for (int i = 0; i < bookmarkCount; i++)
            {
                guide.IncrementBookmarkCount();
            }

            // Add some ratings
            var ratingCount = random.Next(3, 20);
            for (int i = 0; i < ratingCount; i++)
            {
                var rating = random.Next(3, 6); // 3-5 stars
                guide.AddRating(rating);
            }

            guides.Add(guide);
        }

        // Create additional random guides
        var additionalGuideTopics = new[]
        {
            "Spark Plug Replacement", "Air Filter Change", "Transmission Fluid Check",
            "Coolant System Flush", "Timing Belt Replacement", "Alternator Testing",
            "Starter Motor Diagnosis", "Fuel Pump Replacement", "Suspension Check",
            "Wheel Alignment Basics", "Paint Touch-Up", "Headlight Restoration",
            "Battery Replacement", "Fuse Box Guide", "Emergency Repairs"
        };

        var categories = new[] { "Maintenance", "Repair", "Diagnostics", "Detailing", "Performance", "Safety" };
        var difficulties = new[] { GuideDifficulty.Beginner, GuideDifficulty.Intermediate, GuideDifficulty.Advanced };

        for (int i = 0; i < 15; i++)
        {
            var topic = additionalGuideTopics[i];
            var category = categories[random.Next(categories.Length)];
            var difficulty = difficulties[random.Next(difficulties.Length)];
            var author = users[random.Next(users.Count)];
            var minutes = random.Next(30, 180);

            var content = $@"# {topic}

## Overview
This guide covers {topic.ToLower()} for your vehicle.

## What You'll Need
- Basic tools
- Safety equipment
- Replacement parts (if needed)

## Step-by-Step Instructions

### Step 1: Preparation
Prepare your workspace and gather all necessary tools.

### Step 2: Assessment
Assess the current condition and identify what needs to be done.

### Step 3: Execution
Follow the proper procedure for {topic.ToLower()}.

### Step 4: Testing
Test the results and ensure everything is working properly.

### Step 5: Cleanup
Clean up your workspace and dispose of materials properly.

## Safety Notes
- Always prioritize safety
- Use proper protective equipment
- Consult a professional if unsure

## Conclusion
Following this guide will help you successfully complete {topic.ToLower()}.";

            var guide = new Guide(topic, content, author.Id, $"Learn how to perform {topic.ToLower()} on your vehicle.", category, difficulty, minutes);

            // Add some random tags
            var commonTags = new[] { "diy", "maintenance", "repair", "automotive", "tutorial" };
            var tagCount = random.Next(2, 4);
            for (int j = 0; j < tagCount; j++)
            {
                guide.AddTag(commonTags[random.Next(commonTags.Length)]);
            }

            // 70% chance to publish
            if (random.Next(100) < 70)
            {
                guide.Publish();

                // 20% chance to be verified if published
                if (random.Next(100) < 20)
                {
                    guide.Verify();
                }
            }

            // Add some engagement
            var views = random.Next(10, 200);
            for (int j = 0; j < views; j++)
            {
                guide.IncrementViewCount();
            }

            guides.Add(guide);
        }

        await _context.Guides.AddRangeAsync(guides);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Successfully seeded {guides.Count} guides.");
    }

    private async Task SeedNewsAsync()
    {
        if (await _context.News.AnyAsync()) return;

        _logger.LogInformation("Seeding news articles...");

        var users = await _userManager.Users.ToListAsync();
        var newsItems = new List<NewsItem>();
        var random = new Random();

        // Create news seed data
        var newsDataList = new List<object>();
        
        // Add Tesla news
        newsDataList.Add(new { 
            Headline = "Tesla Announces Revolutionary Battery Technology", 
            Category = NewsCategory.Technology,
            Summary = "Tesla unveils new 4680 battery cells that promise 50% more range and faster charging times.",
            Body = @"Tesla has announced a breakthrough in battery technology with their new 4680 battery cells. These cylindrical cells are five times larger than the current 2170 cells and promise significant improvements in energy density and manufacturing efficiency.

The new batteries feature a tabless design that reduces heat generation and allows for faster charging. According to Tesla's engineering team, vehicles equipped with these batteries will see up to 50% more range compared to current models.

Manufacturing of the new batteries will begin at Tesla's Gigafactory in Texas, with production expected to ramp up throughout 2024. The company plans to integrate these batteries into the Model S, Model 3, Model X, and Model Y lineups.

Industry experts believe this advancement could accelerate the adoption of electric vehicles by addressing two major consumer concerns: range anxiety and charging time. The new technology is expected to reduce charging time from 30 minutes to under 15 minutes for an 80% charge.

Tesla's stock price surged 8% following the announcement, reflecting investor confidence in the company's continued innovation in the electric vehicle space.",
            CarMake = "Tesla",
            CarModel = "Model S",
            CarYear = 2024,
            Tags = new[] { "tesla", "battery", "technology", "electric-vehicles", "innovation" },
            Source = "Tesla Press Release",
            SourceUrl = "https://tesla.com/news",
            ImageUrl = "https://images.unsplash.com/photo-1560958089-b8a1929cea89?w=800&h=400&fit=crop"
        });

        // Add Ford news
        newsDataList.Add(new { 
            Headline = "Ford F-150 Lightning Wins Truck of the Year Award", 
            Category = NewsCategory.CarReviews,
            Summary = "The electric F-150 Lightning receives prestigious award for innovation and performance.",
            Body = @"The Ford F-150 Lightning has been named Truck of the Year by Motor Trend magazine, marking a historic moment as the first electric pickup to receive this prestigious award.

The Lightning impressed judges with its impressive towing capacity of 10,000 pounds, 320-mile range, and innovative features like the ability to power a home during outages. The truck's Pro Power Onboard system can provide up to 9.6kW of electricity, enough to run most homes for three days.

'The F-150 Lightning represents the future of pickup trucks,' said Motor Trend's editor-in-chief. 'It delivers all the capability truck owners expect while adding the benefits of electric propulsion.'

Ford has received over 200,000 reservations for the Lightning since its announcement. The company is ramping up production at its Rouge Electric Vehicle Center in Michigan to meet demand.

The Lightning starts at $39,974 for the regular cab work truck and goes up to $90,874 for the fully loaded Platinum model. Ford expects to deliver 150,000 units in the first full year of production.",
            CarMake = "Ford",
            CarModel = "F-150 Lightning",
            CarYear = 2024,
            Tags = new[] { "ford", "f150", "electric-truck", "award", "motor-trend" },
            Source = "Motor Trend",
            SourceUrl = "https://motortrend.com",
            ImageUrl = "https://images.unsplash.com/photo-1605559424843-9e4c228bf1c2?w=800&h=400&fit=crop"
        });

        // Add industry news without car info
        newsDataList.Add(new { 
            Headline = "Global Chip Shortage Continues to Impact Auto Industry", 
            Category = NewsCategory.Industry,
            Summary = "Semiconductor shortage forces major automakers to adjust production schedules and prioritize high-margin vehicles.",
            Body = @"The global semiconductor shortage continues to disrupt automotive production, with major manufacturers reporting significant impacts on their 2024 production schedules.

General Motors announced it will temporarily halt production at three North American plants, affecting popular models including the Chevrolet Silverado and GMC Sierra. The company expects the shortage to reduce production by approximately 200,000 vehicles this quarter.

Ford has implemented a strategy of building vehicles without certain chips and storing them until semiconductors become available. The company has thousands of F-150 trucks waiting for chips at various facilities.

'We're prioritizing our most profitable vehicles and working closely with suppliers to secure chip allocations,' said Ford's chief operating officer. The company is also investing in direct relationships with chip manufacturers to reduce future supply chain risks.

The shortage, which began during the COVID-19 pandemic, has been exacerbated by increased demand for consumer electronics and geopolitical tensions affecting chip production in Asia. Industry analysts expect the situation to gradually improve throughout 2024, but full recovery may not occur until 2025.",
            CarMake = (string?)null,
            CarModel = (string?)null,
            CarYear = (int?)null,
            Tags = new[] { "chip-shortage", "production", "supply-chain", "automotive-industry" },
            Source = "Automotive News",
            SourceUrl = "https://autonews.com",
            ImageUrl = "https://images.unsplash.com/photo-1518709268805-4e9042af2176?w=800&h=400&fit=crop"
        });

        // Create news items from sample data
        foreach (dynamic data in newsDataList)
        {
            var randomAuthor = users[random.Next(users.Count)];
            var newsItem = new NewsItem(data.Headline, data.Body, randomAuthor.Id, data.Category);

            // Set summary
            if (!string.IsNullOrEmpty(data.Summary))
            {
                newsItem.UpdateContent(data.Headline, data.Body, data.Summary);
            }

            // Set car information
            if (!string.IsNullOrEmpty(data.CarMake))
            {
                newsItem.SetCarInfo(data.CarMake, data.CarModel, data.CarYear);
            }

            // Set source information
            if (!string.IsNullOrEmpty(data.Source))
            {
                newsItem.SetSource(data.Source, data.SourceUrl);
            }

            // Set main image
            if (!string.IsNullOrEmpty(data.ImageUrl))
            {
                newsItem.SetMainImage(data.ImageUrl);
            }

            // Add tags
            foreach (var tag in data.Tags)
            {
                newsItem.AddTag(tag);
            }

            // Set SEO data
            newsItem.UpdateSeoData(
                $"{data.Headline} - Community Car News",
                data.Summary
            );

            // Publish the news item
            newsItem.Publish();

            // 20% chance to be featured
            if (random.Next(100) < 20)
            {
                newsItem.SetFeatured(true);
            }

            // 10% chance to be pinned
            if (random.Next(100) < 10)
            {
                newsItem.SetPinned(true);
            }

            // Add random engagement
            var viewCount = random.Next(100, 2000);
            for (int i = 0; i < viewCount; i++)
            {
                newsItem.IncrementViewCount();
            }

            var likeCount = random.Next(10, 150);
            for (int i = 0; i < likeCount; i++)
            {
                newsItem.IncrementLikeCount();
            }

            var commentCount = random.Next(5, 50);
            for (int i = 0; i < commentCount; i++)
            {
                newsItem.IncrementCommentCount();
            }

            var shareCount = random.Next(2, 25);
            for (int i = 0; i < shareCount; i++)
            {
                newsItem.IncrementShareCount();
            }

            newsItems.Add(newsItem);
        }

        // Create additional random news items
        var additionalHeadlines = new[]
        {
            "Volkswagen ID.4 Wins World Car of the Year",
            "Rivian Announces New R1S Electric SUV Features",
            "Lucid Air Sets New EV Range Record",
            "Hyundai Reveals Hydrogen-Powered NEXO Updates",
            "Audi e-tron GT Receives Performance Upgrade",
            "Chevrolet Bolt EV Gets Price Reduction",
            "Nissan Ariya Electric Crossover Launches",
            "Jaguar I-PACE Receives Software Update",
            "Volvo XC40 Recharge Expands to New Markets",
            "Genesis Electrified GV70 Debuts",
            "Cadillac Lyriq Production Begins",
            "Polestar 3 SUV Unveiled",
            "Subaru Solterra Electric SUV Announced",
            "Mazda MX-30 EV Arrives in US",
            "Acura Integra Returns with Hybrid Power"
        };

        var categories = new[] { NewsCategory.General, NewsCategory.CarReviews, NewsCategory.Industry, NewsCategory.Technology, NewsCategory.Events, NewsCategory.Tips };
        var carMakes = new[] { "Toyota", "Honda", "Ford", "BMW", "Mercedes", "Audi", "Volkswagen", "Nissan", "Hyundai", "Kia", "Tesla", "Rivian", "Lucid", "Polestar" };
        var carModels = new[] { "Model 3", "Civic", "F-150", "3 Series", "C-Class", "A4", "Golf", "Altima", "Elantra", "Forte", "Model Y", "R1T", "Air", "2" };

        for (int i = 0; i < 15; i++)
        {
            var headline = additionalHeadlines[i];
            var category = categories[random.Next(categories.Length)];
            var author = users[random.Next(users.Count)];
            var carMake = carMakes[random.Next(carMakes.Length)];
            var carModel = carModels[random.Next(carModels.Length)];
            var carYear = random.Next(2022, 2026);

            var body = $@"This is a news article about {headline.ToLower()}. 

The automotive industry continues to evolve with new technologies and innovations. This particular development represents an important milestone in the ongoing transformation of the automotive landscape.

Key highlights include:
- Advanced technology integration
- Improved performance and efficiency  
- Enhanced user experience
- Competitive market positioning
- Future development roadmap

Industry experts believe this announcement will have significant implications for the automotive market. Consumers can expect to see these innovations reflected in upcoming vehicle releases.

The company plans to expand this technology across their vehicle lineup over the next several years. Additional details will be announced at upcoming automotive events and press conferences.

This development underscores the rapid pace of change in the automotive industry and the increasing focus on sustainable transportation solutions.";

            var newsItem = new NewsItem(headline, body, author.Id, category);
            
            // Set summary
            newsItem.UpdateContent(headline, body, $"Latest news about {headline.ToLower()} and its impact on the automotive industry.");

            // 70% chance to set car info
            if (random.Next(100) < 70)
            {
                newsItem.SetCarInfo(carMake, carModel, carYear);
            }

            // Set source
            var sources = new[] { "Automotive News", "Car and Driver", "Motor Trend", "Road & Track", "AutoWeek" };
            var source = sources[random.Next(sources.Length)];
            newsItem.SetSource(source, $"https://{source.ToLower().Replace(" ", "").Replace("&", "and")}.com");

            // Add random tags
            var commonTags = new[] { "automotive", "news", "cars", "technology", "industry", "electric", "hybrid", "performance" };
            var tagCount = random.Next(3, 6);
            for (int j = 0; j < tagCount; j++)
            {
                var tag = commonTags[random.Next(commonTags.Length)];
                newsItem.AddTag(tag);
            }

            // Publish the news item
            newsItem.Publish();

            // Add some engagement
            var views = random.Next(50, 500);
            for (int j = 0; j < views; j++)
            {
                newsItem.IncrementViewCount();
            }

            var likes = random.Next(5, 50);
            for (int j = 0; j < likes; j++)
            {
                newsItem.IncrementLikeCount();
            }

            newsItems.Add(newsItem);
        }

        await _context.News.AddRangeAsync(newsItems);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Successfully seeded {newsItems.Count} news articles.");
    }

    private async Task SeedReviewsAsync()
    {
        if (await _context.Reviews.AnyAsync()) return;

        _logger.LogInformation("Seeding reviews...");

        var users = await _userManager.Users.ToListAsync();
        var reviews = new List<CommunityCar.Domain.Entities.Community.Reviews.Review>();
        var random = new Random();

        // Sample review data
        var reviewData = new[]
        {
            new {
                Title = "Excellent Electric Vehicle Experience",
                Comment = @"I've owned this Tesla Model 3 for over a year now, and it has exceeded all my expectations. The acceleration is incredible, the autopilot features work flawlessly on highways, and the over-the-air updates keep improving the car. 

The build quality is solid, and I haven't had any major issues. The charging network is extensive, making long trips worry-free. The interior is minimalist but functional, though some might miss physical buttons.

Overall, this is the best car I've ever owned. The combination of performance, technology, and environmental benefits makes it a clear winner.",
                Rating = 5,
                CarMake = "Tesla",
                CarModel = "Model 3",
                CarYear = 2023,
                IsVerifiedPurchase = true,
                IsRecommended = true,
                Mileage = 15000,
                OwnershipDuration = "1 year",
                QualityRating = 5,
                ValueRating = 4,
                ReliabilityRating = 5,
                PerformanceRating = 5,
                ComfortRating = 4,
                Pros = new[] { "Incredible acceleration", "Advanced autopilot", "Over-the-air updates", "Excellent charging network", "Zero emissions" },
                Cons = new[] { "Minimalist interior might not appeal to everyone", "Road noise at highway speeds" },
                ImageUrls = new[] { "https://images.unsplash.com/photo-1560958089-b8a1929cea89?w=800&h=400&fit=crop" }
            },
            new {
                Title = "Reliable Daily Driver with Great Fuel Economy",
                Comment = @"My Honda Civic has been a fantastic daily driver for the past three years. It's incredibly reliable, fuel-efficient, and comfortable for both city and highway driving.

The interior is well-designed with intuitive controls and good build quality. The infotainment system works well, though it could be more responsive. The trunk space is adequate for a compact car.

Maintenance costs have been very reasonable, and I've had no major repairs. The CVT transmission is smooth, though some might find it less engaging than a traditional automatic.

For the price point, this car offers excellent value and I would definitely recommend it to anyone looking for a practical, efficient vehicle.",
                Rating = 4,
                CarMake = "Honda",
                CarModel = "Civic",
                CarYear = 2021,
                IsVerifiedPurchase = true,
                IsRecommended = true,
                Mileage = 45000,
                OwnershipDuration = "3 years",
                QualityRating = 4,
                ValueRating = 5,
                ReliabilityRating = 5,
                PerformanceRating = 3,
                ComfortRating = 4,
                Pros = new[] { "Excellent fuel economy", "Very reliable", "Comfortable interior", "Low maintenance costs", "Good resale value" },
                Cons = new[] { "CVT can feel sluggish", "Road noise on rough surfaces", "Rear seat space could be better" },
                ImageUrls = new[] { "https://images.unsplash.com/photo-1605559424843-9e4c228bf1c2?w=800&h=400&fit=crop" }
            },
            new {
                Title = "Powerful Truck with Some Drawbacks",
                Comment = @"The F-150 is undoubtedly a capable truck with impressive towing capacity and a comfortable cabin. The EcoBoost engine provides good power and reasonable fuel economy for a full-size truck.

The interior is spacious and well-appointed, with plenty of storage options. The infotainment system is user-friendly, and the driver assistance features work well.

However, I've experienced some reliability issues with the turbocharger, and the repair costs were significant. The ride quality can be harsh when the truck is unloaded, and fuel economy drops significantly when towing.

Despite these issues, it's still a solid choice for those who need a capable work truck, but I'd recommend considering the warranty carefully.",
                Rating = 3,
                CarMake = "Ford",
                CarModel = "F-150",
                CarYear = 2020,
                IsVerifiedPurchase = true,
                IsRecommended = false,
                Mileage = 65000,
                OwnershipDuration = "2.5 years",
                QualityRating = 3,
                ValueRating = 3,
                ReliabilityRating = 2,
                PerformanceRating = 4,
                ComfortRating = 4,
                Pros = new[] { "Excellent towing capacity", "Spacious cabin", "Good infotainment system", "Strong engine options" },
                Cons = new[] { "Reliability concerns with turbo", "High repair costs", "Poor fuel economy when towing", "Harsh ride when unloaded" },
                ImageUrls = new[] { "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=800&h=400&fit=crop" }
            },
            new {
                Title = "Luxury and Performance Combined",
                Comment = @"The BMW 3 Series delivers an exceptional driving experience with its perfect balance of luxury and performance. The handling is precise, the engine is smooth and powerful, and the interior quality is top-notch.

The iDrive system has improved significantly and is now intuitive to use. The seats are comfortable for long drives, and the build quality feels solid throughout.

However, maintenance costs are high, and some features that should be standard are expensive options. The run-flat tires are noisy and expensive to replace.

If you can afford the premium maintenance costs, this is an excellent luxury sport sedan that delivers on BMW's promise of the ultimate driving machine.",
                Rating = 4,
                CarMake = "BMW",
                CarModel = "3 Series",
                CarYear = 2022,
                IsVerifiedPurchase = true,
                IsRecommended = true,
                Mileage = 25000,
                OwnershipDuration = "1.5 years",
                QualityRating = 5,
                ValueRating = 3,
                ReliabilityRating = 4,
                PerformanceRating = 5,
                ComfortRating = 5,
                Pros = new[] { "Excellent handling", "Powerful engine", "Luxury interior", "Advanced technology", "Strong performance" },
                Cons = new[] { "High maintenance costs", "Expensive options", "Noisy run-flat tires", "Premium fuel required" },
                ImageUrls = new[] { "https://images.unsplash.com/photo-1555215695-3004980ad54e?w=800&h=400&fit=crop" }
            },
            new {
                Title = "Great Value SUV for Families",
                Comment = @"The Hyundai Tucson has been an excellent family vehicle. It offers a good balance of features, comfort, and value. The interior is spacious enough for our family of four, and the cargo area is adequate for our needs.

The infotainment system is easy to use, and the warranty coverage provides peace of mind. Fuel economy is decent for an SUV, and the ride quality is comfortable on most road surfaces.

The styling is modern and attractive, and we've received many compliments on the design. Build quality seems solid so far, though it's still relatively new to us.

For the price, this SUV offers a lot of value and would be a good choice for families looking for a reliable, well-equipped vehicle.",
                Rating = 4,
                CarMake = "Hyundai",
                CarModel = "Tucson",
                CarYear = 2023,
                IsVerifiedPurchase = true,
                IsRecommended = true,
                Mileage = 8000,
                OwnershipDuration = "8 months",
                QualityRating = 4,
                ValueRating = 5,
                ReliabilityRating = 4,
                PerformanceRating = 3,
                ComfortRating = 4,
                Pros = new[] { "Great value for money", "Spacious interior", "Good warranty", "Modern styling", "User-friendly tech" },
                Cons = new[] { "Engine could be more powerful", "Some interior materials feel cheap", "Road noise at highway speeds" },
                ImageUrls = new[] { "https://images.unsplash.com/photo-1549317661-bd32c8ce0db2?w=800&h=400&fit=crop" }
            }
        };

        // Create reviews from sample data
        foreach (var data in reviewData)
        {
            var randomReviewer = users[random.Next(users.Count)];
            var targetId = Guid.NewGuid(); // In a real scenario, this would be a vehicle ID
            
            var review = new CommunityCar.Domain.Entities.Community.Reviews.Review(
                targetId, "Vehicle", data.Rating, data.Title, data.Comment, randomReviewer.Id);

            // Set purchase info
            review.SetPurchaseInfo(data.IsVerifiedPurchase, DateTime.Now.AddMonths(-random.Next(6, 36)), random.Next(20000, 80000));
            review.SetRecommendation(data.IsRecommended);
            review.SetCarInfo(data.CarMake, data.CarModel, data.CarYear, data.Mileage, data.OwnershipDuration);
            review.SetDetailedRatings(data.QualityRating, data.ValueRating, data.ReliabilityRating, data.PerformanceRating, data.ComfortRating);

            // Add images
            foreach (var imageUrl in data.ImageUrls)
            {
                review.AddImage(imageUrl);
            }

            // Add pros
            foreach (var pro in data.Pros)
            {
                review.AddPro(pro);
            }

            // Add cons
            foreach (var con in data.Cons)
            {
                review.AddCon(con);
            }

            // Approve the review
            review.Approve();

            // Add some engagement
            var viewCount = random.Next(50, 500);
            for (int i = 0; i < viewCount; i++)
            {
                review.IncrementViewCount();
            }

            var helpfulCount = random.Next(5, 50);
            for (int i = 0; i < helpfulCount; i++)
            {
                review.IncrementHelpfulCount();
            }

            reviews.Add(review);
        }

        // Create additional random reviews
        var carMakes = new[] { "Toyota", "Honda", "Ford", "BMW", "Mercedes", "Audi", "Volkswagen", "Nissan", "Hyundai", "Kia", "Mazda", "Subaru", "Chevrolet", "Jeep", "Volvo" };
        var carModels = new[] { "Sedan", "SUV", "Hatchback", "Coupe", "Wagon", "Crossover", "Pickup", "Convertible" };
        var reviewTitles = new[]
        {
            "Great car for the money", "Disappointed with reliability", "Excellent fuel economy", "Comfortable and spacious",
            "Poor build quality", "Outstanding performance", "Good value proposition", "Maintenance nightmare",
            "Perfect family car", "Not worth the premium", "Reliable daily driver", "Impressive technology features",
            "Overpriced for what you get", "Exceeded expectations", "Solid choice for commuting"
        };

        for (int i = 0; i < 25; i++)
        {
            var randomReviewer = users[random.Next(users.Count)];
            var targetId = Guid.NewGuid();
            var rating = random.Next(1, 6);
            var title = reviewTitles[random.Next(reviewTitles.Length)];
            var carMake = carMakes[random.Next(carMakes.Length)];
            var carModel = carModels[random.Next(carModels.Length)];
            var carYear = random.Next(2018, 2025);

            var comment = GenerateRandomReviewComment(rating, carMake, carModel);

            var review = new CommunityCar.Domain.Entities.Community.Reviews.Review(
                targetId, "Vehicle", rating, title, comment, randomReviewer.Id);

            // Set car info
            review.SetCarInfo(carMake, carModel, carYear, random.Next(5000, 100000), $"{random.Next(6, 60)} months");

            // 70% chance of verified purchase
            if (random.Next(100) < 70)
            {
                review.SetPurchaseInfo(true, DateTime.Now.AddMonths(-random.Next(1, 36)), random.Next(15000, 75000));
            }

            // 60% chance of recommendation for ratings 4-5, 20% for rating 3, 5% for ratings 1-2
            var recommendChance = rating >= 4 ? 60 : rating == 3 ? 20 : 5;
            if (random.Next(100) < recommendChance)
            {
                review.SetRecommendation(true);
            }

            // Add detailed ratings (50% chance)
            if (random.Next(100) < 50)
            {
                review.SetDetailedRatings(
                    random.Next(rating - 1, rating + 2),
                    random.Next(rating - 1, rating + 2),
                    random.Next(rating - 1, rating + 2),
                    random.Next(rating - 1, rating + 2),
                    random.Next(rating - 1, rating + 2)
                );
            }

            // Add some pros and cons (30% chance)
            if (random.Next(100) < 30)
            {
                var commonPros = new[] { "Good fuel economy", "Comfortable seats", "Reliable", "Good value", "Easy to drive", "Spacious interior" };
                var commonCons = new[] { "Road noise", "Poor infotainment", "Expensive maintenance", "Limited cargo space", "Harsh ride", "Cheap interior materials" };

                var prosCount = random.Next(1, 4);
                for (int j = 0; j < prosCount; j++)
                {
                    review.AddPro(commonPros[random.Next(commonPros.Length)]);
                }

                var consCount = random.Next(1, 3);
                for (int j = 0; j < consCount; j++)
                {
                    review.AddCon(commonCons[random.Next(commonCons.Length)]);
                }
            }

            // Approve most reviews (90%)
            if (random.Next(100) < 90)
            {
                review.Approve();
            }

            // Add engagement
            var views = random.Next(10, 200);
            for (int j = 0; j < views; j++)
            {
                review.IncrementViewCount();
            }

            var helpful = random.Next(0, 20);
            for (int j = 0; j < helpful; j++)
            {
                review.IncrementHelpfulCount();
            }

            reviews.Add(review);
        }

        await _context.Reviews.AddRangeAsync(reviews);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Successfully seeded {reviews.Count} reviews.");
    }

    private async Task SeedMapsAsync()
    {
        if (await _context.PointsOfInterest.AnyAsync()) return;

        _logger.LogInformation("Seeding maps data...");

        var users = await _userManager.Users.ToListAsync();
        var pointsOfInterest = new List<CommunityCar.Domain.Entities.Community.Maps.PointOfInterest>();
        var routes = new List<CommunityCar.Domain.Entities.Community.Maps.Route>();
        var checkIns = new List<CommunityCar.Domain.Entities.Community.Maps.CheckIn>();
        var random = new Random();

        // Sample POI data for automotive locations
        var poiData = new[]
        {
            new {
                Name = "AutoZone Downtown",
                Description = "Full-service auto parts store with knowledgeable staff and wide selection of automotive parts and accessories.",
                Latitude = 30.2672,
                Longitude = -97.7431,
                Type = POIType.AutoPartsStore,
                Category = POICategory.Automotive,
                Address = "123 Main St, Austin, TX 78701",
                PhoneNumber = "+1-512-555-0101",
                Website = "https://autozone.com",
                Services = new[] { "Battery Testing", "Oil Recycling", "Tool Rental", "Parts Lookup" },
                PaymentMethods = new[] { "Cash", "Credit Card", "Debit Card" },
                OpeningHours = "Mon-Sat: 7:30 AM - 10:00 PM, Sun: 9:00 AM - 9:00 PM",
                IsOpen24Hours = false
            },
            new {
                Name = "Circuit of The Americas",
                Description = "World-class racing facility hosting Formula 1, MotoGP, and other major motorsport events.",
                Latitude = 30.1328,
                Longitude = -97.6411,
                Type = POIType.RaceTrack,
                Category = POICategory.Recreation,
                Address = "9201 Circuit of The Americas Blvd, Austin, TX 78617",
                PhoneNumber = "+1-512-655-6300",
                Website = "https://circuitoftheamericas.com",
                Services = new[] { "Track Days", "Driving Experiences", "Racing Events", "Corporate Events" },
                PaymentMethods = new[] { "Credit Card", "Debit Card", "Online Payment" },
                OpeningHours = "Event dependent - check website",
                IsOpen24Hours = false
            },
            new {
                Name = "Tesla Supercharger Station",
                Description = "High-speed charging station for Tesla vehicles with 12 charging stalls and amenities nearby.",
                Latitude = 30.2849,
                Longitude = -97.7341,
                Type = POIType.ChargingStation,
                Category = POICategory.Automotive,
                Address = "2700 W Anderson Ln, Austin, TX 78757",
                PhoneNumber = "+1-877-798-3752",
                Website = "https://tesla.com/supercharger",
                Services = new[] { "Supercharging", "Destination Charging", "Mobile Connector" },
                PaymentMethods = new[] { "Tesla Account", "Credit Card" },
                OpeningHours = "24/7",
                IsOpen24Hours = true
            },
            new {
                Name = "Jiffy Lube Express",
                Description = "Quick oil change and automotive maintenance services with no appointment necessary.",
                Latitude = 30.3077,
                Longitude = -97.7272,
                Type = POIType.AutoRepairShop,
                Category = POICategory.Automotive,
                Address = "1234 North Lamar Blvd, Austin, TX 78756",
                PhoneNumber = "+1-512-555-0202",
                Website = "https://jiffylube.com",
                Services = new[] { "Oil Change", "Filter Replacement", "Fluid Top-off", "Battery Check" },
                PaymentMethods = new[] { "Cash", "Credit Card", "Debit Card", "Mobile Pay" },
                OpeningHours = "Mon-Sat: 8:00 AM - 7:00 PM, Sun: 10:00 AM - 5:00 PM",
                IsOpen24Hours = false
            },
            new {
                Name = "BMW of Austin",
                Description = "Authorized BMW dealership offering new and certified pre-owned vehicles, service, and parts.",
                Latitude = 30.2240,
                Longitude = -97.8089,
                Type = POIType.CarDealership,
                Category = POICategory.Automotive,
                Address = "7011 McNeil Dr, Austin, TX 78729",
                PhoneNumber = "+1-512-343-3500",
                Website = "https://bmwofaustin.com",
                Services = new[] { "New Car Sales", "Used Car Sales", "Service & Repair", "Parts & Accessories" },
                PaymentMethods = new[] { "Cash", "Credit Card", "Financing", "Lease" },
                OpeningHours = "Mon-Sat: 9:00 AM - 8:00 PM, Sun: 12:00 PM - 6:00 PM",
                IsOpen24Hours = false
            },
            new {
                Name = "Cars & Coffee Austin",
                Description = "Monthly automotive enthusiast meetup featuring classic cars, supercars, and everything in between.",
                Latitude = 30.4518,
                Longitude = -97.7987,
                Type = POIType.MeetupPoint,
                Category = POICategory.Community,
                Address = "The Domain, 11410 Century Oaks Terrace, Austin, TX 78758",
                PhoneNumber = (string?)null,
                Website = "https://carsandcoffeeaustin.com",
                Services = new[] { "Car Show", "Networking", "Photography", "Vendor Booths" },
                PaymentMethods = new[] { "Free Event" },
                OpeningHours = "First Saturday of each month: 8:00 AM - 11:00 AM",
                IsOpen24Hours = false
            },
            new {
                Name = "Discount Tire",
                Description = "America's largest independent tire and wheel retailer with expert installation and service.",
                Latitude = 30.2711,
                Longitude = -97.7436,
                Type = POIType.TireShop,
                Category = POICategory.Automotive,
                Address = "1515 S Lamar Blvd, Austin, TX 78704",
                PhoneNumber = "+1-512-555-0303",
                Website = "https://discounttire.com",
                Services = new[] { "Tire Installation", "Wheel Alignment", "Tire Rotation", "Flat Repair" },
                PaymentMethods = new[] { "Cash", "Credit Card", "Financing Available" },
                OpeningHours = "Mon-Fri: 8:00 AM - 6:00 PM, Sat: 8:00 AM - 5:00 PM",
                IsOpen24Hours = false
            },
            new {
                Name = "Hill Country Scenic Drive",
                Description = "Beautiful scenic route through Texas Hill Country with rolling hills and wildflower views.",
                Latitude = 30.0000,
                Longitude = -98.0000,
                Type = POIType.Scenic,
                Category = POICategory.Recreation,
                Address = "Ranch Road 12, Dripping Springs, TX",
                PhoneNumber = (string?)null,
                Website = (string?)null,
                Services = new[] { "Scenic Driving", "Photography", "Wildflower Viewing", "Hill Country Tours" },
                PaymentMethods = new[] { "Free Access" },
                OpeningHours = "Always open",
                IsOpen24Hours = true
            }
        };

        // Create POIs from sample data
        foreach (var data in poiData)
        {
            var randomCreator = users[random.Next(users.Count)];
            var poi = new CommunityCar.Domain.Entities.Community.Maps.PointOfInterest(
                data.Name, data.Description, data.Latitude, data.Longitude, 
                data.Type, data.Category, randomCreator.Id);

            // Set contact info
            poi.UpdateContactInfo(data.PhoneNumber, data.Website, null);
            poi.UpdateLocation(data.Latitude, data.Longitude, data.Address);
            poi.UpdateOperatingHours(data.OpeningHours, data.IsOpen24Hours);

            // Add services
            foreach (var service in data.Services)
            {
                poi.AddService(service);
            }

            // Add payment methods
            foreach (var method in data.PaymentMethods)
            {
                poi.AddPaymentMethod(method);
            }

            // Add some amenities
            var commonAmenities = new[] { "Parking", "WiFi", "Restrooms", "Waiting Area", "Coffee" };
            var amenityCount = random.Next(1, 4);
            for (int i = 0; i < amenityCount; i++)
            {
                poi.AddAmenity(commonAmenities[random.Next(commonAmenities.Length)]);
            }

            // 30% chance to be verified
            if (random.Next(100) < 30)
            {
                var verifier = users[random.Next(users.Count)];
                poi.Verify(verifier.Id);
            }

            // Add random engagement
            var viewCount = random.Next(50, 500);
            for (int i = 0; i < viewCount; i++)
            {
                poi.IncrementViewCount();
            }

            var checkInCount = random.Next(5, 50);
            for (int i = 0; i < checkInCount; i++)
            {
                poi.IncrementCheckInCount();
            }

            // Add rating
            var rating = random.NextDouble() * 2 + 3; // 3-5 stars
            var reviewCount = random.Next(5, 25);
            poi.UpdateRating(rating, reviewCount);

            pointsOfInterest.Add(poi);
        }

        // Create additional random POIs
        var additionalPOINames = new[]
        {
            "Quick Lube Express", "Midas Auto Service", "Valvoline Instant Oil Change",
            "Pep Boys Auto Parts", "O'Reilly Auto Parts", "NAPA Auto Parts",
            "Shell Gas Station", "Chevron Station", "Exxon Mobil",
            "Ford Dealership", "Toyota Center", "Honda of Austin",
            "Firestone Complete Auto Care", "Goodyear Auto Service", "NTB Tire Center",
            "CarMax Used Cars", "Carvana Vending Machine", "Enterprise Car Rental"
        };

        var poiTypes = new[] { POIType.AutoPartsStore, POIType.AutoRepairShop, POIType.GasStation, POIType.CarDealership, POIType.TireShop };
        var categories = new[] { POICategory.Automotive, POICategory.Community, POICategory.Recreation };

        // Austin area coordinates
        var austinLat = 30.2672;
        var austinLon = -97.7431;

        for (int i = 0; i < 20; i++)
        {
            var name = additionalPOINames[random.Next(additionalPOINames.Length)];
            var type = poiTypes[random.Next(poiTypes.Length)];
            var category = categories[random.Next(categories.Length)];
            var creator = users[random.Next(users.Count)];

            // Generate random coordinates around Austin
            var lat = austinLat + (random.NextDouble() - 0.5) * 0.5; // ±0.25 degrees
            var lon = austinLon + (random.NextDouble() - 0.5) * 0.5;

            var poi = new CommunityCar.Domain.Entities.Community.Maps.PointOfInterest(
                name, $"Local automotive service provider in Austin area.", 
                lat, lon, type, category, creator.Id);

            // Add some basic info
            poi.UpdateContactInfo($"+1-512-555-{random.Next(1000, 9999)}", null, null);
            poi.UpdateOperatingHours("Mon-Fri: 8:00 AM - 6:00 PM", false);

            // Add services based on type
            var services = type switch
            {
                POIType.AutoPartsStore => new[] { "Parts Sales", "Battery Testing", "Tool Rental" },
                POIType.AutoRepairShop => new[] { "Oil Change", "Brake Service", "Tire Service" },
                POIType.GasStation => new[] { "Fuel", "Car Wash", "Convenience Store" },
                POIType.CarDealership => new[] { "New Car Sales", "Used Car Sales", "Service" },
                POIType.TireShop => new[] { "Tire Installation", "Wheel Alignment", "Balancing" },
                _ => new[] { "General Service" }
            };

            foreach (var service in services)
            {
                poi.AddService(service);
            }

            // Add engagement
            var views = random.Next(10, 200);
            for (int j = 0; j < views; j++)
            {
                poi.IncrementViewCount();
            }

            pointsOfInterest.Add(poi);
        }

        await _context.PointsOfInterest.AddRangeAsync(pointsOfInterest);
        await _context.SaveChangesAsync();

        // Create sample routes
        var routeData = new[]
        {
            new {
                Name = "Hill Country Loop",
                Description = "Scenic drive through Texas Hill Country with beautiful views and winding roads perfect for sports cars.",
                Type = RouteType.Scenic,
                Difficulty = DifficultyLevel.Intermediate,
                DistanceKm = 85.3,
                DurationMinutes = 120,
                IsScenic = true,
                HasTolls = false,
                IsOffRoad = false,
                Tags = new[] { "scenic", "hill-country", "weekend-drive", "photography" }
            },
            new {
                Name = "Austin Track Day Route",
                Description = "Performance route from downtown Austin to Circuit of The Americas with highway and back road sections.",
                Type = RouteType.Performance,
                Difficulty = DifficultyLevel.Advanced,
                DistanceKm = 45.7,
                DurationMinutes = 60,
                IsScenic = false,
                HasTolls = false,
                IsOffRoad = false,
                Tags = new[] { "performance", "track-day", "cota", "racing" }
            },
            new {
                Name = "Bluebonnet Trail",
                Description = "Spring wildflower viewing route through rural Texas roads, best visited in March-April.",
                Type = RouteType.Touring,
                Difficulty = DifficultyLevel.Beginner,
                DistanceKm = 67.2,
                DurationMinutes = 90,
                IsScenic = true,
                HasTolls = false,
                IsOffRoad = false,
                Tags = new[] { "wildflowers", "spring", "family-friendly", "photography" }
            },
            new {
                Name = "Off-Road Adventure Trail",
                Description = "Challenging off-road route through Texas backcountry. 4WD vehicle recommended.",
                Type = RouteType.OffRoad,
                Difficulty = DifficultyLevel.Expert,
                DistanceKm = 32.1,
                DurationMinutes = 180,
                IsScenic = true,
                HasTolls = false,
                IsOffRoad = true,
                Tags = new[] { "off-road", "4wd", "adventure", "challenging" }
            },
            new {
                Name = "Austin Coffee Shop Cruise",
                Description = "Urban route connecting Austin's best coffee shops and car-friendly hangout spots.",
                Type = RouteType.Cruise,
                Difficulty = DifficultyLevel.Beginner,
                DistanceKm = 28.5,
                DurationMinutes = 45,
                IsScenic = false,
                HasTolls = false,
                IsOffRoad = false,
                Tags = new[] { "urban", "coffee", "social", "meetup" }
            }
        };

        foreach (var data in routeData)
        {
            var creator = users[random.Next(users.Count)];
            var route = new CommunityCar.Domain.Entities.Community.Maps.Route(
                data.Name, data.Description, creator.Id, data.Type, data.Difficulty);

            route.UpdateMetrics(data.DistanceKm, data.DurationMinutes);
            route.UpdateCharacteristics(data.IsScenic, data.HasTolls, data.IsOffRoad, null, null);

            // Add tags
            foreach (var tag in data.Tags)
            {
                route.AddTag(tag);
            }

            // Add some completions
            var completions = random.Next(5, 50);
            for (int i = 0; i < completions; i++)
            {
                route.IncrementCompletionCount();
            }

            // Add rating
            var rating = random.NextDouble() * 1.5 + 3.5; // 3.5-5 stars
            var reviewCount = random.Next(3, 15);
            route.UpdateRating(rating, reviewCount);

            routes.Add(route);
        }

        await _context.Routes.AddRangeAsync(routes);
        await _context.SaveChangesAsync();

        // Create sample check-ins
        var checkInComments = new[]
        {
            "Great service and friendly staff!",
            "Quick and efficient oil change.",
            "Love this place for car meets!",
            "Best prices in town.",
            "Clean facilities and professional service.",
            "Had a great experience here.",
            "Highly recommend this location.",
            "Fast service, will come back.",
            "Good selection of parts.",
            "Excellent customer service."
        };

        foreach (var poi in pointsOfInterest.Take(15)) // Add check-ins to first 15 POIs
        {
            var checkInCount = random.Next(3, 12);
            for (int i = 0; i < checkInCount; i++)
            {
                var user = users[random.Next(users.Count)];
                var comment = random.Next(100) < 70 ? checkInComments[random.Next(checkInComments.Length)] : null;
                var rating = random.Next(100) < 60 ? (double?)random.Next(3, 6) : null;
                var isPrivate = random.Next(100) < 20;

                var checkIn = new CommunityCar.Domain.Entities.Community.Maps.CheckIn(
                    poi.Id, user.Id, comment, rating, isPrivate);

                checkIns.Add(checkIn);
            }
        }

        await _context.CheckIns.AddRangeAsync(checkIns);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Successfully seeded {pointsOfInterest.Count} points of interest, {routes.Count} routes, and {checkIns.Count} check-ins.");
    }

    private static string GenerateRandomReviewComment(int rating, string carMake, string carModel)
    {
        var positiveComments = new[]
        {
            $"I've been very happy with my {carMake} {carModel}. It's reliable, efficient, and comfortable for daily driving.",
            $"This {carMake} {carModel} has exceeded my expectations. Great build quality and excellent features for the price.",
            $"Highly recommend this {carMake} {carModel}. It's been trouble-free and a pleasure to drive.",
            $"The {carMake} {carModel} is an excellent choice. Good performance, comfort, and value for money."
        };

        var neutralComments = new[]
        {
            $"The {carMake} {carModel} is decent overall. Some good points and some areas for improvement.",
            $"Mixed feelings about this {carMake} {carModel}. It does the job but has some quirks.",
            $"The {carMake} {carModel} is okay for the price. Not outstanding but gets the job done.",
            $"Average experience with this {carMake} {carModel}. Some things I like, others not so much."
        };

        var negativeComments = new[]
        {
            $"Disappointed with this {carMake} {carModel}. Several issues and higher maintenance costs than expected.",
            $"Would not recommend this {carMake} {carModel}. Poor reliability and expensive repairs.",
            $"The {carMake} {carModel} has been problematic. Multiple trips to the dealer for various issues.",
            $"Not impressed with this {carMake} {carModel}. Build quality is poor and it feels cheap."
        };

        return rating >= 4 ? positiveComments[new Random().Next(positiveComments.Length)] :
               rating == 3 ? neutralComments[new Random().Next(neutralComments.Length)] :
               negativeComments[new Random().Next(negativeComments.Length)];
    }

    private async Task SeedEventsAsync()
    {
        if (await _context.Events.AnyAsync()) return;

        var users = await _context.Users.ToListAsync();
        if (!users.Any()) return;

        var random = new Random();
        var events = new List<CommunityCar.Domain.Entities.Community.Events.Event>();

        var eventData = new[]
        {
            new { Title = "Weekend Car Meet at Central Park", Description = "Join us for a relaxed car meet in Central Park. Bring your ride and meet fellow car enthusiasts! Coffee and donuts provided.", Location = "Central Park, New York, NY", Tags = new[] { "car-meet", "weekend", "coffee", "social" }, Price = (decimal?)null, MaxAttendees = (int?)50 },
            new { Title = "Track Day at Laguna Seca", Description = "Experience the thrill of driving your car on the legendary Laguna Seca raceway. Professional instructors available for beginners.", Location = "WeatherTech Raceway Laguna Seca, Monterey, CA", Tags = new[] { "track-day", "racing", "performance", "laguna-seca" }, Price = (decimal?)299.99m, MaxAttendees = (int?)30 },
            new { Title = "Classic Car Show & Swap Meet", Description = "Annual classic car show featuring vintage automobiles from the 1920s to 1980s. Swap meet for parts and memorabilia.", Location = "Fairgrounds, Pomona, CA", Tags = new[] { "classic-cars", "vintage", "swap-meet", "show" }, Price = (decimal?)15.00m, MaxAttendees = (int?)null },
            new { Title = "Midnight Canyon Run", Description = "Late night spirited drive through the winding canyon roads. Experienced drivers only. Safety briefing mandatory.", Location = "Angeles Crest Highway, CA", Tags = new[] { "canyon-run", "night-drive", "spirited", "experienced" }, Price = (decimal?)null, MaxAttendees = (int?)20 },
            new { Title = "Electric Vehicle Expo", Description = "Showcase of the latest electric vehicles, charging technology, and sustainable transportation solutions.", Location = "Convention Center, Austin, TX", Tags = new[] { "electric-vehicles", "ev", "technology", "sustainable" }, Price = (decimal?)25.00m, MaxAttendees = (int?)200 },
            new { Title = "Autocross Competition", Description = "Timed autocross event for all skill levels. Helmets required. Trophies for top finishers in each class.", Location = "Parking Lot, Phoenix, AZ", Tags = new[] { "autocross", "competition", "timed", "racing" }, Price = (decimal?)45.00m, MaxAttendees = (int?)60 },
            new { Title = "Cars & Coffee Monthly", Description = "Monthly gathering of car enthusiasts. All makes and models welcome. Great opportunity to network and share stories.", Location = "Shopping Center, Irvine, CA", Tags = new[] { "cars-and-coffee", "monthly", "networking", "all-makes" }, Price = (decimal?)null, MaxAttendees = (int?)100 },
            new { Title = "Drift Practice Session", Description = "Open drift practice at our private facility. Beginners welcome with instructor guidance. Safety gear required.", Location = "Drift Track, Atlanta, GA", Tags = new[] { "drift", "practice", "beginners", "instruction" }, Price = (decimal?)75.00m, MaxAttendees = (int?)25 },
            new { Title = "Supercar Rally", Description = "Exclusive rally for supercars through scenic mountain roads. Lunch stop at luxury resort included.", Location = "Rocky Mountains, Colorado", Tags = new[] { "supercar", "rally", "luxury", "scenic" }, Price = (decimal?)500.00m, MaxAttendees = (int?)15 },
            new { Title = "Muscle Car Meetup", Description = "Gathering for American muscle car owners. Burnout contest and drag racing discussions. BBQ lunch provided.", Location = "Drag Strip, Detroit, MI", Tags = new[] { "muscle-cars", "american", "burnout", "bbq" }, Price = (decimal?)20.00m, MaxAttendees = (int?)75 }
        };

        for (int i = 0; i < eventData.Length; i++)
        {
            var data = eventData[i];
            var organizer = users[random.Next(users.Count)];
            
            // Create events with varying dates (some past, some upcoming)
            var daysOffset = random.Next(-30, 60); // Events from 30 days ago to 60 days in future
            var startTime = DateTime.UtcNow.AddDays(daysOffset).AddHours(random.Next(9, 18));
            var endTime = startTime.AddHours(random.Next(2, 8));

            var eventItem = new CommunityCar.Domain.Entities.Community.Events.Event(
                data.Title,
                data.Description,
                startTime,
                endTime,
                data.Location,
                organizer.Id
            );

            // Set additional properties
            if (data.Price.HasValue)
            {
                eventItem.UpdatePricing(data.Price.Value, "Payment required at registration");
            }

            if (data.MaxAttendees.HasValue)
            {
                eventItem.UpdateAttendanceSettings(data.MaxAttendees.Value, random.Next(0, 2) == 1);
            }

            eventItem.UpdateVisibility(random.Next(0, 10) > 1); // 90% public events

            // Add tags
            foreach (var tag in data.Tags)
            {
                eventItem.AddTag(tag);
            }

            // Add some attendees for past and current events
            if (daysOffset <= 0)
            {
                var attendeeCount = random.Next(1, Math.Min(data.MaxAttendees ?? 50, 30));
                for (int j = 0; j < attendeeCount; j++)
                {
                    eventItem.IncrementAttendeeCount();
                }
            }

            events.Add(eventItem);
        }

        await _context.Events.AddRangeAsync(events);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Successfully seeded {events.Count} events.");
    }

    private async Task SeedGroupsAsync()
    {
        if (await _context.Groups.AnyAsync()) return;

        _logger.LogInformation("Seeding groups...");

        var users = await _userManager.Users.ToListAsync();
        var groups = new List<CommunityCar.Domain.Entities.Community.Groups.Group>();
        var random = new Random();

        var groupData = new[]
        {
            new {
                Name = "Classic Mustang Enthusiasts",
                Description = "Dedicated to Ford Mustang lovers from 1964-1973. Share your restoration projects, ask questions, and connect with fellow enthusiasts.",
                Category = (string?)"Ford",
                Privacy = CommunityCar.Domain.Enums.GroupPrivacy.Public,
                Location = (string?)"Detroit, MI",
                Rules = (string?)"Be respectful. Share knowledge. No spam or commercial posts without permission.",
                RequiresApproval = false,
                IsOfficial = true,
                Tags = new[] { "mustang", "ford", "muscle-car", "classic", "restoration" }
            },
            new {
                Name = "Vintage Car Restoration",
                Description = "Share tips, progress, and ask questions about restoring classic cars from all eras. From barn finds to concours restorations.",
                Category = (string?)"General",
                Privacy = CommunityCar.Domain.Enums.GroupPrivacy.Public,
                Location = (string?)null,
                Rules = (string?)"Be respectful. Share knowledge. No spam. Post progress photos when possible.",
                RequiresApproval = false,
                IsOfficial = false,
                Tags = new[] { "restoration", "diy", "classic-cars", "vintage", "bodywork" }
            },
            new {
                Name = "Electric Vehicle Conversion",
                Description = "Converting classic cars to electric power. Share your EV conversion projects, technical discussions, and troubleshooting.",
                Category = (string?)"Modification",
                Privacy = CommunityCar.Domain.Enums.GroupPrivacy.Private,
                Location = (string?)null,
                Rules = (string?)"Technical discussions only. Safety first. Share schematics and progress photos.",
                RequiresApproval = true,
                IsOfficial = false,
                Tags = new[] { "ev", "electric", "modification", "conversion", "battery" }
            },
            new {
                Name = "British Sports Cars",
                Description = "MG, Triumph, Austin-Healey, Jaguar, and more. Celebrating the heritage of British automotive engineering.",
                Category = "British",
                Privacy = CommunityCar.Domain.Enums.GroupPrivacy.Public,
                Location = "London, UK",
                Rules = "British cars only. Share stories, photos, and technical advice. Keep it civil.",
                RequiresApproval = false,
                IsOfficial = false,
                Tags = new[] { "british", "mg", "triumph", "jaguar", "austin-healey" }
            },
            new {
                Name = "American Muscle",
                Description = "Discussing classic American muscle cars from the golden era. Camaros, Challengers, GTOs, and more.",
                Category = "American",
                Privacy = CommunityCar.Domain.Enums.GroupPrivacy.Public,
                Location = (string?)null,
                Rules = "American muscle cars only. No imports. Share dyno sheets and track times.",
                RequiresApproval = false,
                IsOfficial = true,
                Tags = new[] { "muscle", "american", "v8", "camaro", "challenger" }
            },
            new {
                Name = "European Classics",
                Description = "Mercedes, BMW, Porsche, Ferrari, and other European vintage models. Celebrating European automotive excellence.",
                Category = "European",
                Privacy = CommunityCar.Domain.Enums.GroupPrivacy.Public,
                Location = (string?)null,
                Rules = "European cars only. Share maintenance tips and driving experiences.",
                RequiresApproval = false,
                IsOfficial = false,
                Tags = new[] { "european", "mercedes", "bmw", "porsche", "ferrari" }
            },
            new {
                Name = "JDM Legends",
                Description = "Japanese Domestic Market classics. Skylines, Supras, NSXs, and other JDM icons.",
                Category = "Japanese",
                Privacy = CommunityCar.Domain.Enums.GroupPrivacy.Private,
                Location = (string?)null,
                Rules = "JDM cars only. No USDM versions. Share import stories and modifications.",
                RequiresApproval = true,
                IsOfficial = false,
                Tags = new[] { "jdm", "japan", "skyline", "supra", "nsx" }
            },
            new {
                Name = "Corvette Club",
                Description = "All generations of America's sports car. From C1 to C8, celebrating Corvette heritage and performance.",
                Category = "Chevrolet",
                Privacy = CommunityCar.Domain.Enums.GroupPrivacy.Public,
                Location = "Bowling Green, KY",
                Rules = "Corvette owners and enthusiasts only. Family-friendly discussions. Share track experiences.",
                RequiresApproval = false,
                IsOfficial = true,
                Tags = new[] { "corvette", "chevrolet", "sports-car", "track", "performance" }
            },
            new {
                Name = "VW Air-Cooled",
                Description = "Beetles, Buses, Type 3s, and other air-cooled Volkswagens. Keeping the air-cooled spirit alive.",
                Category = "Volkswagen",
                Privacy = CommunityCar.Domain.Enums.GroupPrivacy.Public,
                Location = (string?)null,
                Rules = "Air-cooled VWs only. Share restoration tips and camping adventures.",
                RequiresApproval = false,
                IsOfficial = false,
                Tags = new[] { "vw", "beetle", "bus", "air-cooled", "camping" }
            },
            new {
                Name = "Alfa Romeo Owners",
                Description = "Italian passion and performance. Celebrating the art and engineering of Alfa Romeo.",
                Category = "Italian",
                Privacy = CommunityCar.Domain.Enums.GroupPrivacy.Private,
                Location = (string?)null,
                Rules = "Alfa Romeo owners only. Share your passion and mechanical adventures.",
                RequiresApproval = true,
                IsOfficial = false,
                Tags = new[] { "alfa-romeo", "italian", "performance", "passion", "engineering" }
            },
            new {
                Name = "Hot Rod Builders",
                Description = "Custom builds and modifications. Traditional hot rods, street rods, and custom creations.",
                Category = "Modification",
                Privacy = CommunityCar.Domain.Enums.GroupPrivacy.Public,
                Location = (string?)null,
                Rules = "Custom builds only. Share build progress and technical details.",
                RequiresApproval = false,
                IsOfficial = false,
                Tags = new[] { "hot-rod", "custom", "build", "fabrication", "street-rod" }
            },
            new {
                Name = "Motorcycle Classics",
                Description = "Vintage motorcycles and restoration. Two-wheeled classics from all eras and manufacturers.",
                Category = "Motorcycle",
                Privacy = CommunityCar.Domain.Enums.GroupPrivacy.Public,
                Location = (string?)null,
                Rules = "Motorcycles only. Share restoration projects and riding experiences.",
                RequiresApproval = false,
                IsOfficial = false,
                Tags = new[] { "motorcycle", "vintage", "restoration", "riding", "classics" }
            },
            new {
                Name = "Truck & Van Classics",
                Description = "Classic trucks and vans community. Work trucks, panel vans, and commercial vehicles.",
                Category = "Trucks",
                Privacy = CommunityCar.Domain.Enums.GroupPrivacy.Public,
                Location = (string?)null,
                Rules = "Trucks and vans only. Share work stories and restoration projects.",
                RequiresApproval = false,
                IsOfficial = false,
                Tags = new[] { "truck", "van", "commercial", "work", "utility" }
            },
            new {
                Name = "Racing Heritage",
                Description = "Historic racing cars and motorsports. Formula cars, sports racers, and competition vehicles.",
                Category = "Racing",
                Privacy = CommunityCar.Domain.Enums.GroupPrivacy.Private,
                Location = (string?)null,
                Rules = "Serious discussions only. Racing cars and motorsport history. No off-topic posts.",
                RequiresApproval = true,
                IsOfficial = false,
                Tags = new[] { "racing", "motorsport", "historic", "formula", "competition" }
            },
            new {
                Name = "Parts Marketplace",
                Description = "Buy, sell, and trade classic car parts. Connect with other enthusiasts for hard-to-find components.",
                Category = "Marketplace",
                Privacy = CommunityCar.Domain.Enums.GroupPrivacy.Public,
                Location = (string?)null,
                Rules = "No scams. Verified sellers only. Be honest about condition. Include photos.",
                RequiresApproval = false,
                IsOfficial = false,
                Tags = new[] { "parts", "marketplace", "buy-sell", "trade", "components" }
            }
        };

        // Create groups from sample data
        foreach (dynamic data in groupData)
        {
            var randomOwner = users[random.Next(users.Count)];
            var group = new CommunityCar.Domain.Entities.Community.Groups.Group(
                name: data.Name, 
                description: data.Description, 
                privacy: data.Privacy, 
                ownerId: randomOwner.Id, 
                category: data.Category, 
                rules: data.Rules, 
                requiresApproval: data.RequiresApproval, 
                location: data.Location);

            // Add tags
            foreach (var tag in data.Tags)
            {
                group.AddTag(tag);
            }

            // Set official status
            if (data.IsOfficial)
            {
                group.MarkAsOfficial();
            }
            else
            {
                // 30% chance to be verified
                if (random.Next(100) < 30)
                {
                    group.Verify();
                }
            }

            // Add random member count
            var memberCount = random.Next(50, 3500);
            for (int i = 0; i < memberCount; i++)
            {
                group.IncrementMemberCount();
            }

            // Add random post count
            var postCount = random.Next(20, memberCount / 2);
            for (int i = 0; i < postCount; i++)
            {
                group.IncrementPostCount();
            }

            groups.Add(group);
        }

        await _context.Groups.AddRangeAsync(groups);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Successfully seeded {groups.Count} groups.");
    }

    private async Task SeedPostsAsync()
    {
        if (await _context.Posts.AnyAsync()) return;

        _logger.LogInformation("Seeding posts...");

        var users = await _userManager.Users.ToListAsync();
        var posts = new List<CommunityCar.Domain.Entities.Community.Posts.Post>();
        var random = new Random();

        var postData = new[]
        {
            new {
                Title = "How to maintain your classic car during winter?",
                Content = @"Winter can be tough on classic cars. Here are some essential tips to keep your vehicle in top shape during the cold months:

**Storage Tips:**
1. Store your car in a dry, climate-controlled garage if possible
2. Use a breathable car cover to protect the paint
3. Keep the garage temperature stable to prevent condensation

**Battery Care:**
- Disconnect the battery or use a trickle charger
- Check battery terminals for corrosion
- Consider removing the battery entirely for indoor storage

**Fuel System:**
- Add fuel stabilizer to prevent degradation
- Fill the tank to prevent moisture buildup
- Consider draining the fuel system for long-term storage

**Tires:**
- Inflate tires to proper pressure to prevent flat spots
- Consider moving the car periodically or using tire cradles
- Check for any signs of cracking or deterioration

**Fluids:**
- Change oil before storage
- Check coolant levels and condition
- Top off all fluids to prevent corrosion

What are your winter storage tips? Share your experiences below!",
                Type = CommunityCar.Domain.Enums.PostType.Text
            },
            new {
                Title = "My 1967 Mustang Restoration Journey",
                Content = @"After 18 months of hard work, my Mustang is finally coming together! Here's what I've accomplished so far:

**Bodywork Complete:**
- Stripped to bare metal and repaired rust
- New quarter panels and floor pans
- Fresh coat of Wimbledon White paint

**Engine Rebuild:**
- 289 V8 completely rebuilt
- New pistons, rings, and bearings
- Edelbrock intake and Holley carburetor

**Interior:**
- Restored original seats with new covers
- New carpet and door panels
- Original AM/FM radio restored

**Still To Do:**
- Final assembly and detail work
- Brake system overhaul
- Exhaust system installation

The journey has been challenging but incredibly rewarding. Can't wait to take her for the first drive!

#mustang #restoration #1967 #classiccar",
                Type = CommunityCar.Domain.Enums.PostType.Image
            },
            new {
                Title = "Best engine oils for vintage cars - Comprehensive Guide",
                Content = @"Choosing the right engine oil for vintage cars is crucial for longevity and performance. Here's what you need to know:

**Conventional vs Synthetic:**
- Vintage engines were designed for conventional oils
- Modern synthetics may cause seal leaks in older engines
- Consider high-mileage formulations for older vehicles

**Viscosity Considerations:**
- Follow manufacturer specifications
- Consider climate and driving conditions
- 20W-50 is popular for many classics

**Additives:**
- Zinc (ZDDP) is crucial for flat-tappet cams
- Many modern oils have reduced zinc content
- Look for oils specifically formulated for classics

**Recommended Brands:**
- Valvoline VR1 Racing Oil
- Lucas Classic Car Motor Oil
- Castrol GTX Classic
- Shell Rotella T (diesel oil with high zinc)

**Change Intervals:**
- More frequent changes for stored vehicles
- Consider oil analysis for optimal intervals
- Don't exceed 6 months even with low miles

What oil do you use in your classic? Share your experiences and recommendations!

Link to detailed comparison: https://example.com/vintage-car-oils",
                Type = CommunityCar.Domain.Enums.PostType.Link
            },
            new {
                Title = "What's your dream classic car?",
                Content = @"Let's have some fun! If money was no object, what classic car would you love to own and why?

Here are some options to get us started:

🏁 **American Muscle:**
- 1970 Plymouth 'Cuda 440 Six Pack
- 1969 Dodge Charger R/T
- 1967 Shelby GT500

🏎️ **European Exotics:**
- 1961 Ferrari 250 GT SWB
- 1973 Porsche 911 Carrera RS
- 1963 Jaguar E-Type

🇯🇵 **JDM Legends:**
- 1994 Toyota Supra Turbo
- 1999 Nissan Skyline GT-R R34
- 1991 Honda NSX

💎 **Luxury Classics:**
- 1955 Mercedes 300SL Gullwing
- 1961 Aston Martin DB4 GT
- 1938 Bugatti Type 57 Atlantic

Vote for your favorite category and tell us why! What draws you to that particular car? Is it the design, performance, rarity, or personal connection?

Can't wait to see your choices! 🚗💨",
                Type = CommunityCar.Domain.Enums.PostType.Poll
            },
            new {
                Title = "Track Day Preparation Checklist",
                Content = @"Getting ready for your first track day? Here's a comprehensive checklist to ensure you're prepared:

**Vehicle Preparation:**
□ Check tire pressure and tread depth
□ Inspect brake pads and fluid
□ Verify all fluids are topped off
□ Ensure battery is secure
□ Remove loose items from interior
□ Check wheel lug nuts

**Safety Equipment:**
□ DOT or Snell approved helmet
□ Long pants and closed-toe shoes
□ Fire extinguisher (if required)
□ First aid kit
□ Emergency contact information

**Documentation:**
□ Driver's license
□ Insurance information
□ Vehicle registration
□ Track day waiver (if pre-signed)

**Tools and Spares:**
□ Basic tool kit
□ Tire pressure gauge
□ Spare brake pads
□ Extra brake fluid
□ Duct tape and zip ties
□ Towels and cleaning supplies

**Personal Items:**
□ Sunscreen and water
□ Folding chair
□ Camera for memories
□ Cash for lunch/drinks
□ Change of clothes

**Pre-Track Inspection:**
- Walk around the car
- Check for any leaks
- Verify all lights work
- Test brakes in parking lot

Remember: The goal is to have fun and learn, not to set lap records on your first day!

What would you add to this list?",
                Type = CommunityCar.Domain.Enums.PostType.Text
            }
        };

        // Create posts from sample data
        foreach (var data in postData)
        {
            var randomAuthor = users[random.Next(users.Count)];
            var post = new CommunityCar.Domain.Entities.Community.Posts.Post(
                data.Title, data.Content, data.Type, randomAuthor.Id);

            posts.Add(post);
        }

        // Create additional random posts
        var additionalTitles = new[]
        {
            "Carburetor tuning tips for beginners",
            "Found this gem in a barn - what should I do?",
            "Best classic car shows this summer",
            "Dealing with rust - prevention and treatment",
            "Electric conversion vs keeping original engine",
            "Insurance tips for classic car owners",
            "Weekend project: Installing new gauges",
            "Classic car values - bubble or sustainable growth?",
            "Favorite driving roads for classic cars",
            "Parts sourcing - OEM vs reproduction",
            "Paint correction on vintage finishes",
            "Transmission rebuild or replacement?",
            "Classic car storage solutions",
            "Joining a car club - worth it?",
            "Documenting your restoration project",
            "Classic car photography tips",
            "Maintenance schedules for vintage vehicles",
            "Upgrading brakes while keeping originality",
            "Classic car events calendar",
            "Selling a classic car - best practices"
        };

        var postTypes = new[] { 
            CommunityCar.Domain.Enums.PostType.Text, 
            CommunityCar.Domain.Enums.PostType.Image, 
            CommunityCar.Domain.Enums.PostType.Link,
            CommunityCar.Domain.Enums.PostType.Poll
        };

        for (int i = 0; i < 20; i++)
        {
            var title = additionalTitles[i];
            var type = postTypes[random.Next(postTypes.Length)];
            var author = users[random.Next(users.Count)];

            var content = type switch
            {
                CommunityCar.Domain.Enums.PostType.Text => $"This is a discussion about {title.ToLower()}. What are your thoughts and experiences with this topic? Share your knowledge with the community!",
                CommunityCar.Domain.Enums.PostType.Image => $"Check out these photos related to {title.ToLower()}. What do you think? Any similar experiences or advice?",
                CommunityCar.Domain.Enums.PostType.Link => $"Found this great resource about {title.ToLower()}: https://example.com/{title.Replace(" ", "-").ToLower()}",
                CommunityCar.Domain.Enums.PostType.Poll => $"Quick poll about {title.ToLower()}. What's your preference? Vote and share your reasoning!",
                _ => $"Discussion about {title.ToLower()}."
            };

            var post = new CommunityCar.Domain.Entities.Community.Posts.Post(
                title, content, type, author.Id);

            posts.Add(post);
        }

        await _context.Posts.AddRangeAsync(posts);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Successfully seeded {posts.Count} posts.");
    }
}