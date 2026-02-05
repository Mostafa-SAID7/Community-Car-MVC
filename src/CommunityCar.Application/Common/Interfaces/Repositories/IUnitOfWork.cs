using CommunityCar.Application.Common.Interfaces.Repositories.Account;
using CommunityCar.Application.Common.Interfaces.Repositories.Chat;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Application.Common.Interfaces.Repositories.AI;
using CommunityCar.Application.Common.Interfaces.Repositories.Localization;
using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Analytics.Content;
using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Analytics.Users.Behavior;
using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Analytics.Users.Segments;
using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Analytics.Users.Preferences;
using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Management.Users.Core;
using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Management.Users.Actions;
using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Overview.Users.Statistics;
using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Overview.Users.Security;
using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Reports.Users.General;
using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Reports.Users.Security;
using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Reports.Users.Audit;

namespace CommunityCar.Application.Common.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IConversationRepository Conversations { get; }
    IMessageRepository Messages { get; }
    IQARepository QA { get; }
    IVoteRepository Votes { get; }
    IViewRepository Views { get; }
    IBookmarkRepository Bookmarks { get; }
    IReactionRepository Reactions { get; }
    ICommentRepository Comments { get; }
    IShareRepository Shares { get; }
    ICategoryRepository Categories { get; }
    ITagRepository Tags { get; }
    IPointOfInterestRepository PointsOfInterest { get; }
    ICheckInRepository CheckIns { get; }
    IRouteRepository Routes { get; }
    INewsRepository News { get; }
    IReviewsRepository Reviews { get; }
    IStoriesRepository Stories { get; }
    IRatingRepository Ratings { get; }
    IEventsRepository Events { get; }
    IGroupsRepository Groups { get; }
    IPostsRepository Posts { get; }
    IGuidesRepository Guides { get; }
    ILocalizationCultureRepository LocalizationCultures { get; }
    ILocalizationResourceRepository LocalizationResources { get; }
    
    // Dashboard Repositories - Analytics
    IContentAnalyticsRepository ContentAnalytics { get; }
    IUserBehaviorAnalyticsRepository UserBehaviorAnalytics { get; }
    IUserSegmentRepository UserSegments { get; }
    IUserPreferencesRepository UserPreferences { get; }
    
    // Dashboard Repositories - Management
    CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Management.Users.Core.IUserManagementRepository UserManagement { get; }
    IUserManagementActionsRepository UserManagementActions { get; }
    
    // Dashboard Repositories - Overview
    IUserOverviewStatisticsRepository UserOverviewStatistics { get; }
    IUserOverviewSecurityRepository UserOverviewSecurity { get; }
    
    // Dashboard Repositories - Reports
    IUserReportsRepository UserReports { get; }
    IUserSecurityReportsRepository UserSecurityReports { get; }
    IUserAuditReportsRepository UserAuditReports { get; }
    
    // Dashboard Repositories - System
    // IAuditRepository Audit { get; }
    // IPerformanceRepository Performance { get; }
    // ISystemRepository System { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}


