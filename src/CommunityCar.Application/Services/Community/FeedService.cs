using AutoMapper;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Features.Feed.DTOs;
using CommunityCar.Application.Features.Feed.ViewModels;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Services.Community.Feed;

namespace CommunityCar.Application.Services.Community;

/// <summary>
/// Orchestrator service for feed operations - coordinates focused feed services
/// </summary>
public class FeedService : IFeedService
{
    private readonly IFeedContentAggregatorService _contentAggregatorService;
    private readonly IFeedInteractionService _interactionService;
    private readonly IFeedUtilityService _utilityService;

    public FeedService(
        IFeedContentAggregatorService contentAggregatorService,
        IFeedInteractionService interactionService,
        IFeedUtilityService utilityService)
    {
        _contentAggregatorService = contentAggregatorService;
        _interactionService = interactionService;
        _utilityService = utilityService;
    }

    public async Task<FeedResponse> GetPersonalizedFeedAsync(FeedRequest request)
    {
        // Get user's interests and preferences
        var userInterests = await _utilityService.GetUserInterestsAsync(request.UserId);
        var friendIds = await _utilityService.GetUserFriendIdsAsync(request.UserId);

        // Get personalized content
        var feedItems = await _contentAggregatorService.GetPersonalizedContentAsync(request, userInterests, friendIds);

        // Apply sorting and pagination
        feedItems = _utilityService.ApplySorting(feedItems, request.SortBy);
        var totalCount = feedItems.Count;
        var pagedItems = _utilityService.ApplyPagination(feedItems, request.Page, request.PageSize);
        
        // Load initial comments for each feed item
        await _interactionService.LoadInitialCommentsAsync(pagedItems, request.UserId);

        // Get additional feed data
        var activeStories = await _utilityService.GetActiveStoriesAsync(request.UserId);
        var trendingTopics = await _utilityService.GetTrendingTopicsAsync(10);
        var suggestedFriends = request.UserId.HasValue 
            ? await _utilityService.GetSuggestedFriendsAsync(request.UserId.Value, 5) 
            : new List<SuggestedFriendVM>();

        return new FeedResponse
        {
            FeedItems = pagedItems,
            Stories = activeStories,
            TrendingTopics = trendingTopics,
            SuggestedFriends = suggestedFriends,
            Pagination = _utilityService.CreatePaginationInfo(request.Page, request.PageSize, totalCount),
            Stats = await _utilityService.GetFeedStatsAsync(request.UserId),
            HasMoreContent = (request.Page * request.PageSize) < totalCount
        };
    }

    public async Task<FeedResponse> GetTrendingFeedAsync(FeedRequest request)
    {
        // Get trending content
        var feedItems = await _contentAggregatorService.GetTrendingContentAsync(request);

        // Sort by trending score
        feedItems = feedItems.OrderByDescending(x => x.RelevanceScore).ToList();
        var totalCount = feedItems.Count;
        var pagedItems = _utilityService.ApplyPagination(feedItems, request.Page, request.PageSize);
        
        // Load initial comments for each feed item
        await _interactionService.LoadInitialCommentsAsync(pagedItems, request.UserId);

        return new FeedResponse
        {
            FeedItems = pagedItems,
            Stories = await _utilityService.GetActiveStoriesAsync(request.UserId),
            TrendingTopics = await _utilityService.GetTrendingTopicsAsync(15),
            Pagination = _utilityService.CreatePaginationInfo(request.Page, request.PageSize, totalCount),
            Stats = await _utilityService.GetFeedStatsAsync(request.UserId),
            HasMoreContent = (request.Page * request.PageSize) < totalCount
        };
    }

    public async Task<FeedResponse> GetFriendsFeedAsync(FeedRequest request)
    {
        if (!request.UserId.HasValue)
            return new FeedResponse();

        var friendIds = await _utilityService.GetUserFriendIdsAsync(request.UserId);
        
        // Get content from friends only
        var feedItems = await _contentAggregatorService.GetFriendsContentAsync(request, friendIds);

        var totalCount = feedItems.Count;
        var pagedItems = _utilityService.ApplyPagination(feedItems, request.Page, request.PageSize);
        
        // Load initial comments for each feed item
        await _interactionService.LoadInitialCommentsAsync(pagedItems, request.UserId);

        return new FeedResponse
        {
            FeedItems = pagedItems,
            Stories = await _utilityService.GetActiveStoriesAsync(request.UserId),
            TrendingTopics = await _utilityService.GetTrendingTopicsAsync(5),
            SuggestedFriends = await _utilityService.GetSuggestedFriendsAsync(request.UserId.Value, 3),
            Pagination = _utilityService.CreatePaginationInfo(request.Page, request.PageSize, totalCount),
            Stats = await _utilityService.GetFeedStatsAsync(request.UserId),
            HasMoreContent = (request.Page * request.PageSize) < totalCount
        };
    }

    #region Delegate to Utility Service

    public async Task<IEnumerable<StoryFeedVM>> GetActiveStoriesAsync(Guid? userId = null)
        => await _utilityService.GetActiveStoriesAsync(userId);

    public async Task<IEnumerable<TrendingTopicVM>> GetTrendingTopicsAsync(int count = 10)
        => await _utilityService.GetTrendingTopicsAsync(count);

    public Task<IEnumerable<SuggestedFriendVM>> GetSuggestedFriendsAsync(Guid userId, int count = 5)
        => _utilityService.GetSuggestedFriendsAsync(userId, count);

    public async Task<FeedStatsVM> GetFeedStatsAsync(Guid? userId = null)
        => await _utilityService.GetFeedStatsAsync(userId);

    #endregion

    #region Delegate to Interaction Service

    public Task<bool> MarkAsSeenAsync(Guid userId, Guid contentId, string contentType)
        => _interactionService.MarkAsSeenAsync(userId, contentId, contentType);

    public Task<bool> InteractWithContentAsync(Guid userId, Guid contentId, string contentType, string interactionType)
        => _interactionService.InteractWithContentAsync(userId, contentId, contentType, interactionType);

    public async Task<bool> AddCommentAsync(Guid userId, Guid contentId, string contentType, string comment)
        => await _interactionService.AddCommentAsync(userId, contentId, contentType, comment);

    public async Task<IEnumerable<object>> GetCommentsAsync(Guid contentId, string contentType)
        => await _interactionService.GetCommentsAsync(contentId, contentType);

    public async Task<bool> BookmarkContentAsync(Guid userId, Guid contentId, string contentType)
        => await _interactionService.BookmarkContentAsync(userId, contentId, contentType);

    public async Task<bool> HideContentAsync(Guid userId, Guid contentId, string contentType)
        => await _interactionService.HideContentAsync(userId, contentId, contentType);

    public async Task<bool> ReportContentAsync(Guid userId, Guid contentId, string contentType, string reason)
        => await _interactionService.ReportContentAsync(userId, contentId, contentType, reason);

    #endregion
}


