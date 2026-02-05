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
using CommunityCar.Application.Services.AI;
using CommunityCar.Application.Services.Storage;
using CommunityCar.Application.Services.Shared;

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
        services.AddScoped<IContentModerationService, ContentModerationService>();
        services.AddScoped<INewsService, NewsService>();
        services.AddScoped<IPostsService, PostsService>();
        services.AddScoped<IQAService, QAService>();
        services.AddScoped<IReviewsService, ReviewsService>();
        services.AddScoped<IStoriesService, StoriesService>();
        services.AddScoped<IBroadcastService, BroadcastService>();

        // Shared Services
        services.AddScoped<ISharedSearchService, SharedSearchService>();
        services.AddScoped<Common.Interfaces.Services.Shared.IFileStorageService, Services.Shared.FileStorageService>();
        services.AddScoped<Common.Interfaces.Services.Storage.IFileStorageService, Services.Storage.FileStorageService>();

        // AI Services
        services.AddScoped<IAIManagementService, AIManagementService>();

        return services;
    }
}