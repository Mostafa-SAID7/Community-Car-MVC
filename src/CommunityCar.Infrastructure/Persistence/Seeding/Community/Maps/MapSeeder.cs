using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding.Community.Maps;

public class MapSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<MapSeeder> _logger;

    public MapSeeder(
        ApplicationDbContext context,
        UserManager<User> userManager,
        ILogger<MapSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _context.PointsOfInterest.AnyAsync()) return;

        _logger.LogInformation("Seeding maps data...");

        var users = await _userManager.Users.ToListAsync();
        var pointsOfInterest = new List<PointOfInterest>();
        var routes = new List<Route>();
        var checkIns = new List<CheckIn>();
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
            var poi = new PointOfInterest(
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

            var poi = new PointOfInterest(
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
            var route = new Route(
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

                var checkIn = new CheckIn(
                    poi.Id, user.Id, comment, rating, isPrivate);

                checkIns.Add(checkIn);
            }
        }

        await _context.CheckIns.AddRangeAsync(checkIns);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Successfully seeded {pointsOfInterest.Count} points of interest, {routes.Count} routes, and {checkIns.Count} check-ins.");
    }
}