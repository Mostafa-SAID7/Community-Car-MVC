using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.Entities.Account.Media;
using CommunityCar.Domain.Enums.Account;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding.Account.Media;

public class UserGallerySeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<UserGallerySeeder> _logger;

    public UserGallerySeeder(ApplicationDbContext context, UserManager<User> userManager, ILogger<UserGallerySeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _context.UserGalleries.AnyAsync()) return;

        _logger.LogInformation("Seeding user galleries...");

        var users = await _userManager.Users.Take(30).ToListAsync(); // Seed galleries for first 30 users
        var galleries = new List<UserGallery>();
        var random = new Random();

        var photoTitles = new[]
        {
            "My Classic Mustang",
            "Engine Bay Detail",
            "Interior Restoration",
            "Before and After",
            "Track Day Action",
            "Weekend Cruise",
            "Car Show Display",
            "Restoration Progress",
            "New Wheels",
            "Paint Correction",
            "Under the Hood",
            "Dashboard View",
            "Sunset Drive",
            "Garage Shot",
            "Detail Work",
            "Custom Modifications",
            "Original Parts",
            "Tool Collection",
            "Workshop Setup",
            "Final Result"
        };

        var descriptions = new[]
        {
            "Proud of this restoration project",
            "Finally got the engine running perfectly",
            "Love the way the interior turned out",
            "Amazing transformation over 2 years",
            "Had a blast at the track today",
            "Perfect weather for a drive",
            "Won second place at the show",
            "Slow but steady progress",
            "New shoes for the old girl",
            "Paint looks like glass now",
            "Clean engine bay after detailing",
            "Original gauges still working",
            "Golden hour photography",
            "Home sweet home",
            "Attention to detail pays off",
            "Custom work by local shop",
            "Hard to find original parts",
            "My trusty tools",
            "Where the magic happens",
            "All the hard work paid off"
        };

        var sampleImageUrls = new[]
        {
            "https://images.unsplash.com/photo-1560958089-b8a1929cea89?w=800&h=600&fit=crop",
            "https://images.unsplash.com/photo-1605559424843-9e4c228bf1c2?w=800&h=600&fit=crop",
            "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=800&h=600&fit=crop",
            "https://images.unsplash.com/photo-1555215695-3004980ad54e?w=800&h=600&fit=crop",
            "https://images.unsplash.com/photo-1549317661-bd32c8ce0db2?w=800&h=600&fit=crop",
            "https://images.unsplash.com/photo-1518709268805-4e9042af2176?w=800&h=600&fit=crop",
            "https://images.unsplash.com/photo-1571019613454-1cb2f99b2d8b?w=800&h=600&fit=crop",
            "https://images.unsplash.com/photo-1552519507-da3b142c6e3d?w=800&h=600&fit=crop",
            "https://images.unsplash.com/photo-1563720223185-11003d516935?w=800&h=600&fit=crop",
            "https://images.unsplash.com/photo-1544636331-e26879cd4d9b?w=800&h=600&fit=crop"
        };

        var tags = new[]
        {
            "[\"classic\", \"restoration\", \"vintage\"]",
            "[\"engine\", \"performance\", \"mechanical\"]",
            "[\"interior\", \"upholstery\", \"original\"]",
            "[\"before-after\", \"transformation\", \"progress\"]",
            "[\"track\", \"racing\", \"performance\"]",
            "[\"cruise\", \"weekend\", \"driving\"]",
            "[\"show\", \"display\", \"award\"]",
            "[\"work-in-progress\", \"restoration\", \"diy\"]",
            "[\"wheels\", \"tires\", \"upgrade\"]",
            "[\"paint\", \"detailing\", \"finish\"]"
        };

        foreach (var user in users)
        {
            // Each user gets 3-12 gallery items
            var itemCount = random.Next(3, 13);

            for (int i = 0; i < itemCount; i++)
            {
                var title = photoTitles[random.Next(photoTitles.Length)];
                var description = descriptions[random.Next(descriptions.Length)];
                var mediaUrl = sampleImageUrls[random.Next(sampleImageUrls.Length)];
                var thumbnailUrl = mediaUrl.Replace("w=800&h=600", "w=300&h=200");
                var mediaType = MediaType.Image;
                var isPublic = random.Next(100) < 85; // 85% public
                var tagSet = tags[random.Next(tags.Length)];

                var galleryItem = new UserGallery(user.Id, title, mediaUrl, mediaType, isPublic);
                
                // Set additional properties
                galleryItem.UpdateDetails(title, description, tagSet);
                galleryItem.SetThumbnail(thumbnailUrl);

                // Add some engagement
                var viewCount = random.Next(5, 200);
                for (int j = 0; j < viewCount; j++)
                {
                    galleryItem.IncrementViews();
                }

                var likeCount = random.Next(0, 50);
                for (int j = 0; j < likeCount; j++)
                {
                    galleryItem.IncrementLikes();
                }

                // 20% chance to be featured
                if (random.Next(100) < 20)
                {
                    galleryItem.ToggleFeatured();
                }

                galleries.Add(galleryItem);
            }
        }

        await _context.UserGalleries.AddRangeAsync(galleries);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Successfully seeded {galleries.Count} user gallery items.");
    }
}