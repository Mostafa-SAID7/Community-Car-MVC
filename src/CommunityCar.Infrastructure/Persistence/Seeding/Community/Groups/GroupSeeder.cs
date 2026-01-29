using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.Entities.Community.Groups;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding.Community.Groups;

public class GroupSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<GroupSeeder> _logger;

    public GroupSeeder(
        ApplicationDbContext context,
        UserManager<User> userManager,
        ILogger<GroupSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _context.Groups.AnyAsync()) return;

        _logger.LogInformation("Seeding groups...");

        var users = await _userManager.Users.ToListAsync();
        var groups = new List<Group>();
        var random = new Random();

        var groupData = new[]
        {
            new {
                Name = "Classic Mustang Enthusiasts",
                Description = "Dedicated to Ford Mustang lovers from 1964-1973. Share your restoration projects, ask questions, and connect with fellow enthusiasts.",
                Category = (string?)"Ford",
                Privacy = GroupPrivacy.Public,
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
                Privacy = GroupPrivacy.Public,
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
                Privacy = GroupPrivacy.Private,
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
                Privacy = GroupPrivacy.Public,
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
                Privacy = GroupPrivacy.Public,
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
                Privacy = GroupPrivacy.Public,
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
                Privacy = GroupPrivacy.Private,
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
                Privacy = GroupPrivacy.Public,
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
                Privacy = GroupPrivacy.Public,
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
                Privacy = GroupPrivacy.Private,
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
                Privacy = GroupPrivacy.Public,
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
                Privacy = GroupPrivacy.Public,
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
                Privacy = GroupPrivacy.Public,
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
                Privacy = GroupPrivacy.Private,
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
                Privacy = GroupPrivacy.Public,
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
            var group = new Group(
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
}