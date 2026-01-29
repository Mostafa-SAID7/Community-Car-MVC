using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.Entities.Community.Events;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding.Community.Events;

public class EventSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<EventSeeder> _logger;

    public EventSeeder(
        ApplicationDbContext context,
        UserManager<User> userManager,
        ILogger<EventSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _context.Events.AnyAsync()) return;

        _logger.LogInformation("Seeding events...");

        var users = await _userManager.Users.ToListAsync();
        if (!users.Any()) return;

        var random = new Random();
        var events = new List<Event>();

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

            var eventItem = new Event(
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
}