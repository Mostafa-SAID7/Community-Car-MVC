using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.Entities.Community.News;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding.Community.Content;

public class NewsSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<NewsSeeder> _logger;

    public NewsSeeder(
        ApplicationDbContext context,
        UserManager<User> userManager,
        ILogger<NewsSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAsync()
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
}