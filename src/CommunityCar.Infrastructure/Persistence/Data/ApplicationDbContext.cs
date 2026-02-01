using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CommunityCar.Application.Common.Interfaces.Data;
using UserEntity = CommunityCar.Domain.Entities.Account.Core.User;
using UserActivityEntity = CommunityCar.Domain.Entities.Account.Core.UserActivity;
using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.Entities.Account.Gamification;
using CommunityCar.Domain.Entities.Account.Media;
using CommunityCar.Domain.Entities.Account.Profile;
using CommunityCar.Domain.Entities.Account.Management;
using CommunityCar.Domain.Entities.Account.Authentication;
using CommunityCar.Domain.Entities.Account.Authorization;
using CommunityCar.Domain.Entities.Account.Analytics;
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
using CommunityCar.Domain.Entities.Dashboard.Overview;
using CommunityCar.Domain.Entities.Dashboard.Settings;
using CommunityCar.Domain.Entities.Dashboard.System;
using CommunityCar.Domain.Entities.Localization;

using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Entities.AI;
using CommunityCar.Domain.Base;
using CommunityCar.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;

namespace CommunityCar.Infrastructure.Persistence.Data;

public class ApplicationDbContext : IdentityDbContext<UserEntity, Role, Guid>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        // Suppress the warning for pending model changes in EF Core 9.0
        optionsBuilder.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }

    public new DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<UserGallery> UserGalleries => Set<UserGallery>();
    public DbSet<UserBadge> UserBadges => Set<UserBadge>();
    public DbSet<UserToken> UserTokens => Set<UserToken>();
    public DbSet<UserAchievement> UserAchievements => Set<UserAchievement>();
    public DbSet<UserActivityEntity> UserActivities => Set<UserActivityEntity>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<UserInterest> UserInterests => Set<UserInterest>();
    public DbSet<UserFollowing> UserFollowings => Set<UserFollowing>();
    public DbSet<UserProfileView> UserProfileViews => Set<UserProfileView>();
    public DbSet<UserManagement> UserManagements => Set<UserManagement>();
    public DbSet<UserManagementAction> UserManagementActions => Set<UserManagementAction>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
    
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
    public DbSet<GeneratedReport> GeneratedReports => Set<GeneratedReport>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<LogEntry> LogEntries => Set<LogEntry>();

    // Chats
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ConversationParticipant> ConversationParticipants => Set<ConversationParticipant>();
    public DbSet<Message> Messages => Set<Message>();

    // Localization
    public DbSet<LocalizationCulture> LocalizationCultures => Set<LocalizationCulture>();
    public DbSet<LocalizationResource> LocalizationResources => Set<LocalizationResource>();

    // AI
    public DbSet<AIModel> AIModels => Set<AIModel>();
    public DbSet<TrainingJob> TrainingJobs => Set<TrainingJob>();
    public DbSet<TrainingHistory> TrainingHistories => Set<TrainingHistory>();

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

        builder.Entity<UserContentAnalytics>()
            .Property(u => u.EngagementRate)
            .HasPrecision(18, 4);

        builder.Entity<UserAnalytics>(entity =>
        {
            entity.Property(e => e.RetentionRate).HasPrecision(18, 4);
            entity.Property(e => e.ChurnRate).HasPrecision(18, 4);
            entity.Property(e => e.BounceRate).HasPrecision(18, 4);
        });

        builder.Entity<PointOfInterest>()
            .Property(p => p.PriceRange)
            .HasPrecision(18, 2);
        
        // Configure Route entity to ignore RouteWaypoint as a separate entity
        builder.Ignore<RouteWaypoint>();

        // User Slug Configuration
        builder.Entity<UserEntity>()
            .HasIndex(u => u.Slug)
            .IsUnique();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = "System"; // Ideally get from ICurrentUserService

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is ISoftDeletable softDeletable && entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                softDeletable.SoftDelete(currentUserId);
            }
            else if (entry.Entity is BaseEntity baseEntity)
            {
                if (entry.State == EntityState.Added)
                {
                    baseEntity.Audit(currentUserId);
                }
                else if (entry.State == EntityState.Modified)
                {
                    baseEntity.Audit(currentUserId);
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters)
    {
        return await Database.ExecuteSqlRawAsync(sql, parameters);
    }
}
