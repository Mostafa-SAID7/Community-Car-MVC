using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding.Account.Core;

public class UserSeeder
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<UserSeeder> _logger;

    public UserSeeder(UserManager<User> userManager, ILogger<UserSeeder> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAsync()
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
            await _userManager.AddToRoleAsync(seedUser, Roles.SuperAdmin);
            users.Add(seedUser);
        }

        // Add specialized staff users
        await CreateStaffUserAsync("content@communitycar.com", "Content Admin", Roles.ContentAdmin, users);
        await CreateStaffUserAsync("design@communitycar.com", "Design Admin", Roles.DesignAdmin, users);
        await CreateStaffUserAsync("db@communitycar.com", "DB Admin", Roles.DatabaseAdmin, users);

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

    private async Task CreateStaffUserAsync(string email, string fullName, string role, List<User> usersList)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new User(email, email.Split('@')[0])
            {
                FullName = fullName,
                EmailConfirmed = true,
                City = "Cairo",
                Country = "Egypt",
                Bio = $"Staff member: {role}"
            };
            var result = await _userManager.CreateAsync(user, "Password123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                usersList.Add(user);
                _logger.LogInformation($"Created staff user: {user.UserName} with role {role}");
            }
        }
    }
}