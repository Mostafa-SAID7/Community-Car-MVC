using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.Entities.Community.Guides;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding.Community.Content;

public class GuideSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<GuideSeeder> _logger;

    public GuideSeeder(
        ApplicationDbContext context,
        UserManager<User> userManager,
        ILogger<GuideSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAsync()
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
}