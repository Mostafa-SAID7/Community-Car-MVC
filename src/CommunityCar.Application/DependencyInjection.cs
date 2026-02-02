using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Localization;
using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Interfaces.Services.Account.Authentication.OAuth;
using CommunityCar.Application.Common.Interfaces.Services.Account.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Authentication;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using CommunityCar.Application.Common.Interfaces.Services.SEO;
using CommunityCar.Application.Common.Interfaces.Services.Communication;
using CommunityCar.Application.Common.Interfaces.Services.Caching;
using CommunityCar.Application.Common.Interfaces.Services.Storage;
using CommunityCar.Application.Common.Interfaces.Services.Shared;
using CommunityCar.Application.Common.Interfaces.Services.AI;
using CommunityCar.Application.Common.Interfaces.Services;
using CommunityCar.Application.Services.Community;
using CommunityCar.Application.Services.Community.Feed;
using CommunityCar.Application.Services.Localization;
using CommunityCar.Application.Services.Account;
using CommunityCar.Application.Services.Account.Authentication;
using CommunityCar.Application.Services.Account.Authentication.OAuth;
using CommunityCar.Application.Services.Account.Authorization;
using CommunityCar.Application.Services.Dashboard;
using CommunityCar.Application.Services.SEO;
using CommunityCar.Application.Services.Communication;
using CommunityCar.Application.Services.Caching;
using CommunityCar.Application.Services.Storage;
using CommunityCar.Application.Services.Shared;
using CommunityCar.Application.Services.AI;
using CommunityCar.Application.Services.AI.ModelManagement;
using CommunityCar.Application.Services.AI.Training;
using CommunityCar.Application.Services.AI.History;
using CommunityCar.Application.Services.Maps.Routing;
using CommunityCar.Application.Services.Maps.Pricing;
using CommunityCar.Application.Services.BackgroundJobs;

using Microsoft.Extensions.DependencyInjection;

namespace CommunityCar.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register application services here
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        
        // Community services
        services.AddScoped<IQAService, QAService>();
        services.AddScoped<IMapsService, MapsService>();
        services.AddScoped<INewsService, NewsService>();
        services.AddScoped<IReviewsService, ReviewsService>();
        services.AddScoped<IEventsService, EventsService>();
        services.AddScoped<IGroupsService, GroupsService>();
        services.AddScoped<IPostsService, PostsService>();
        services.AddScoped<IStoriesService, StoriesService>();
        services.AddScoped<IFeedService, FeedService>();
        services.AddScoped<IInteractionService, InteractionService>();
        services.AddScoped<IFriendsService, FriendsService>();
        services.AddScoped<IGuidesService, GuidesService>();
        services.AddScoped<IContentModerationService, ContentModerationService>();
        services.AddScoped<IGuidesNotificationService, GuidesNotificationService>();
        services.AddScoped<INewsNotificationService, NewsNotificationService>();

        // Focused Feed Services
        services.AddScoped<IFeedContentAggregatorService, FeedContentAggregatorService>();
        services.AddScoped<IFeedInteractionService, FeedInteractionService>();
        services.AddScoped<IFeedUtilityService, FeedUtilityService>();
        services.AddScoped<ILocalizationService, LocalizationService>();

        // Account services
        services.AddScoped<IOAuthService, OAuthService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IAccountSecurityService, AccountSecurityService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAccountManagementService, AccountManagementService>();
        services.AddScoped<IUserGalleryService, UserGalleryService>();
        services.AddScoped<IGamificationService, GamificationService>();
        services.AddScoped<IProgressionService, ProgressionService>();
        services.AddScoped<IProfileViewService, ProfileViewService>();
        services.AddScoped<IUserAnalyticsService, UserAnalyticsService>();

        // OAuth Services
        services.AddScoped<IGoogleOAuthService, GoogleOAuthService>();
        services.AddScoped<IFacebookOAuthService, FacebookOAuthService>();

        // TwoFactor Services
        services.AddScoped<ITwoFactorService, TwoFactorService>();

        // Authorization Services
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IPermissionService, PermissionService>();

        // Communication Services
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IChatService, ChatService>();

        // Storage Services
        services.AddScoped<IFileStorageService, FileStorageService>();

        // Shared Services
        services.AddScoped<ISharedSearchService, SharedSearchService>();

        // AI Services
        services.AddScoped<IAIManagementService, AIManagementService>();
        services.AddScoped<IModelManagementService, ModelManagementService>();
        services.AddScoped<ITrainingManagementService, TrainingManagementService>();
        services.AddScoped<ITrainingHistoryService, TrainingHistoryService>();

        // Dashboard Services
        services.AddScoped<IOverviewService, OverviewService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();
        services.AddScoped<IReportsService, ReportsService>();
        services.AddScoped<IMonitoringService, MonitoringService>();
        services.AddScoped<IMaintenanceService, MaintenanceService>();
        services.AddScoped<IManagementService, ManagementService>();
        services.AddScoped<ISettingsService, SettingsService>();
        
        // Error Management
        services.AddScoped<IErrorService, CommunityCar.Application.Services.Dashboard.ErrorService>();
        services.AddScoped<IErrorReportingService, CommunityCar.Application.Services.ErrorReportingService>();

        // SEO and Performance services
        services.AddScoped<ISEOService, SEOService>();
        services.AddScoped<IPerformanceService, PerformanceService>();

        // Background Job Services
        services.AddScoped<BackgroundJobSchedulerService>();
        services.AddScoped<GamificationBackgroundJobService>();
        services.AddScoped<MaintenanceBackgroundJobService>();
        services.AddScoped<FeedBackgroundJobService>();
        services.AddScoped<EmailBackgroundJobService>();



        return services;
    }
}


