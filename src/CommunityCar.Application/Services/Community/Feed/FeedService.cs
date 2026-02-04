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
        // Return a basic feed with mock data
        return await GetPersonalizedFeedAsync(Guid.NewGuid(), 1, 20);
    }

    public async Task<FeedVM> GetPersonalizedFeedAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        // Create request from parameters
        var request = new FeedVM
        {
            UserId = userId,
            Page = page,
            PageSize = pageSize,
            SortBy = FeedSortBy.Newest
        };

        // Get user's interests and preferences
        var userInterests = await _utilityService.GetUserInterestsAsync(userId);
        var friendIds = await _utilityService.GetUserFriendIdsAsync(userId);

        // Get personalized content
        var feedItems = await _contentAggregatorService.GetPersonalizedContentAsync(request, userInterests, friendIds);

        // Apply sorting and pagination
        feedItems = _utilityService.ApplySorting(feedItems, request.SortBy);
        var totalCount = feedItems.Count;
        var pagedItems = _utilityService.ApplyPagination(feedItems, request.Page, request.PageSize);
        
        // Load initial comments for each feed item
        await _interactionService.LoadInitialCommentsAsync(pagedItems, request.UserId);

        // Get additional feed data
        var activeStories = await _utilityService.GetActiveStoriesAsync(userId);
        var trendingTopics = await _utilityService.GetTrendingTopicsAsync(10);
        var suggestedFriends = await _utilityService.GetSuggestedFriendsAsync(userId, 5);

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

    public async Task<TrendingFeedVM> GetTrendingFeedAsync(int page = 1, int pageSize = 20)
    {
        // Create request from parameters
        var request = new FeedVM
        {
            Page = page,
            PageSize = pageSize,
            SortBy = FeedSortBy.Trending
        };

        // Get trending content
        var feedItems = await _contentAggregatorService.GetTrendingContentAsync(request);

        // Sort by trending score
        feedItems = feedItems.OrderByDescending(x => x.RelevanceScore).ToList();
        var totalCount = feedItems.Count;
        var pagedItems = _utilityService.ApplyPagination(feedItems, request.Page, request.PageSize);
        
        // Load initial comments for each feed item
        await _interactionService.LoadInitialCommentsAsync(pagedItems, request.UserId);

        return new TrendingFeedVM
        {
            FeedItems = pagedItems,
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

    public async Task<FeedVM> GetFriendsFeedAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        // Create request from parameters
        var request = new FeedVM
        {
            UserId = userId,
            Page = page,
            PageSize = pageSize,
            SortBy = FeedSortBy.Newest
        };

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

    public async Task<bool> ReportContentAsync(Guid contentId, string contentType, string reason)
        => await _interactionService.ReportContentAsync(Guid.Empty, contentId, contentType, reason);

    public async Task<FeedStatsVM> GetFeedStatsAsync()
        => await _utilityService.GetFeedStatsAsync(null);

    public async Task<bool> MarkAsSeenAsync(Guid userId, Guid contentId)
        => await _interactionService.MarkAsSeenAsync(userId, contentId, "Post");
}