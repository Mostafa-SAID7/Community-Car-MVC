using Microsoft.Extensions.DependencyInjection;
using CommunityCar.Application.Common.Interfaces.Services.Community.Events;
using CommunityCar.Application.Common.Interfaces.Services.Community.Feed;
using CommunityCar.Application.Common.Interfaces.Services.Community.Friends;
using CommunityCar.Application.Common.Interfaces.Services.Community.Groups;
using CommunityCar.Application.Common.Interfaces.Services.Community.Guides;
using CommunityCar.Application.Common.Interfaces.Services.Community.Interactions;
using CommunityCar.Application.Common.Interfaces.Services.Community.Maps;
using CommunityCar.Application.Common.Interfaces.Services.Community.Moderation;
using CommunityCar.Application.Common.Interfaces.Services.Community.News;
using CommunityCar.Application.Common.Interfaces.Services.Community.Posts;
using CommunityCar.Application.Common.Interfaces.Services.Community.QA;
using CommunityCar.Application.Common.Interfaces.Services.Community.Reviews;
using CommunityCar.Application.Common.Interfaces.Services.Community.Stories;
using CommunityCar.Application.Common.Interfaces.Services.Community.Broadcast;
using CommunityCar.Application.Common.Interfaces.Services.Account.Core;
using CommunityCar.Application.Common.Interfaces.Services.Account.Security;
using CommunityCar.Application.Common.Interfaces.Services.Account.Profile;
using CommunityCar.Application.Common.Interfaces.Services.Account.Media;
using CommunityCar.Application.Common.Interfaces.Services.Account.Gamification;
using CommunityCar.Application.Common.Interfaces.Services.Account.Authentication.OAuth;
using CommunityCar.Application.Common.Interfaces.Services.Account.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Authentication;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Analytics;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Analytics.Content;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Analytics.Users.Behavior;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Analytics.Users.Segments;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Analytics.Users.Preferences;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Management;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Management.Users.Core;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Management.Users.Actions;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Overview;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Overview.Users.Statistics;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Overview.Users.Security;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Reports;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Reports.Users.General;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Reports.Users.Security;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Reports.Users.Audit;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Settings;
using CommunityCar.Application.Common.Interfaces.Services.Communication;
using CommunityCar.Application.Common.Interfaces.Services.Shared;
using CommunityCar.Application.Common.Interfaces.Services.Account.Management;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Maintenance;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Performance;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.SEO;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.ErrorReporting;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Analytics.Users;
using CommunityCar.Application.Services.Community.Events;
using CommunityCar.Application.Services.Community.Feed;
using CommunityCar.Application.Services.Community.Friends;
using CommunityCar.Application.Services.Community.Groups;
using CommunityCar.Application.Services.Community.Guides;
using CommunityCar.Application.Services.Community.Interactions;
using CommunityCar.Application.Services.Community.Maps;
using CommunityCar.Application.Services.Community.Moderation;
using CommunityCar.Application.Services.Community.News;
using CommunityCar.Application.Services.Community.Posts;
using CommunityCar.Application.Services.Community.QA;
using CommunityCar.Application.Services.Community.Reviews;
using CommunityCar.Application.Services.Community.Stories;
using CommunityCar.Application.Services.Community.Broadcast;
using CommunityCar.Application.Services.Account.Core;
using CommunityCar.Application.Services.Account.Security;
using CommunityCar.Application.Services.Account.Profile;
using CommunityCar.Application.Services.Account.Media;
using CommunityCar.Application.Services.Account.Gamification;
using CommunityCar.Application.Services.Account.Authentication.OAuth;
using CommunityCar.Application.Services.Account.Authorization;
using CommunityCar.Application.Services.Dashboard.Analytics;
using CommunityCar.Application.Services.Dashboard.Analytics.Users.Behavior;
using CommunityCar.Application.Services.Dashboard.Analytics.Users.Segments;
using CommunityCar.Application.Services.Dashboard.Analytics.Users.Preferences;
using CommunityCar.Application.Services.Dashboard.Management;
using CommunityCar.Application.Services.Dashboard.Management.Users.Core;
using CommunityCar.Application.Services.Dashboard.Management.Users.Actions;
using CommunityCar.Application.Services.Dashboard.Overview;
using CommunityCar.Application.Services.Dashboard.Overview.Users.Statistics;
using CommunityCar.Application.Services.Dashboard.Overview.Users.Security;
using CommunityCar.Application.Services.Dashboard.Reports;
using CommunityCar.Application.Services.Dashboard.Reports.Users.General;
using CommunityCar.Application.Services.Dashboard.Reports.Users.Security;
using CommunityCar.Application.Services.Dashboard.Reports.Users.Audit;
using CommunityCar.Application.Services.Dashboard.Settings;
using CommunityCar.Application.Services.Communication;
using CommunityCar.Application.Services.Shared;
using CommunityCar.Application.Services.Account.Management;
using CommunityCar.Application.Services.AI;
using CommunityCar.Application.Services.Storage;
using CommunityCar.Application.Services.Dashboard.Maintenance;
using CommunityCar.Application.Services.Dashboard.Performance;
using CommunityCar.Application.Services.Dashboard.SEO;
using CommunityCar.Application.Services.Dashboard.ErrorReporting;

namespace CommunityCar.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Community Services
        services.AddScoped<IEventsService, EventsService>();
        services.AddScoped<IFeedService, FeedService>();
        services.AddScoped<IFriendsService, FriendsService>();
        services.AddScoped<IGroupsService, GroupsService>();
        services.AddScoped<IGuidesService, GuidesService>();
        // services.AddScoped<IInteractionsService, InteractionsService>(); // TODO: Create InteractionsService
        // services.AddScoped<CommunityCar.Application.Common.Interfaces.Services.Community.Maps.IMapsService, MapsService>(); // TODO: Create MapsService
        services.AddScoped<IContentModerationService, ContentModerationService>();
        services.AddScoped<INewsService, NewsService>();
        services.AddScoped<IPostsService, PostsService>();
        services.AddScoped<IQAService, QAService>();
        services.AddScoped<IReviewsService, ReviewsService>();
        services.AddScoped<IStoriesService, StoriesService>();
        services.AddScoped<IBroadcastService, BroadcastService>();

        // Account Services
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IAccountSecurityService, AccountSecurityService>();
        services.AddScoped<IAccountManagementService, AccountManagementService>();
        // services.AddScoped<IUserProfileService, UserProfileService>(); // TODO: Create UserProfileService
        services.AddScoped<IUserGalleryService, UserGalleryService>();
        // services.AddScoped<IUserGamificationService, UserGamificationService>(); // TODO: Create UserGamificationService
        services.AddScoped<IFacebookOAuthService, FacebookOAuthService>();
        services.AddScoped<IRoleService, RoleService>();

        // Dashboard Services
        services.AddScoped<IAnalyticsService, AnalyticsService>();
        services.AddScoped<IUserBehaviorAnalyticsService, UserBehaviorAnalyticsService>();
        services.AddScoped<IUserSegmentationService, UserSegmentationService>();
        services.AddScoped<IUserPreferencesAnalyticsService, UserPreferencesAnalyticsService>();
        services.AddScoped<IManagementService, ManagementService>();
        services.AddScoped<IUserManagementCoreService, UserManagementCoreService>();
        services.AddScoped<IUserManagementActionsService, UserManagementActionsService>();
        services.AddScoped<IOverviewService, OverviewService>();
        services.AddScoped<IUserOverviewStatisticsService, UserOverviewStatisticsService>();
        services.AddScoped<IUserOverviewSecurityService, UserOverviewSecurityService>();
        services.AddScoped<IReportsService, ReportsService>();
        services.AddScoped<IUserReportsService, UserReportsService>();
        services.AddScoped<IUserSecurityReportsService, UserSecurityReportsService>();
        services.AddScoped<IUserAuditReportsService, UserAuditReportsService>();
        services.AddScoped<ISettingsService, SettingsService>();

        // Communication Services
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        services.AddScoped<INotificationService, NotificationService>();

        // Shared Services
        services.AddScoped<ISharedSearchService, SharedSearchService>();
        services.AddScoped<Common.Interfaces.Services.Shared.IFileStorageService, Services.Shared.FileStorageService>();
        services.AddScoped<Common.Interfaces.Services.Storage.IFileStorageService, Services.Storage.FileStorageService>();

        // AI Services
        services.AddScoped<IAIManagementService, AIManagementService>();

        // Analytics Services
        services.AddScoped<IUserAnalyticsService, Services.Dashboard.Analytics.Users.UserAnalyticsService>();

        // Missing Dashboard Services
        services.AddScoped<IMaintenanceService, MaintenanceService>();
        services.AddScoped<IPerformanceService, PerformanceService>();
        services.AddScoped<ISEOService, SEOService>();
        services.AddScoped<IErrorService, ErrorService>();
        services.AddScoped<IUserPreferencesService, UserPreferencesService>();
        services.AddScoped<IUserSegmentService, UserSegmentService>();

        return services;
    }
}