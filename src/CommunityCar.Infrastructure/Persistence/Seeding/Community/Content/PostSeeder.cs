using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.Entities.Community.Posts;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding.Community.Content;

public class PostSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<PostSeeder> _logger;

    public PostSeeder(
        ApplicationDbContext context,
        UserManager<User> userManager,
        ILogger<PostSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _context.Posts.AnyAsync()) return;

        _logger.LogInformation("Seeding posts...");

        var users = await _userManager.Users.ToListAsync();
        var posts = new List<Post>();
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
                Type = PostType.Text
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
                Type = PostType.Image
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
                Type = PostType.Link
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
                Type = PostType.Poll
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
                Type = PostType.Text
            }
        };

        // Create posts from sample data
        foreach (var data in postData)
        {
            var randomAuthor = users[random.Next(users.Count)];
            var post = new Post(
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
            PostType.Text, 
            PostType.Image, 
            PostType.Link,
            PostType.Poll
        };

        for (int i = 0; i < 20; i++)
        {
            var title = additionalTitles[i];
            var type = postTypes[random.Next(postTypes.Length)];
            var author = users[random.Next(users.Count)];

            var content = type switch
            {
                PostType.Text => $"This is a discussion about {title.ToLower()}. What are your thoughts and experiences with this topic? Share your knowledge with the community!",
                PostType.Image => $"Check out these photos related to {title.ToLower()}. What do you think? Any similar experiences or advice?",
                PostType.Link => $"Found this great resource about {title.ToLower()}: https://example.com/{title.Replace(" ", "-").ToLower()}",
                PostType.Poll => $"Quick poll about {title.ToLower()}. What's your preference? Vote and share your reasoning!",
                _ => $"Discussion about {title.ToLower()}."
            };

            var post = new Post(
                title, content, type, author.Id);

            posts.Add(post);
        }

        await _context.Posts.AddRangeAsync(posts);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Successfully seeded {posts.Count} posts.");
    }
}