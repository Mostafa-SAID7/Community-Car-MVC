using System.Reflection;
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
        services.AddScoped<IStoriesService, StoriesService>();
        services.AddScoped<IFeedService, FeedService>();
        services.AddScoped<IInteractionService, InteractionService>();
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
        services.AddScoped<IDashboardOverviewService, DashboardOverviewService>();
        services.AddScoped<IDashboardAnalyticsService, DashboardAnalyticsService>();
        services.AddScoped<IDashboardReportsService, DashboardReportsService>();
        services.AddScoped<IDashboardMonitoringService, DashboardMonitoringService>();
        services.AddScoped<IDashboardManagementService, DashboardManagementService>();
        services.AddScoped<IDashboardSettingsService, DashboardSettingsService>();
        
        // SEO and Performance services
        services.AddScoped<ISEOService, SEOService>();
        services.AddScoped<IPerformanceService, PerformanceService>();
        
        return services;
    }
}
