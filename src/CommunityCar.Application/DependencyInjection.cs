using CommunityCar.Application.Common.Interfaces.Services;
using CommunityCar.Application.Common.Interfaces.Orchestrators;
using CommunityCar.Application.Orchestrators;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Localization;
using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using CommunityCar.Application.Common.Interfaces.Services.SEO;
using CommunityCar.Application.Services.Community;
using CommunityCar.Application.Services.Community.Feed;
using CommunityCar.Application.Services.Localization;
using CommunityCar.Application.Services.Account;
using CommunityCar.Application.Services.Identity;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Services.Dashboard;
using CommunityCar.Application.Services.SEO;
using CommunityCar.Application.Services.Maps.Routing;
using CommunityCar.Application.Services.Maps.Pricing;
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

        // Focused Feed Services
        services.AddScoped<IFeedContentAggregatorService, FeedContentAggregatorService>();
        services.AddScoped<IFeedInteractionService, FeedInteractionService>();
        services.AddScoped<IFeedUtilityService, FeedUtilityService>();
        services.AddScoped<ILocalizationService, LocalizationService>();

        // Unified Account services
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IAccountSecurityService, AccountSecurityService>();
        services.AddScoped<IAccountManagementService, AccountManagementService>();
        services.AddScoped<IUserGalleryService, UserGalleryService>();
        services.AddScoped<IGamificationService, GamificationService>();

        // Identity Management Services
        services.AddScoped<IIdentityManagementService, IdentityManagementService>();
        services.AddScoped<IUserIdentityService, UserIdentityService>();
        services.AddScoped<IRoleManagementService, RoleManagementService>();
        services.AddScoped<IClaimsManagementService, ClaimsManagementService>();

        // Dashboard services
        services.AddScoped<IOverviewService, OverviewService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();
        services.AddScoped<IReportsService, ReportsService>();
        services.AddScoped<IMonitoringService, MonitoringService>();
        
        // Error Management
        services.AddScoped<IErrorService, CommunityCar.Application.Services.Dashboard.ErrorService>();
        services.AddScoped<IErrorReportingService, CommunityCar.Application.Services.ErrorReportingService>();
        services.AddScoped<IManagementService, ManagementService>();
        services.AddScoped<ISettingsService, SettingsService>();

        // SEO and Performance services
        services.AddScoped<ISEOService, SEOService>();
        services.AddScoped<IPerformanceService, PerformanceService>();

        // Background Job Services
        services.AddScoped<BackgroundJobSchedulerService>();

        // Orchestrators
        services.AddScoped<IIdentityOrchestrator, IdentityOrchestrator>();
        services.AddScoped<IAccountLifecycleOrchestrator, AccountLifecycleOrchestrator>();
        services.AddScoped<IAccountSecurityOrchestrator, AccountSecurityOrchestrator>();
        services.AddScoped<IProfileOrchestrator, ProfileOrchestrator>();

        return services;
    }
}


