using CommunityCar.Domain.Entities.Auth;
using CommunityCar.Domain.Entities.Community.QA;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding;

public class DataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(ApplicationDbContext context, UserManager<User> userManager, ILogger<DataSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            await SeedUsersAsync();
            await SeedQAAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

    private async Task SeedUsersAsync()
    {
        if (await _userManager.Users.AnyAsync()) return;

        var user = new User("seed@communitycar.com", "seed@communitycar.com")
        {
            FullName = "Seed User",
            EmailConfirmed = true,

        };

        await _userManager.CreateAsync(user, "Password123!");
    }

    private async Task SeedQAAsync()
    {
        if (await _context.Questions.AnyAsync()) return;

        var user = await _userManager.FindByEmailAsync("seed@communitycar.com");
        if (user == null) return;

        var questions = new List<Question>
        {
            new Question("How do I change my oil?", "I have a 2020 Honda Civic and I want to change the oil myself. What tools do I need?", user.Id),
            new Question("Best tires for snow?", "I live in Canada and need recommendations for winter tires.", user.Id),
            new Question("Check engine light is on", "My check engine light came on yesterday. The car seems to run fine. What should I do?", user.Id),
            new Question("How to improve fuel economy?", "With gas prices rising, I want to get better mileage. Any tips?", user.Id),
            new Question("Is it worth fixing a 20-year-old car?", "I have a 2005 Corolla with 200k miles. It needs a new transmission. Should I fix it?", user.Id)
        };

        await _context.Questions.AddRangeAsync(questions);
        await _context.SaveChangesAsync();

        var answers = new List<Answer>
        {
            new Answer("You'll need a 17mm wrench, an oil filter wrench, a drain pan, and obviously the oil and filter.", questions[0].Id, user.Id),
            new Answer("Michelin X-Ice are fantastic. Bridgestone Blizzaks are also very good but wear out faster.", questions[1].Id, user.Id),
            new Answer("Go to an auto parts store, they usually scan it for free.", questions[2].Id, user.Id)
        };

        await _context.Answers.AddRangeAsync(answers);
        await _context.SaveChangesAsync();
    }
}
