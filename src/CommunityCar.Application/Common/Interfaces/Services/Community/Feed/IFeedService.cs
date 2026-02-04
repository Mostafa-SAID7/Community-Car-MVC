using CommunityCar.Application.Features.Community.Feed.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Community.Feed;

public interface IFeedService
{
    Task<object> GetFeedAsync();
    Task<TrendingFeedVM> GetTrendingFeedAsync(int page = 1, int pageSize = 20);
    Task<FeedVM> GetFriendsFeedAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<FeedVM> GetPersonalizedFeedAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<bool> ReportContentAsync(Guid contentId, string contentType, string reason);
    Task<FeedStatsVM> GetFeedStatsAsync();
    Task<bool> MarkAsSeenAsync(Guid userId, Guid contentId);
}