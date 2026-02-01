using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Features.Community.Feed.DTOs;
using CommunityCar.Application.Features.Community.Feed.ViewModels;
using CommunityCar.Application.Common.Models;

namespace CommunityCar.Application.Services.Community.Feed;

/// <summary>
/// Service responsible for feed utility operations (sorting, pagination, stats)
/// </summary>
public class FeedUtilityService : IFeedUtilityService
{
    private readonly IUnitOfWork _unitOfWork;

    public FeedUtilityService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<List<string>> GetUserInterestsAsync(Guid? userId)
    {
        if (!userId.HasValue) return Task.FromResult(new List<string>());
        
        // TODO: Get user's interests from profile, past interactions, etc.
        return Task.FromResult(new List<string> { "BMW", "Mercedes", "Racing", "Electric Cars" });
    }

    public Task<List<Guid>> GetUserFriendIdsAsync(Guid? userId)
    {
        if (!userId.HasValue) return Task.FromResult(new List<Guid>());
        
        // TODO: Get user's friend IDs from friendship table
        return Task.FromResult(new List<Guid>());
    }

    public async Task<IEnumerable<StoryFeedVM>> GetActiveStoriesAsync(Guid? userId = null)
    {
        var activeStories = await _unitOfWork.Stories.GetActiveAsync();
        var storyVMs = new List<StoryFeedVM>();

        foreach (var story in activeStories.Take(20)) // Limit to 20 most recent
        {
            var storyVM = new StoryFeedVM
            {
                Id = story.Id,
                MediaUrl = story.MediaUrl,
                ThumbnailUrl = story.ThumbnailUrl,
                Type = story.Type,
                Caption = story.Caption,
                CaptionAr = story.CaptionAr,
                AuthorId = story.AuthorId,
                AuthorName = "User", // TODO: Get from user service
                CreatedAt = story.CreatedAt,
                ExpiresAt = story.ExpiresAt,
                TimeRemaining = CalculateTimeRemaining(story.ExpiresAt),
                IsExpired = story.IsExpired,
                ViewCount = story.ViewCount,
                LikeCount = story.LikeCount,
                CarMake = story.CarMake,
                CarModel = story.CarModel,
                CarYear = story.CarYear,
                CarDisplayName = story.CarDisplayName,
                Location = story.LocationName,
                Tags = story.Tags.ToList(),
                AdditionalMediaUrls = story.AdditionalMediaUrls.ToList(),
                IsMultiMedia = story.IsMultiMedia,
                TotalMediaCount = story.TotalMediaCount,
                IsViewed = false, // TODO: Check if user has viewed
                IsLikedByUser = false // TODO: Check if user has liked
            };

            storyVMs.Add(storyVM);
        }

        return storyVMs.OrderByDescending(s => s.CreatedAt);
    }

    public async Task<IEnumerable<TrendingTopicVM>> GetTrendingTopicsAsync(int count = 10)
    {
        // This is a simplified implementation - in production you'd have more sophisticated trending algorithms
        var topics = new List<TrendingTopicVM>();

        // Get trending from news tags
        var newsTags = await _unitOfWork.News.GetPopularTagsAsync(count);
        foreach (var tag in newsTags.Take(count / 2))
        {
            topics.Add(new TrendingTopicVM
            {
                Topic = tag,
                Category = "News",
                PostCount = 10, // TODO: Calculate actual count
                EngagementCount = 50, // TODO: Calculate actual engagement
                TrendingScore = 85.5,
                TrendingReason = "Breaking News",
                LastActivityAt = DateTime.UtcNow.AddMinutes(-30),
                TimeAgo = "30 minutes ago"
            });
        }

        // Get trending car makes
        var carMakes = await _unitOfWork.News.GetAvailableCarMakesAsync();
        foreach (var make in carMakes.Take(count / 2))
        {
            topics.Add(new TrendingTopicVM
            {
                Topic = make,
                Category = "Cars",
                PostCount = 25,
                EngagementCount = 120,
                TrendingScore = 78.2,
                TrendingReason = "Community Interest",
                LastActivityAt = DateTime.UtcNow.AddHours(-2),
                TimeAgo = "2 hours ago"
            });
        }

        return topics.OrderByDescending(t => t.TrendingScore).Take(count);
    }

    public Task<IEnumerable<SuggestedFriendVM>> GetSuggestedFriendsAsync(Guid userId, int count = 5)
    {
        // This is a simplified implementation - in production you'd have more sophisticated friend suggestion algorithms
        var suggestions = new List<SuggestedFriendVM>();

        // TODO: Implement actual friend suggestion logic based on:
        // - Mutual friends
        // - Similar interests (car makes, tags)
        // - Location proximity
        // - Activity patterns
        // - Mutual interests

        // For now, return mock data
        foreach (var i in Enumerable.Range(0, count))
        {
            suggestions.Add(new SuggestedFriendVM
            {
                UserId = Guid.NewGuid(),
                Name = $"Car Enthusiast {i + 1}",
                Bio = "Passionate about cars and automotive technology",
                MutualFriendsCount = 3,
                SuggestionReason = "Similar interests",
                PostCount = 15,
                FollowerCount = 120,
                LastActiveAt = DateTime.UtcNow.AddHours(-i),
                LastActiveAgo = $"{i} hours ago",
                FavoriteCarMakes = new List<string> { "BMW", "Mercedes" },
                CommonInterests = new List<string> { "Racing", "Modifications" }
            });
        }

        return Task.FromResult<IEnumerable<SuggestedFriendVM>>(suggestions);
    }

    public async Task<FeedStatsVM> GetFeedStatsAsync(Guid? userId = null)
    {
        var allNews = await _unitOfWork.News.GetAllAsync();
        var allReviews = await _unitOfWork.Reviews.GetAllAsync();
        var allStories = await _unitOfWork.Stories.GetAllAsync();

        return new FeedStatsVM
        {
            TotalItems = allNews.Count() + allReviews.Count() + allStories.Count(),
            UnseenItems = 5, // TODO: Calculate based on user's last seen
            TrendingItems = 10,
            FriendsItems = 8,
            StoriesCount = allStories.Count(),
            ActiveStoriesCount = allStories.Count(s => s.IsActive),
            LastRefreshAt = DateTime.UtcNow,
            LastRefreshAgo = "Just now",
            NewsCount = allNews.Count(),
            ReviewsCount = allReviews.Count(),
            QACount = 0, // TODO: Add QA count
            PostsCount = 0, // TODO: Add posts count
            TotalLikes = allNews.Sum(n => n.LikeCount) + allReviews.Sum(r => r.HelpfulCount),
            TotalComments = allNews.Sum(n => n.CommentCount) + allReviews.Sum(r => r.ReplyCount),
            TotalShares = allNews.Sum(n => n.ShareCount),
            TotalViews = allNews.Sum(n => n.ViewCount) + allReviews.Sum(r => r.ViewCount)
        };
    }

    public List<FeedItemVM> ApplySorting(List<FeedItemVM> items, FeedSortBy sortBy)
    {
        return sortBy switch
        {
            FeedSortBy.Newest => items.OrderByDescending(x => x.CreatedAt).ToList(),
            FeedSortBy.Popular => items.OrderByDescending(x => x.LikeCount + x.CommentCount).ToList(),
            FeedSortBy.Trending => items.OrderByDescending(x => x.IsTrending ? 1 : 0).ThenByDescending(x => x.RelevanceScore).ToList(),
            FeedSortBy.Engagement => items.OrderByDescending(x => x.LikeCount + x.CommentCount + x.ShareCount).ToList(),
            _ => items.OrderByDescending(x => x.RelevanceScore).ToList()
        };
    }

    public List<FeedItemVM> ApplyPagination(List<FeedItemVM> items, int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;
        return items.Skip(skip).Take(pageSize).ToList();
    }

    public PaginationInfo CreatePaginationInfo(int currentPage, int pageSize, int totalItems)
    {
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        var skip = (currentPage - 1) * pageSize;

        return new PaginationInfo
        {
            CurrentPage = currentPage,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            HasPreviousPage = currentPage > 1,
            HasNextPage = currentPage < totalPages,
            StartItem = skip + 1,
            EndItem = Math.Min(skip + pageSize, totalItems)
        };
    }

    public string CalculateTimeRemaining(DateTime expiresAt)
    {
        var timeSpan = expiresAt - DateTime.UtcNow;
        
        if (timeSpan.TotalMinutes < 1) return "Expiring soon";
        if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes}m left";
        if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours}h left";
        
        return $"{(int)timeSpan.TotalDays}d left";
    }

    public async Task<int> CleanupExpiredStoriesAsync()
    {
        var expiredStories = await _unitOfWork.Stories.GetExpiredAsync();
        int count = 0;
        foreach (var story in expiredStories)
        {
            story.Archive();
            count++;
        }

        if (count > 0)
        {
            await _unitOfWork.SaveChangesAsync();
        }

        return count;
    }
}
