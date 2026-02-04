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
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.SoftDelete;
using CommunityCar.Application.Common.Interfaces.Services.Community.Stories;
using CommunityCar.Application.Common.Interfaces.Services.Community.Broadcast;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Localization;
using CommunityCar.Application.Common.Interfaces.Services.Account.Core;
using CommunityCar.Application.Common.Interfaces.Services.Account.Management;
using CommunityCar.Application.Common.Interfaces.Services.Account.Security;
using CommunityCar.Application.Common.Interfaces.Services.Account.Profile;
using CommunityCar.Application.Common.Interfaces.Services.Account.Media;
using CommunityCar.Application.Common.Interfaces.Services.Account.Gamification;
using CommunityCar.Application.Common.Interfaces.Services.Account.Authentication.OAuth;
using CommunityCar.Application.Common.Interfaces.Services.Account.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Authentication;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Analytics;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Audit;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.ErrorReporting;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Maintenance;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Management;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Monitoring;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Overview;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Performance;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Reports;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Security;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.SEO;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Settings;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.System;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.UserManagement;
using CommunityCar.Application.Common.Interfaces.Services.Communication;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Caching;
using CommunityCar.Application.Common.Interfaces.Services.Storage;
using CommunityCar.Application.Common.Interfaces.Services.Shared;
using CommunityCar.Application.Common.Interfaces.Services.AI;
using CommunityCar.Application.Services.Community.Events;
using CommunityCar.Application.Services.Community.Groups;
using CommunityCar.Application.Services.Community.News;
using CommunityCar.Application.Services.Community.Posts;
using CommunityCar.Application.Services.Community.QA;
using CommunityCar.Application.Services.Community.Reviews;
using CommunityCar.Application.Services.Community.Stories;
using CommunityCar.Application.Services.Community.Maps;
using CommunityCar.Application.Services.Community.Feed;
using CommunityCar.Application.Services.Community.Friends;
using CommunityCar.Application.Services.Community.Guides;
using CommunityCar.Application.Services.Community.Moderation;
using CommunityCar.Application.Services.Community.Interactions;
using CommunityCar.Application.Services.Dashboard.SoftDelete;
using CommunityCar.Application.Services.Dashboard.Localization;
using CommunityCar.Application.Services.Account.Core;
using CommunityCar.Application.Services.Account.Management;
using CommunityCar.Application.Services.Account.Security;
using CommunityCar.Application.Services.Account.Profile;
using CommunityCar.Application.Services.Account.Media;
using CommunityCar.Application.Services.Account.Gamification;
using CommunityCar.Application.Services.Account.Authentication;
using CommunityCar.Application.Services.Account.Authentication.OAuth;
using CommunityCar.Application.Services.Account.Authorization;
using CommunityCar.Application.Services.Dashboard.Analytics;
using CommunityCar.Application.Services.Dashboard.Audit;
using CommunityCar.Application.Services.Dashboard.ErrorReporting;
using CommunityCar.Application.Services.Dashboard.Maintenance;
using CommunityCar.Application.Services.Dashboard.Management;
using CommunityCar.Application.Services.Dashboard.Monitoring;
using CommunityCar.Application.Services.Dashboard.Overview;
using CommunityCar.Application.Services.Dashboard.Performance;
using CommunityCar.Application.Services.Dashboard.Reports;
using CommunityCar.Application.Services.Dashboard.Security;
using CommunityCar.Application.Services.Dashboard.Settings;
using CommunityCar.Application.Services.Dashboard.System;
using CommunityCar.Application.Services.Dashboard.UserManagement;
using CommunityCar.Application.Services.SEO;
using CommunityCar.Application.Services.Communication;
using CommunityCar.Application.Services.Dashboard.Caching;
using CommunityCar.Application.Services.Storage;
using CommunityCar.Application.Services.Shared;
using CommunityCar.Application.Services.AI;
using CommunityCar.Application.Services.AI.ModelManagement;
using CommunityCar.Application.Services.AI.Training;
using CommunityCar.Application.Services.AI.History;
using CommunityCar.Application.Services.Maps.Routing;
using CommunityCar.Application.Services.Maps.Pricing;
using CommunityCar.Application.Services.Dashboard.BackgroundJobs;

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
        
        // Soft Delete Service
        services.AddScoped<ISoftDeleteService, SoftDeleteService>();

        // Focused Feed Services
        services.AddScoped<IFeedContentAggregatorService, FeedContentAggregatorService>();
        services.AddScoped<IFeedInteractionService, FeedInteractionService>();
        services.AddScoped<IFeedUtilityService, FeedUtilityService>();
        services.AddScoped<ILocalizationService, LocalizationService>();

        // Account services
        services.AddScoped<ICurrentUserService, CurrentUserService>();
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
        services.AddScoped<IOverviewService, CommunityCar.Application.Services.Dashboard.Overview.OverviewService>();
        services.AddScoped<IAnalyticsService, CommunityCar.Application.Services.Dashboard.Analytics.AnalyticsService>();
        services.AddScoped<IReportsService, CommunityCar.Application.Services.Dashboard.Reports.ReportsService>();
        services.AddScoped<IMonitoringService, CommunityCar.Application.Services.Dashboard.Monitoring.MonitoringService>();
        services.AddScoped<IMaintenanceService, CommunityCar.Application.Services.Dashboard.Maintenance.MaintenanceService>();
        services.AddScoped<IManagementService, CommunityCar.Application.Services.Dashboard.Management.ManagementService>();
        services.AddScoped<ISettingsService, CommunityCar.Application.Services.Dashboard.Settings.SettingsService>();
        services.AddScoped<IPerformanceService, CommunityCar.Application.Services.Dashboard.Performance.PerformanceService>();
        services.AddScoped<ISecurityService, CommunityCar.Application.Services.Dashboard.Security.SecurityService>();
        services.AddScoped<IAuditService, CommunityCar.Application.Services.Dashboard.Audit.AuditService>();
        services.AddScoped<ISystemManagementService, CommunityCar.Application.Services.Dashboard.System.SystemManagementService>();
        services.AddScoped<IUserManagementService, CommunityCar.Application.Services.Dashboard.UserManagement.UserManagementService>();
        
        // Error Management
        services.AddScoped<IErrorService, CommunityCar.Application.Services.Dashboard.ErrorReporting.ErrorService>();
        services.AddScoped<IErrorReportingService, CommunityCar.Application.Services.Dashboard.ErrorReporting.ErrorReportingService>();

        // SEO services
        services.AddScoped<ISEOService, SEOService>();

        // Background Job Services
        services.AddScoped<BackgroundJobSchedulerService>();
        services.AddScoped<GamificationBackgroundJobService>();
        services.AddScoped<MaintenanceBackgroundJobService>();
        services.AddScoped<FeedBackgroundJobService>();
        services.AddScoped<EmailBackgroundJobService>();



        return services;
    }
}


