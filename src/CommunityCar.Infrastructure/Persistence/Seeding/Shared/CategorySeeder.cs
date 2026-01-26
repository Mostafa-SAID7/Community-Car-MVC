using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding.Shared;

public class CategorySeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CategorySeeder> _logger;

    public CategorySeeder(ApplicationDbContext context, ILogger<CategorySeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            if (await _context.Categories.AnyAsync())
            {
                _logger.LogInformation("Categories already exist, skipping seeding");
                return;
            }

            _logger.LogInformation("Seeding categories...");

            var categories = new List<Category>();

            // Root categories
            var automotive = new Category("Automotive", "automotive", "All things related to cars and vehicles");
            var maintenance = new Category("Maintenance", "maintenance", "Car maintenance and repair topics");
            var performance = new Category("Performance", "performance", "Performance modifications and tuning");
            var lifestyle = new Category("Lifestyle", "lifestyle", "Car lifestyle and culture");
            var technology = new Category("Technology", "technology", "Automotive technology and innovations");
            var community = new Category("Community", "community", "Community discussions and events");

            categories.AddRange(new[] { automotive, maintenance, performance, lifestyle, technology, community });

            // Add root categories first
            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();

            // Sub-categories for Automotive
            var automotiveSubCategories = new[]
            {
                new Category("Sedans", "sedans", "Sedan cars and discussions", automotive.Id),
                new Category("SUVs", "suvs", "Sport Utility Vehicles", automotive.Id),
                new Category("Sports Cars", "sports-cars", "High-performance sports cars", automotive.Id),
                new Category("Electric Vehicles", "electric-vehicles", "Electric and hybrid vehicles", automotive.Id),
                new Category("Luxury Cars", "luxury-cars", "Premium and luxury vehicles", automotive.Id),
                new Category("Classic Cars", "classic-cars", "Vintage and classic automobiles", automotive.Id)
            };

            // Sub-categories for Maintenance
            var maintenanceSubCategories = new[]
            {
                new Category("Engine", "engine", "Engine maintenance and repair", maintenance.Id),
                new Category("Transmission", "transmission", "Transmission issues and maintenance", maintenance.Id),
                new Category("Brakes", "brakes", "Brake system maintenance", maintenance.Id),
                new Category("Suspension", "suspension", "Suspension and steering", maintenance.Id),
                new Category("Electrical", "electrical", "Electrical system issues", maintenance.Id),
                new Category("Tires", "tires", "Tire maintenance and selection", maintenance.Id)
            };

            // Sub-categories for Performance
            var performanceSubCategories = new[]
            {
                new Category("Tuning", "tuning", "Engine tuning and modifications", performance.Id),
                new Category("Exhaust", "exhaust", "Exhaust system modifications", performance.Id),
                new Category("Intake", "intake", "Air intake modifications", performance.Id),
                new Category("Suspension Mods", "suspension-mods", "Performance suspension upgrades", performance.Id),
                new Category("Turbo & Supercharging", "turbo-supercharging", "Forced induction systems", performance.Id),
                new Category("Racing", "racing", "Racing and track modifications", performance.Id)
            };

            // Sub-categories for Lifestyle
            var lifestyleSubCategories = new[]
            {
                new Category("Car Shows", "car-shows", "Car shows and exhibitions", lifestyle.Id),
                new Category("Road Trips", "road-trips", "Road trip experiences and planning", lifestyle.Id),
                new Category("Photography", "photography", "Automotive photography", lifestyle.Id),
                new Category("Collecting", "collecting", "Car collecting and investment", lifestyle.Id),
                new Category("Culture", "culture", "Car culture and communities", lifestyle.Id),
                new Category("Events", "events", "Automotive events and meetups", lifestyle.Id)
            };

            // Sub-categories for Technology
            var technologySubCategories = new[]
            {
                new Category("Infotainment", "infotainment", "In-car entertainment systems", technology.Id),
                new Category("Safety Systems", "safety-systems", "Advanced safety technologies", technology.Id),
                new Category("Autonomous Driving", "autonomous-driving", "Self-driving car technology", technology.Id),
                new Category("Connectivity", "connectivity", "Connected car features", technology.Id),
                new Category("Diagnostics", "diagnostics", "Car diagnostic tools and software", technology.Id),
                new Category("Apps & Software", "apps-software", "Automotive apps and software", technology.Id)
            };

            // Sub-categories for Community
            var communitySubCategories = new[]
            {
                new Category("General Discussion", "general-discussion", "General car-related discussions", community.Id),
                new Category("Buying & Selling", "buying-selling", "Car buying and selling advice", community.Id),
                new Category("Reviews", "reviews", "Car reviews and opinions", community.Id),
                new Category("Q&A", "qa", "Questions and answers", community.Id),
                new Category("News", "news", "Automotive news and updates", community.Id),
                new Category("Off-Topic", "off-topic", "Non-automotive discussions", community.Id)
            };

            // Add all sub-categories
            var allSubCategories = automotiveSubCategories
                .Concat(maintenanceSubCategories)
                .Concat(performanceSubCategories)
                .Concat(lifestyleSubCategories)
                .Concat(technologySubCategories)
                .Concat(communitySubCategories)
                .ToList();

            await _context.Categories.AddRangeAsync(allSubCategories);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully seeded {Count} categories", categories.Count + allSubCategories.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding categories");
            throw;
        }
    }
}