using CommunityCar.Application.Features.Feed.DTOs;
using CommunityCar.Application.Features.Feed.ViewModels;

namespace CommunityCar.Application.Services.Community.Feed;

/// <summary>
/// Service responsible for aggregating content from different sources for feeds
/// </summary>
public interface IFeedContentAggregatorService
{
    /// <summary>
    /// Gets personalized content based on user interests and friends
    /// </summary>
    Task<List<FeedItemVM>> GetPersonalizedContentAsync(FeedRequest request, List<string> userInterests, List<Guid> friendIds);

    /// <summary>
    /// Gets trending content across all sources
    /// </summary>
    Task<List<FeedItemVM>> GetTrendingContentAsync(FeedRequest request);

    /// <summary>
    /// Gets content from user's friends only
    /// </summary>
    Task<List<FeedItemVM>> GetFriendsContentAsync(FeedRequest request, List<Guid> friendIds);

    /// <summary>
    /// Gets content from specific sources (news, reviews, QA, stories)
    /// </summary>
    Task<List<FeedItemVM>> GetContentFromSourceAsync(FeedRequest request, string contentType, List<string> userInterests, List<Guid> friendIds);
}


