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
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Application.Common.Interfaces.Data;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<UserProfile> UserProfiles { get; }
    
    // Shared
    DbSet<Comment> Comments { get; }
    DbSet<Vote> Votes { get; }
    DbSet<Reaction> Reactions { get; }
    DbSet<Rating> Ratings { get; }
    DbSet<Bookmark> Bookmarks { get; }
    DbSet<Tag> Tags { get; }
    DbSet<Category> Categories { get; }
    DbSet<View> Views { get; }

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

    // Dashboard
    DbSet<Metric> Metrics { get; }
    DbSet<Ticket> Tickets { get; }
    DbSet<LogEntry> LogEntries { get; }
    DbSet<GeneratedReport> GeneratedReports { get; }
    DbSet<SystemSetting> SystemSettings { get; }

    // Chats
    DbSet<Conversation> Conversations { get; }
    DbSet<Message> Messages { get; }

    // Localization
    DbSet<LocalizationCulture> LocalizationCultures { get; }
    DbSet<LocalizationResource> LocalizationResources { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}