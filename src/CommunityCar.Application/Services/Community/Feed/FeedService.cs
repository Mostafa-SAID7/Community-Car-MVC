using AutoMapper;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Services.Community.Feed;
using CommunityCar.Application.Features.Community.Feed.ViewModels;
using CommunityCar.Application.Features.Shared.ViewModels;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Services.Community.Feed;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Services.Community.Feed;

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

    public async Task<object> GetFeedAsync()
    {
        var request = new FeedVM
        {
            Page = 1,
            PageSize = 20,
            SortBy = FeedSortBy.Newest
        };

        return await GetPersonalizedFeedAsync(request);
    }

    public async Task<FeedVM> GetPersonalizedFeedAsync(FeedVM request)
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

        return new FeedVM
        {
            FeedItems = pagedItems,
            Stories = activeStories.Select(s => new StoryItemVM
            {
                Id = s.Id,
                AuthorId = s.AuthorId,
                AuthorName = s.AuthorName,
                AuthorAvatar = s.AuthorAvatar ?? string.Empty,
                MediaUrl = s.MediaUrl,
                ThumbnailUrl = s.ThumbnailUrl,
                CreatedAt = s.CreatedAt,
                ExpiresAt = s.ExpiresAt,
                IsViewed = s.IsViewed,
                ViewCount = s.ViewCount
            }).ToList(),
            TrendingTopics = trendingTopics.Select(t => new TrendingTopicVM
            {
                Topic = t.Topic,
                PostCount = t.PostCount,
                TimeAgo = t.TimeAgo,
                TrendingReason = t.TrendingReason,
                TopicAr = t.TopicAr,
                Category = t.Category,
                EngagementCount = t.EngagementCount,
                TrendingScore = t.TrendingScore,
                LastActivityAt = t.LastActivityAt
            }).ToList(),
            SuggestedFriends = suggestedFriends.Select(f => new FriendSuggestionVM
            {
                UserId = f.UserId,
                FullName = f.Name,
                ProfilePictureUrl = f.Avatar,
                MutualFriendsCount = f.MutualFriendsCount
            }).ToList(),
            Pagination = new CommunityCar.Application.Common.Models.PaginationInfo
            {
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
                HasPreviousPage = request.Page > 1,
                HasNextPage = request.Page < (int)Math.Ceiling((double)totalCount / request.PageSize),
                StartItem = (request.Page - 1) * request.PageSize + 1,
                EndItem = Math.Min(request.Page * request.PageSize, totalCount)
            },
            Stats = await _utilityService.GetFeedStatsAsync(request.UserId),
            HasMoreContent = (request.Page * request.PageSize) < totalCount
        };
    }

    public async Task<FeedVM> GetTrendingFeedAsync(FeedVM request)
    {
        // Get trending content
        var feedItems = await _contentAggregatorService.GetTrendingContentAsync(request);

        // Sort by trending score
        feedItems = feedItems.OrderByDescending(x => x.RelevanceScore).ToList();
        var totalCount = feedItems.Count;
        var pagedItems = _utilityService.ApplyPagination(feedItems, request.Page, request.PageSize);
        
        // Load initial comments for each feed item
        await _interactionService.LoadInitialCommentsAsync(pagedItems, request.UserId);

        return new FeedVM
        {
            FeedItems = pagedItems,
            Stories = (await _utilityService.GetActiveStoriesAsync(request.UserId)).Select(s => new StoryItemVM
            {
                Id = s.Id,
                AuthorId = s.AuthorId,
                AuthorName = s.AuthorName,
                AuthorAvatar = s.AuthorAvatar ?? string.Empty,
                MediaUrl = s.MediaUrl,
                ThumbnailUrl = s.ThumbnailUrl,
                CreatedAt = s.CreatedAt,
                ExpiresAt = s.ExpiresAt,
                IsViewed = s.IsViewed,
                ViewCount = s.ViewCount
            }).ToList(),
            TrendingTopics = (await _utilityService.GetTrendingTopicsAsync(15)).ToList(),
            Pagination = new CommunityCar.Application.Common.Models.PaginationInfo
            {
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
                HasPreviousPage = request.Page > 1,
                HasNextPage = request.Page < (int)Math.Ceiling((double)totalCount / request.PageSize),
                StartItem = (request.Page - 1) * request.PageSize + 1,
                EndItem = Math.Min(request.Page * request.PageSize, totalCount)
            },
            Stats = await _utilityService.GetFeedStatsAsync(request.UserId),
            HasMoreContent = (request.Page * request.PageSize) < totalCount
        };
    }

    public async Task<FeedVM> GetFriendsFeedAsync(FeedVM request)
    {
        if (!request.UserId.HasValue)
            return new FeedVM();

        var friendIds = await _utilityService.GetUserFriendIdsAsync(request.UserId);
        
        // Get content from friends only
        var feedItems = await _contentAggregatorService.GetFriendsContentAsync(request, friendIds);

        var totalCount = feedItems.Count;
        var pagedItems = _utilityService.ApplyPagination(feedItems, request.Page, request.PageSize);
        
        // Load initial comments for each feed item
        await _interactionService.LoadInitialCommentsAsync(pagedItems, request.UserId);

        return new FeedVM
        {
            FeedItems = pagedItems,
            Stories = (await _utilityService.GetActiveStoriesAsync(request.UserId)).Select(s => new StoryItemVM
            {
                Id = s.Id,
                AuthorId = s.AuthorId,
                AuthorName = s.AuthorName,
                AuthorAvatar = s.AuthorAvatar ?? string.Empty,
                MediaUrl = s.MediaUrl,
                ThumbnailUrl = s.ThumbnailUrl,
                CreatedAt = s.CreatedAt,
                ExpiresAt = s.ExpiresAt,
                IsViewed = s.IsViewed,
                ViewCount = s.ViewCount
            }).ToList(),
            TrendingTopics = (await _utilityService.GetTrendingTopicsAsync(5)).ToList(),
            SuggestedFriends = (await _utilityService.GetSuggestedFriendsAsync(request.UserId.Value, 3)).Select(f => new FriendSuggestionVM
            {
                UserId = f.UserId,
                FullName = f.Name,
                ProfilePictureUrl = f.Avatar,
                MutualFriendsCount = f.MutualFriendsCount
            }).ToList(),
            Pagination = new CommunityCar.Application.Common.Models.PaginationInfo
            {
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
                HasPreviousPage = request.Page > 1,
                HasNextPage = request.Page < (int)Math.Ceiling((double)totalCount / request.PageSize),
                StartItem = (request.Page - 1) * request.PageSize + 1,
                EndItem = Math.Min(request.Page * request.PageSize, totalCount)
            },
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

    public async Task<FeedStatsVM> GetFeedStatisticsAsync()
        => await _utilityService.GetFeedStatsAsync(null);

    public async Task<int> CleanupExpiredStoriesAsync()
        => await _utilityService.CleanupExpiredStoriesAsync();

    public async Task<IEnumerable<FeedItemVM>> GetPopularContentAsync(int hours)
        => await _contentAggregatorService.GetPopularContentAsync(hours);

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