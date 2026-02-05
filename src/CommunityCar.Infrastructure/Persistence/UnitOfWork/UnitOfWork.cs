using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Repositories.Account;
using CommunityCar.Application.Common.Interfaces.Repositories.Chat;
using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Application.Common.Interfaces.Repositories.Localization;
using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Analytics.Content;
using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Analytics.Users.Behavior;
using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Analytics.Users.Segments;
using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Analytics.Users.Preferences;
using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Management.Users.Core;
using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Management.Users.Actions;
using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Overview.Users.Statistics;
using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Overview.Users.Activity;
using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Overview.Users.Security;
using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Reports.Users.General;
using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Reports.Users.Security;
using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Reports.Users.Audit;
using CommunityCar.Infrastructure.Persistence.Data;

namespace CommunityCar.Infrastructure.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(
        ApplicationDbContext context,
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository,
        IQARepository qaRepository,
        IVoteRepository voteRepository,
        IViewRepository viewRepository,
        IBookmarkRepository bookmarkRepository,
        IReactionRepository reactionRepository,
        ICommentRepository commentRepository,
        IShareRepository shareRepository,
        ICategoryRepository categoryRepository,
        ITagRepository tagRepository,
        IPointOfInterestRepository pointOfInterestRepository,
        ICheckInRepository checkInRepository,
        IRouteRepository routeRepository,
        INewsRepository newsRepository,
        IReviewsRepository reviewsRepository,
        IStoriesRepository storiesRepository,
        IRatingRepository ratingRepository,
        IEventsRepository eventsRepository,
        IGroupsRepository groupsRepository,
        IPostsRepository postsRepository,
        IGuidesRepository guidesRepository,
        IUserRepository userRepository,
        ILocalizationCultureRepository localizationCultureRepository,
        ILocalizationResourceRepository localizationResourceRepository,
        // Dashboard repositories
        IContentAnalyticsRepository contentAnalyticsRepository,
        IUserBehaviorAnalyticsRepository userBehaviorAnalyticsRepository,
        IUserSegmentRepository userSegmentRepository,
        IUserPreferencesRepository userPreferencesRepository,
        CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Management.Users.Core.IUserManagementRepository userManagementRepository,
        IUserManagementActionsRepository userManagementActionsRepository,
        IUserOverviewStatisticsRepository userOverviewStatisticsRepository,
        IUserOverviewActivityRepository userOverviewActivityRepository,
        IUserOverviewSecurityRepository userOverviewSecurityRepository,
        IUserReportsRepository userReportsRepository,
        IUserSecurityReportsRepository userSecurityReportsRepository,
        IUserAuditReportsRepository userAuditReportsRepository)
    {
        _context = context;
        Conversations = conversationRepository;
        Messages = messageRepository;
        QA = qaRepository;
        Votes = voteRepository;
        Views = viewRepository;
        Bookmarks = bookmarkRepository;
        Reactions = reactionRepository;
        Comments = commentRepository;
        Shares = shareRepository;
        Categories = categoryRepository;
        Tags = tagRepository;
        PointsOfInterest = pointOfInterestRepository;
        CheckIns = checkInRepository;
        Routes = routeRepository;
        News = newsRepository;
        Reviews = reviewsRepository;
        Stories = storiesRepository;
        Ratings = ratingRepository;
        Events = eventsRepository;
        Groups = groupsRepository;
        Posts = postsRepository;
        Guides = guidesRepository;
        Users = userRepository;
        LocalizationCultures = localizationCultureRepository;
        LocalizationResources = localizationResourceRepository;
        
        // Dashboard repositories
        ContentAnalytics = contentAnalyticsRepository;
        UserBehaviorAnalytics = userBehaviorAnalyticsRepository;
        UserSegments = userSegmentRepository;
        UserPreferences = userPreferencesRepository;
        UserManagement = userManagementRepository;
        UserManagementActions = userManagementActionsRepository;
        UserOverviewStatistics = userOverviewStatisticsRepository;
        UserOverviewActivity = userOverviewActivityRepository;
        UserOverviewSecurity = userOverviewSecurityRepository;
        UserReports = userReportsRepository;
        UserSecurityReports = userSecurityReportsRepository;
        UserAuditReports = userAuditReportsRepository;
    }

    public IConversationRepository Conversations { get; }
    public IMessageRepository Messages { get; }
    public IQARepository QA { get; }
    public IVoteRepository Votes { get; }
    public IViewRepository Views { get; }
    public IBookmarkRepository Bookmarks { get; }
    public IReactionRepository Reactions { get; }
    public ICommentRepository Comments { get; }
    public IShareRepository Shares { get; }
    public ICategoryRepository Categories { get; }
    public ITagRepository Tags { get; }
    public IPointOfInterestRepository PointsOfInterest { get; }
    public ICheckInRepository CheckIns { get; }
    public IRouteRepository Routes { get; }
    public INewsRepository News { get; }
    public IReviewsRepository Reviews { get; }
    public IStoriesRepository Stories { get; }
    public IRatingRepository Ratings { get; }
    public IEventsRepository Events { get; }
    public IGroupsRepository Groups { get; }
    public IPostsRepository Posts { get; }
    public IGuidesRepository Guides { get; }
    public IUserRepository Users { get; }
    public ILocalizationCultureRepository LocalizationCultures { get; }
    public ILocalizationResourceRepository LocalizationResources { get; }
    
    // Dashboard Repositories - Analytics
    public IContentAnalyticsRepository ContentAnalytics { get; }
    public IUserBehaviorAnalyticsRepository UserBehaviorAnalytics { get; }
    public IUserSegmentRepository UserSegments { get; }
    public IUserPreferencesRepository UserPreferences { get; }
    
    // Dashboard Repositories - Management
    public CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Management.Users.Core.IUserManagementRepository UserManagement { get; }
    public IUserManagementActionsRepository UserManagementActions { get; }
    
    // Dashboard Repositories - Overview
    public IUserOverviewStatisticsRepository UserOverviewStatistics { get; }
    public IUserOverviewActivityRepository UserOverviewActivity { get; }
    public IUserOverviewSecurityRepository UserOverviewSecurity { get; }
    
    // Dashboard Repositories - Reports
    public IUserReportsRepository UserReports { get; }
    public IUserSecurityReportsRepository UserSecurityReports { get; }
    public IUserAuditReportsRepository UserAuditReports { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
