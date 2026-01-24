using System.Reflection;
using CommunityCar.Application.Common.Interfaces.Services;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Localization;
using CommunityCar.Application.Common.Interfaces.Services.Profile;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using CommunityCar.Application.Common.Interfaces.Services.SEO;
using CommunityCar.Application.Services.Community;
using CommunityCar.Application.Services.Localization;
using CommunityCar.Application.Services.Profile;
using CommunityCar.Application.Services.Dashboard;
using CommunityCar.Application.Services.SEO;
using Microsoft.Extensions.DependencyInjection;

namespace CommunityCar.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register application services here
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);
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
        services.AddScoped<ILocalizationService, LocalizationService>();

        // Profile services
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IProfileManagementService, ProfileManagementService>();
        services.AddScoped<IProfileSecurityService, ProfileSecurityService>();
        services.AddScoped<IProfileOAuthService, ProfileOAuthService>();
        services.AddScoped<IProfileEmailService, ProfileEmailService>();
        services.AddScoped<IProfileAccountService, ProfileAccountService>();
        services.AddScoped<IAccountManagementService, AccountManagementService>();

        // Dashboard services
        services.AddScoped<IOverviewService, OverviewService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();
        services.AddScoped<IReportsService, ReportsService>();
        services.AddScoped<IMonitoringService, MonitoringService>();
        
        // Error Management
        services.AddScoped<IErrorService, CommunityCar.Application.Services.Dashboard.ErrorService>();
        services.AddScoped<IManagementService, ManagementService>();
        services.AddScoped<ISettingsService, SettingsService>();

        // SEO and Performance services
        services.AddScoped<ISEOService, SEOService>();
        services.AddScoped<IPerformanceService, PerformanceService>();

        return services;
    }
}
