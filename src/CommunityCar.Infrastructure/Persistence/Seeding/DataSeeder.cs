using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Seeding.Shared;
using CommunityCar.Infrastructure.Persistence.Seeding.AI;
using CommunityCar.Infrastructure.Persistence.Seeding.Account.Core;
using CommunityCar.Infrastructure.Persistence.Seeding.Account.Authorization;
using CommunityCar.Infrastructure.Persistence.Seeding.Account.Activity;
using CommunityCar.Infrastructure.Persistence.Seeding.Account.Gamification;
using CommunityCar.Infrastructure.Persistence.Seeding.Account.Social;
using CommunityCar.Infrastructure.Persistence.Seeding.Account.Media;
using CommunityCar.Infrastructure.Persistence.Seeding.Community.QA;
using CommunityCar.Infrastructure.Persistence.Seeding.Community.Social;
using CommunityCar.Infrastructure.Persistence.Seeding.Community.Content;
using CommunityCar.Infrastructure.Persistence.Seeding.Community.Maps;
using CommunityCar.Infrastructure.Persistence.Seeding.Community.Events;
using CommunityCar.Infrastructure.Persistence.Seeding.Community.Groups;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding;

public class DataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly ILogger<DataSeeder> _logger;
    private readonly IServiceProvider _serviceProvider;

    public DataSeeder(
        ApplicationDbContext context, 
        UserManager<User> userManager, 
        RoleManager<IdentityRole<Guid>> roleManager,
        ILogger<DataSeeder> logger, 
        IServiceProvider serviceProvider)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Starting database seeding...");

            // Seed Account-related data first
            await SeedAccountDataAsync();
            
            // Seed shared entities (categories, tags)
            await SeedSharedEntitiesAsync();
            
            // Seed community content
            await SeedCommunityDataAsync();
            
            // Seed shared interactions (reactions, comments, etc.)
            await SeedSharedInteractionsAsync();

            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task SeedAccountDataAsync()
    {
        try
        {
            _logger.LogInformation("Seeding Account data...");

            // Seed roles first
            var roleSeeder = new RoleSeeder(_roleManager, _serviceProvider.GetRequiredService<ILogger<RoleSeeder>>());
            await roleSeeder.SeedAsync();

            // Seed users
            var userSeeder = new UserSeeder(_userManager, _serviceProvider.GetRequiredService<ILogger<UserSeeder>>());
            await userSeeder.SeedAsync();

            // Seed user activities
            var activitySeeder = new UserActivitySeeder(_context, _userManager, _serviceProvider.GetRequiredService<ILogger<UserActivitySeeder>>());
            await activitySeeder.SeedAsync();

            // Seed gamification data
            var badgeSeeder = new BadgeSeeder(_context, _serviceProvider.GetRequiredService<ILogger<BadgeSeeder>>());
            await badgeSeeder.SeedAsync();

            var achievementSeeder = new AchievementSeeder(_context, _serviceProvider.GetRequiredService<ILogger<AchievementSeeder>>());
            await achievementSeeder.SeedAsync();

            // Seed social data
            var followingSeeder = new UserFollowingSeeder(_context, _userManager, _serviceProvider.GetRequiredService<ILogger<UserFollowingSeeder>>());
            await followingSeeder.SeedAsync();

            // Seed media data
            var gallerySeeder = new UserGallerySeeder(_context, _userManager, _serviceProvider.GetRequiredService<ILogger<UserGallerySeeder>>());
            await gallerySeeder.SeedAsync();

            _logger.LogInformation("Account data seeding completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding Account data");
            throw;
        }
    }

    private async Task SeedCommunityDataAsync()
    {
        try
        {
            _logger.LogInformation("Seeding Community data...");

            // Seed Q&A
            var qaSeeder = new QASeeder(_context, _userManager, _serviceProvider.GetRequiredService<ILogger<QASeeder>>());
            await qaSeeder.SeedAsync();

            // Seed social data (friendships and conversations)
            var friendshipSeeder = new FriendshipSeeder(_context, _userManager, _serviceProvider.GetRequiredService<ILogger<FriendshipSeeder>>());
            await friendshipSeeder.SeedAsync();

            var conversationSeeder = new ConversationSeeder(_context, _userManager, _serviceProvider.GetRequiredService<ILogger<ConversationSeeder>>());
            await conversationSeeder.SeedAsync();

            // Seed content (guides, news, reviews, posts)
            var guideSeeder = new GuideSeeder(_context, _userManager, _serviceProvider.GetRequiredService<ILogger<GuideSeeder>>());
            await guideSeeder.SeedAsync();

            var newsSeeder = new NewsSeeder(_context, _userManager, _serviceProvider.GetRequiredService<ILogger<NewsSeeder>>());
            await newsSeeder.SeedAsync();

            var reviewSeeder = new ReviewSeeder(_context, _userManager, _serviceProvider.GetRequiredService<ILogger<ReviewSeeder>>());
            await reviewSeeder.SeedAsync();

            var postSeeder = new PostSeeder(_context, _userManager, _serviceProvider.GetRequiredService<ILogger<PostSeeder>>());
            await postSeeder.SeedAsync();

            // Seed maps data
            var mapSeeder = new MapSeeder(_context, _userManager, _serviceProvider.GetRequiredService<ILogger<MapSeeder>>());
            await mapSeeder.SeedAsync();

            // Seed events
            var eventSeeder = new EventSeeder(_context, _userManager, _serviceProvider.GetRequiredService<ILogger<EventSeeder>>());
            await eventSeeder.SeedAsync();

            // Seed groups
            var groupSeeder = new GroupSeeder(_context, _userManager, _serviceProvider.GetRequiredService<ILogger<GroupSeeder>>());
            await groupSeeder.SeedAsync();

            _logger.LogInformation("Community data seeding completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding Community data");
            throw;
        }
    }

    private async Task SeedSharedEntitiesAsync()
    {
        try
        {
            _logger.LogInformation("Seeding shared entities...");

            // Seed categories
            var categorySeeder = new CategorySeeder(_context, _serviceProvider.GetRequiredService<ILogger<CategorySeeder>>());
            await categorySeeder.SeedAsync();

            // Seed tags
            var tagSeeder = new TagSeeder(_context, _serviceProvider.GetRequiredService<ILogger<TagSeeder>>());
            await tagSeeder.SeedAsync();

            _logger.LogInformation("Shared entities seeding completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding shared entities");
            throw;
        }
    }

    private async Task SeedSharedInteractionsAsync()
    {
        try
        {
            _logger.LogInformation("Seeding shared interactions...");

            var interactionSeeder = new SharedInteractionSeeder(_context, _serviceProvider.GetRequiredService<ILogger<SharedInteractionSeeder>>());
            await interactionSeeder.SeedAsync();

            _logger.LogInformation("Shared interactions seeding completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding shared interactions");
            throw;
        }

        // Seed AI models
        try
        {
            _logger.LogInformation("Starting AI models seeding...");
            
            await AIModelSeeder.SeedAsync(_context);
            
            _logger.LogInformation("AI models seeding completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding AI models");
            throw;
        }
    }
}