using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CommunityCar.Application.Common.Interfaces.Data;
using CommunityCar.Domain.Entities.Auth;
using CommunityCar.Domain.Entities.Chats;
using CommunityCar.Domain.Entities.Community.Events;
using CommunityCar.Domain.Entities.Community.Friends;
using CommunityCar.Domain.Entities.Community.Groups;
using CommunityCar.Domain.Entities.Community.Guides;
using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Entities.Community.News;
using CommunityCar.Domain.Entities.Community.Posts;
using CommunityCar.Domain.Entities.Community.QA;
using CommunityCar.Domain.Entities.Community.Reviews;
using CommunityCar.Domain.Entities.Community.Stories;
using CommunityCar.Domain.Entities.Dashboard.Analytics;
using CommunityCar.Domain.Entities.Dashboard.Management;
using CommunityCar.Domain.Entities.Dashboard.Monitoring;
using CommunityCar.Domain.Entities.Dashboard.Reports;
using CommunityCar.Domain.Entities.Dashboard.Settings;
using CommunityCar.Domain.Entities.Localization;
using CommunityCar.Domain.Entities.Profile;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Base;
using CommunityCar.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;

namespace CommunityCar.Infrastructure.Persistence.Data;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public new DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<UserGallery> UserGalleries => Set<UserGallery>();
    public DbSet<UserBadge> UserBadges => Set<UserBadge>();
    public DbSet<UserAchievement> UserAchievements => Set<UserAchievement>();
    public DbSet<UserActivity> UserActivities => Set<UserActivity>();
    public DbSet<UserInterest> UserInterests => Set<UserInterest>();
    public DbSet<UserFollowing> UserFollowings => Set<UserFollowing>();
    
    // Shared
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Vote> Votes => Set<Vote>();
    public DbSet<Reaction> Reactions => Set<Reaction>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<Bookmark> Bookmarks => Set<Bookmark>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<View> Views => Set<View>();
    public DbSet<Share> Shares => Set<Share>();
    
    // Error Management
    public DbSet<ErrorLog> ErrorLogs => Set<ErrorLog>();
    public DbSet<ErrorOccurrence> ErrorOccurrences => Set<ErrorOccurrence>();

    // Community
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Answer> Answers => Set<Answer>();
    public DbSet<Friendship> Friendships => Set<Friendship>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Guide> Guides => Set<Guide>();
    public DbSet<Story> Stories => Set<Story>();
    public DbSet<NewsItem> News => Set<NewsItem>();
    public DbSet<PointOfInterest> PointsOfInterest => Set<PointOfInterest>();
    public DbSet<CheckIn> CheckIns => Set<CheckIn>();
    public DbSet<Route> Routes => Set<Route>();

    // Dashboard
    public DbSet<Metric> Metrics => Set<Metric>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<LogEntry> LogEntries => Set<LogEntry>();
    public DbSet<GeneratedReport> GeneratedReports => Set<GeneratedReport>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();

    // Chats
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ConversationParticipant> ConversationParticipants => Set<ConversationParticipant>();
    public DbSet<Message> Messages => Set<Message>();

    // Localization
    public DbSet<LocalizationCulture> LocalizationCultures => Set<LocalizationCulture>();
    public DbSet<LocalizationResource> LocalizationResources => Set<LocalizationResource>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Apply configurations from assembly
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Configure soft delete global query filters
        builder.ConfigureSoftDeleteFilter();

        // Basic configurations

        
        // Configure decimal properties to fix EF warnings
        builder.Entity<PointOfInterest>()
            .Property(p => p.PriceRange)
            .HasPrecision(18, 2);
            
        builder.Entity<Review>()
            .Property(r => r.PurchasePrice)
            .HasPrecision(18, 2);
        
        // Configure Route entity to ignore RouteWaypoint as a separate entity
        builder.Ignore<RouteWaypoint>();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Handle soft delete audit trail
        foreach (var entry in ChangeTracker.Entries<ISoftDeletable>())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.SoftDelete("System"); // You can get current user here
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}