using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.Entities.Account.Media;
using CommunityCar.Domain.Entities.Account.Gamification;
using CommunityCar.Domain.Entities.Account.Authentication;
using CommunityCar.Domain.Entities.Account.Analytics;
using CommunityCar.Domain.Entities.Account.Management;
using CommunityCar.Domain.Entities.Account.Authorization;
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
using CommunityCar.Domain.Entities.Dashboard.Settings;
using CommunityCar.Domain.Entities.Dashboard.System;
using CommunityCar.Domain.Entities.Localization;
using CommunityCar.Domain.Entities.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using CommunityCar.Domain.Entities.Account.Profile;
using UserActivityEntity = CommunityCar.Domain.Entities.Account.Core.UserActivity;

namespace CommunityCar.Application.Common.Interfaces.Data;

public interface IApplicationDbContext
{
    DatabaseFacade Database { get; }
    Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters);
    DbSet<User> Users { get; }
    DbSet<UserGallery> UserGalleries { get; }
    DbSet<UserBadge> UserBadges { get; }
    DbSet<UserToken> UserTokens { get; }
    DbSet<UserAchievement> UserAchievements { get; }
    DbSet<UserActivityEntity> UserActivities { get; }
    DbSet<UserInterest> UserInterests { get; }
    DbSet<UserFollowing> UserFollowings { get; }
    DbSet<UserProfileView> UserProfileViews { get; }
    DbSet<UserManagement> UserManagements { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<UserPermission> UserPermissions { get; }
    
    // Shared
    DbSet<Comment> Comments { get; }
    DbSet<Vote> Votes { get; }
    DbSet<Reaction> Reactions { get; }
    DbSet<Rating> Ratings { get; }
    DbSet<Bookmark> Bookmarks { get; }
    DbSet<Tag> Tags { get; }
    DbSet<Category> Categories { get; }
    DbSet<View> Views { get; }
    DbSet<Share> Shares { get; }
    
    // Error Management
    DbSet<ErrorLog> ErrorLogs { get; }

    // Community
    DbSet<Post> Posts { get; }
    DbSet<Group> Groups { get; }
    DbSet<Question> Questions { get; }
    DbSet<Answer> Answers { get; }
    DbSet<Friendship> Friendships { get; }
    DbSet<Review> Reviews { get; }
    DbSet<Event> Events { get; }
    DbSet<Guide> Guides { get; }
    DbSet<Story> Stories { get; }
    DbSet<NewsItem> News { get; }
    DbSet<PointOfInterest> PointsOfInterest { get; }
    DbSet<CheckIn> CheckIns { get; }
    DbSet<Route> Routes { get; }

    // Dashboard
    DbSet<Metric> Metrics { get; }
    DbSet<Ticket> Tickets { get; }
    DbSet<LogEntry> LogEntries { get; }
    DbSet<GeneratedReport> GeneratedReports { get; }
    DbSet<SystemSetting> SystemSettings { get; }

    // Chats
    DbSet<Conversation> Conversations { get; }
    DbSet<ConversationParticipant> ConversationParticipants { get; }
    DbSet<Message> Messages { get; }

    // Localization
    DbSet<LocalizationCulture> LocalizationCultures { get; }
    DbSet<LocalizationResource> LocalizationResources { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}


