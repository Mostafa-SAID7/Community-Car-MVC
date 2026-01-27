using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding.Shared;

public class TagSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TagSeeder> _logger;

    public TagSeeder(ApplicationDbContext context, ILogger<TagSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            var existingTagCount = await _context.Tags.CountAsync();
            if (existingTagCount > 0)
            {
                _logger.LogInformation("Tags already exist ({Count} tags), skipping seeding", existingTagCount);
                return;
            }

            _logger.LogInformation("Seeding tags...");

            var tags = new List<Tag>();

            // Car brands
            var carBrands = new[]
            {
                "Toyota", "Honda", "Ford", "BMW", "Mercedes", "Audi", "Volkswagen", "Nissan", "Hyundai", "Kia",
                "Chevrolet", "Mazda", "Subaru", "Lexus", "Infiniti", "Acura", "Volvo", "Jaguar", "Land-Rover",
                "Porsche", "Ferrari", "Lamborghini", "McLaren", "Bentley", "Rolls-Royce", "Maserati", "Alfa-Romeo",
                "Fiat", "Peugeot", "Renault", "Citroën", "Skoda", "Seat", "Mini", "Tesla", "Rivian", "Lucid"
            };

            foreach (var brand in carBrands)
            {
                tags.Add(new Tag(brand, brand.ToLower().Replace(" ", "-")));
            }

            // Car types
            var carTypes = new[]
            {
                "Sedan", "SUV", "Hatchback", "Coupe", "Convertible", "Wagon", "Pickup", "Crossover",
                "Sports-Car", "Supercar", "Hypercar", "Electric", "Hybrid", "Diesel", "Manual", "Automatic",
                "AWD", "FWD", "RWD", "4WD", "Turbo", "Supercharged", "V6", "V8", "V12", "I4"
            };

            foreach (var type in carTypes)
            {
                tags.Add(new Tag(type, type.ToLower().Replace(" ", "-")));
            }

            // Maintenance tags
            var maintenanceTags = new[]
            {
                "Oil-Change", "Brake-Pads", "Tire-Rotation", "Engine-Tune", "Transmission-Service",
                "Coolant-Flush", "Spark-Plugs", "Air-Filter", "Fuel-Filter", "Battery", "Alternator",
                "Starter", "Radiator", "Thermostat", "Water-Pump", "Timing-Belt", "Serpentine-Belt",
                "Suspension", "Shocks", "Struts", "Ball-Joints", "Tie-Rods", "CV-Joints"
            };

            foreach (var tag in maintenanceTags)
            {
                tags.Add(new Tag(tag.Replace("-", " "), tag.ToLower()));
            }

            // Performance tags
            var performanceTags = new[]
            {
                "Cold-Air-Intake", "Exhaust-System", "Turbocharger", "Supercharger", "Intercooler",
                "ECU-Tune", "Dyno", "Horsepower", "Torque", "Quarter-Mile", "Track-Day", "Autocross",
                "Drag-Racing", "Circuit-Racing", "Drifting", "Time-Attack", "Hillclimb", "Rally"
            };

            foreach (var tag in performanceTags)
            {
                tags.Add(new Tag(tag.Replace("-", " "), tag.ToLower()));
            }

            // Technology tags
            var technologyTags = new[]
            {
                "Android-Auto", "Apple-CarPlay", "GPS", "Bluetooth", "WiFi", "USB", "Wireless-Charging",
                "Backup-Camera", "Dash-Cam", "Blind-Spot-Monitor", "Lane-Assist", "Adaptive-Cruise",
                "Parking-Sensors", "Keyless-Entry", "Push-Start", "Remote-Start", "Alarm-System",
                "Immobilizer", "OBD2", "Diagnostic", "Code-Reader"
            };

            foreach (var tag in technologyTags)
            {
                tags.Add(new Tag(tag.Replace("-", " "), tag.ToLower()));
            }

            // Lifestyle tags
            var lifestyleTags = new[]
            {
                "Car-Show", "Meet-Up", "Road-Trip", "Photography", "Detailing", "Waxing", "Polishing",
                "Ceramic-Coating", "Paint-Protection", "Window-Tint", "Vinyl-Wrap", "Custom-Paint",
                "Wheels", "Rims", "Tires", "Lowered", "Lifted", "Stance", "Camber", "Fitment"
            };

            foreach (var tag in lifestyleTags)
            {
                tags.Add(new Tag(tag.Replace("-", " "), tag.ToLower()));
            }

            // Problem/Issue tags
            var problemTags = new[]
            {
                "Check-Engine", "Overheating", "No-Start", "Rough-Idle", "Stalling", "Misfiring",
                "Knocking", "Squealing", "Grinding", "Vibration", "Pulling", "Wobbling", "Leaking",
                "Smoking", "Burning-Smell", "Electrical-Issue", "AC-Problem", "Heating-Issue"
            };

            foreach (var tag in problemTags)
            {
                tags.Add(new Tag(tag.Replace("-", " "), tag.ToLower()));
            }

            // General tags
            var generalTags = new[]
            {
                "Beginner", "DIY", "Professional", "Warranty", "Insurance", "Finance", "Lease",
                "Buy", "Sell", "Trade", "Review", "Recommendation", "Advice", "Help", "Question",
                "Discussion", "News", "Update", "Recall", "Safety", "Reliability", "Fuel-Economy",
                "Comfort", "Luxury", "Budget", "Affordable", "Expensive", "Rare", "Limited-Edition"
            };

            foreach (var tag in generalTags)
            {
                tags.Add(new Tag(tag.Replace("-", " "), tag.ToLower()));
            }

            // Simulate usage counts for more realistic data
            var random = new Random();
            foreach (var tag in tags)
            {
                // Simulate usage - some tags are more popular than others
                var baseUsage = random.Next(1, 50);
                
                // Make car brands more popular
                if (carBrands.Contains(tag.Name))
                    baseUsage += random.Next(50, 200);
                
                // Make common maintenance items popular
                if (maintenanceTags.Any(mt => mt.Replace("-", " ") == tag.Name))
                    baseUsage += random.Next(20, 100);
                
                // Make performance tags moderately popular
                if (performanceTags.Any(pt => pt.Replace("-", " ") == tag.Name))
                    baseUsage += random.Next(10, 80);

                // Simulate the usage by calling IncrementUsage
                for (int i = 0; i < baseUsage; i++)
                {
                    tag.IncrementUsage();
                }
            }

            await _context.Tags.AddRangeAsync(tags);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully seeded {Count} tags", tags.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding tags");
            throw;
        }
    }
}
