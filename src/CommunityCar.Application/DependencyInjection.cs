using System.Reflection;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Localization;
using CommunityCar.Application.Services.Community;
using CommunityCar.Application.Services.Localization;
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
        return services;
    }
}
