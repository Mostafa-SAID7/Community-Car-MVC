using CommunityCar.Application.Features.Feed.DTOs;
using CommunityCar.Application.Features.Feed.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Community;

public interface IFeedService
{
    Task<FeedResponse> GetPersonalizedFeedAsync(FeedRequest request);
    Task<FeedResponse> GetTrendingFeedAsync(FeedRequest request);
    Task<FeedResponse> GetFriendsFeedAsync(FeedRequest request);
    Task<IEnumerable<StoryFeedVM>> GetActiveStoriesAsync(Guid? userId = null);
    Task<IEnumerable<TrendingTopicVM>> GetTrendingTopicsAsync(int count = 10);
    Task<IEnumerable<SuggestedFriendVM>> GetSuggestedFriendsAsync(Guid userId, int count = 5);
    Task<FeedStatsVM> GetFeedStatsAsync(Guid? userId = null);
    Task<bool> MarkAsSeenAsync(Guid userId, Guid contentId, string contentType);
    Task<bool> InteractWithContentAsync(Guid userId, Guid contentId, string contentType, string interactionType);
    Task<bool> AddCommentAsync(Guid userId, Guid contentId, string contentType, string comment);
    Task<IEnumerable<object>> GetCommentsAsync(Guid contentId, string contentType);
    Task<bool> ReportContentAsync(Guid userId, Guid contentId, string contentType, string reason);
    Task<IEnumerable<FeedItemVM>> GetPopularContentAsync(int hours);
    Task<int> CleanupExpiredStoriesAsync();
    Task<FeedStatsVM> GetFeedStatisticsAsync();
}


