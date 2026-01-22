using AutoMapper;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Features.Feed.DTOs;
using CommunityCar.Application.Features.Feed.ViewModels;
using CommunityCar.Application.Common.Models;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Services.Community;

public class FeedService : IFeedService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public FeedService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<FeedResponse> GetPersonalizedFeedAsync(FeedRequest request)
    {
        var feedItems = new List<FeedItemVM>();
        
        // Get user's interests and preferences
        var userInterests = await GetUserInterestsAsync(request.UserId);
        var friendIds = await GetUserFriendIdsAsync(request.UserId);

        // Collect content from different sources
        if (request.ContentTypes.Contains("News") || !request.ContentTypes.Any())
        {
            var news = await GetPersonalizedNewsAsync(request, userInterests, friendIds);
            feedItems.AddRange(news);
        }

        if (request.ContentTypes.Contains("Reviews") || !request.ContentTypes.Any())
        {
            var reviews = await GetPersonalizedReviewsAsync(request, userInterests, friendIds);
            feedItems.AddRange(reviews);
        }

        if (request.ContentTypes.Contains("QA") || !request.ContentTypes.Any())
        {
            var qaItems = await GetPersonalizedQAAsync(request, userInterests, friendIds);
            feedItems.AddRange(qaItems);
        }

        if (request.ContentTypes.Contains("Stories") || !request.ContentTypes.Any())
        {
            var stories = await GetPersonalizedStoriesAsync(request, userInterests, friendIds);
            feedItems.AddRange(stories);
        }

        // Apply sorting and pagination
        feedItems = ApplySorting(feedItems, request.SortBy);
        var totalCount = feedItems.Count;
        var pagedItems = ApplyPagination(feedItems, request.Page, request.PageSize);

        // Get additional feed data
        var activeStories = await GetActiveStoriesAsync(request.UserId);
        var trendingTopics = await GetTrendingTopicsAsync(10);
        var suggestedFriends = request.UserId.HasValue 
            ? await GetSuggestedFriendsAsync(request.UserId.Value, 5) 
            : new List<SuggestedFriendVM>();

        return new FeedResponse
        {
            FeedItems = pagedItems,
            Stories = activeStories,
            TrendingTopics = trendingTopics,
            SuggestedFriends = suggestedFriends,
            Pagination = CreatePaginationInfo(request.Page, request.PageSize, totalCount),
            Stats = await GetFeedStatsAsync(request.UserId),
            HasMoreContent = (request.Page * request.PageSize) < totalCount
        };
    }

    public async Task<FeedResponse> GetTrendingFeedAsync(FeedRequest request)
    {
        var feedItems = new List<FeedItemVM>();

        // Get trending content from all sources
        var trendingNews = await GetTrendingNewsAsync(request);
        var trendingReviews = await GetTrendingReviewsAsync(request);
        var trendingQA = await GetTrendingQAAsync(request);
        var trendingStories = await GetTrendingStoriesAsync(request);

        feedItems.AddRange(trendingNews);
        feedItems.AddRange(trendingReviews);
        feedItems.AddRange(trendingQA);
        feedItems.AddRange(trendingStories);

        // Sort by trending score
        feedItems = feedItems.OrderByDescending(x => x.RelevanceScore).ToList();
        var totalCount = feedItems.Count;
        var pagedItems = ApplyPagination(feedItems, request.Page, request.PageSize);

        return new FeedResponse
        {
            FeedItems = pagedItems,
            Stories = await GetActiveStoriesAsync(request.UserId),
            TrendingTopics = await GetTrendingTopicsAsync(15),
            Pagination = CreatePaginationInfo(request.Page, request.PageSize, totalCount),
            Stats = await GetFeedStatsAsync(request.UserId),
            HasMoreContent = (request.Page * request.PageSize) < totalCount
        };
    }

    public async Task<FeedResponse> GetFriendsFeedAsync(FeedRequest request)
    {
        if (!request.UserId.HasValue)
            return new FeedResponse();

        var friendIds = await GetUserFriendIdsAsync(request.UserId);
        var feedItems = new List<FeedItemVM>();

        // Get content from friends only
        var friendsNews = await GetFriendsNewsAsync(request, friendIds);
        var friendsReviews = await GetFriendsReviewsAsync(request, friendIds);
        var friendsQA = await GetFriendsQAAsync(request, friendIds);
        var friendsStories = await GetFriendsStoriesAsync(request, friendIds);

        feedItems.AddRange(friendsNews);
        feedItems.AddRange(friendsReviews);
        feedItems.AddRange(friendsQA);
        feedItems.AddRange(friendsStories);

        // Sort by recency for friends feed
        feedItems = feedItems.OrderByDescending(x => x.CreatedAt).ToList();
        var totalCount = feedItems.Count;
        var pagedItems = ApplyPagination(feedItems, request.Page, request.PageSize);

        return new FeedResponse
        {
            FeedItems = pagedItems,
            Stories = await GetActiveStoriesAsync(request.UserId),
            TrendingTopics = await GetTrendingTopicsAsync(5),
            SuggestedFriends = await GetSuggestedFriendsAsync(request.UserId.Value, 3),
            Pagination = CreatePaginationInfo(request.Page, request.PageSize, totalCount),
            Stats = await GetFeedStatsAsync(request.UserId),
            HasMoreContent = (request.Page * request.PageSize) < totalCount
        };
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

        // For now, return mock data
        for (int i = 0; i < count; i++)
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

    public Task<bool> MarkAsSeenAsync(Guid userId, Guid contentId, string contentType)
    {
        // TODO: Implement marking content as seen
        // This would typically involve creating a UserContentView record
        return Task.FromResult(true);
    }

    public Task<bool> InteractWithContentAsync(Guid userId, Guid contentId, string contentType, string interactionType)
    {
        // TODO: Implement content interaction (like, share, bookmark)
        // This would involve updating the appropriate content and creating interaction records
        return Task.FromResult(true);
    }

    public async Task<bool> AddCommentAsync(Guid userId, Guid contentId, string contentType, string comment)
    {
        try
        {
            // TODO: Implement adding comments to content
            // This would involve creating a comment record and updating comment counts
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<object>> GetCommentsAsync(Guid contentId, string contentType)
    {
        try
        {
            // TODO: Implement getting comments for content
            // For now, return mock data
            var comments = new List<object>
            {
                new
                {
                    id = Guid.NewGuid(),
                    authorName = "John Doe",
                    authorAvatar = "/images/default-avatar.png",
                    content = "Great post! Thanks for sharing.",
                    timeAgo = "2 hours ago",
                    createdAt = DateTime.UtcNow.AddHours(-2)
                },
                new
                {
                    id = Guid.NewGuid(),
                    authorName = "Jane Smith",
                    authorAvatar = "/images/default-avatar.png",
                    content = "I completely agree with this review.",
                    timeAgo = "1 hour ago",
                    createdAt = DateTime.UtcNow.AddHours(-1)
                }
            };
            
            return comments;
        }
        catch
        {
            return new List<object>();
        }
    }

    public async Task<bool> BookmarkContentAsync(Guid userId, Guid contentId, string contentType)
    {
        try
        {
            // TODO: Implement bookmarking content
            // This would involve creating/removing bookmark records
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> HideContentAsync(Guid userId, Guid contentId, string contentType)
    {
        try
        {
            // TODO: Implement hiding content for user
            // This would involve creating a hidden content record
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ReportContentAsync(Guid userId, Guid contentId, string contentType, string reason)
    {
        try
        {
            // TODO: Implement reporting content
            // This would involve creating a report record and potentially flagging content
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Private helper methods
    private Task<List<string>> GetUserInterestsAsync(Guid? userId)
    {
        if (!userId.HasValue) return Task.FromResult(new List<string>());
        
        // TODO: Get user's interests from profile, past interactions, etc.
        return Task.FromResult(new List<string> { "BMW", "Mercedes", "Racing", "Electric Cars" });
    }

    private Task<List<Guid>> GetUserFriendIdsAsync(Guid? userId)
    {
        if (!userId.HasValue) return Task.FromResult(new List<Guid>());
        
        // TODO: Get user's friend IDs from friendship table
        return Task.FromResult(new List<Guid>());
    }

    private async Task<List<FeedItemVM>> GetPersonalizedNewsAsync(FeedRequest request, List<string> userInterests, List<Guid> friendIds)
    {
        var news = await _unitOfWork.News.GetPublishedAsync();
        var feedItems = new List<FeedItemVM>();

        foreach (var item in news.Take(10))
        {
            var relevanceScore = CalculateRelevanceScore(item.Tags.ToList(), item.CarMake, userInterests);
            
            feedItems.Add(new FeedItemVM
            {
                Id = item.Id,
                ContentType = "News",
                Title = item.Headline,
                Content = item.Body,
                Summary = item.Summary,
                ImageUrl = item.ImageUrl,
                AuthorId = item.AuthorId,
                AuthorName = "News Author", // TODO: Get from user service
                ViewCount = item.ViewCount,
                LikeCount = item.LikeCount,
                CommentCount = item.CommentCount,
                ShareCount = item.ShareCount,
                Tags = item.Tags.ToList(),
                Category = item.Category.ToString(),
                CarMake = item.CarMake,
                CarModel = item.CarModel,
                CarYear = item.CarYear,
                CarDisplayName = item.CarDisplayName,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt ?? item.CreatedAt,
                TimeAgo = CalculateTimeAgo(item.CreatedAt),
                RelevanceScore = relevanceScore,
                ReasonForShowing = GetReasonForShowing(relevanceScore, userInterests, item.Tags.ToList()),
                IsTrending = item.IsFeatured,
                IsFeatured = item.IsFeatured
            });
        }

        return feedItems;
    }

    private async Task<List<FeedItemVM>> GetPersonalizedReviewsAsync(FeedRequest request, List<string> userInterests, List<Guid> friendIds)
    {
        var reviews = await _unitOfWork.Reviews.GetApprovedAsync();
        var feedItems = new List<FeedItemVM>();

        foreach (var item in reviews.Take(10))
        {
            var relevanceScore = CalculateRelevanceScore(new List<string>(), item.CarMake, userInterests);
            
            feedItems.Add(new FeedItemVM
            {
                Id = item.Id,
                ContentType = "Review",
                Title = item.Title,
                Content = item.Comment,
                AuthorId = item.ReviewerId,
                AuthorName = "Reviewer", // TODO: Get from user service
                ViewCount = item.ViewCount,
                LikeCount = item.HelpfulCount,
                CommentCount = item.ReplyCount,
                Rating = item.Rating,
                CarMake = item.CarMake,
                CarModel = item.CarModel,
                CarYear = item.CarYear,
                CarDisplayName = item.CarDisplayName,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt ?? item.CreatedAt,
                TimeAgo = CalculateTimeAgo(item.CreatedAt),
                RelevanceScore = relevanceScore,
                ReasonForShowing = GetReasonForShowing(relevanceScore, userInterests, new List<string>()),
                IsTrending = item.HelpfulCount > 10
            });
        }

        return feedItems;
    }

    private async Task<List<FeedItemVM>> GetPersonalizedQAAsync(FeedRequest request, List<string> userInterests, List<Guid> friendIds)
    {
        var questions = await _unitOfWork.QA.GetAllAsync();
        var feedItems = new List<FeedItemVM>();

        foreach (var item in questions.Take(10))
        {
            var relevanceScore = CalculateRelevanceScore(item.Tags.ToList(), item.CarMake, userInterests);
            
            feedItems.Add(new FeedItemVM
            {
                Id = item.Id,
                ContentType = "QA",
                Title = item.Title,
                Content = item.Body,
                AuthorId = item.AuthorId,
                AuthorName = "Question Author", // TODO: Get from user service
                ViewCount = item.ViewCount,
                LikeCount = item.VoteScore,
                CommentCount = item.AnswerCount,
                Tags = item.Tags.ToList(),
                CarMake = item.CarMake,
                CarModel = item.CarModel,
                CarYear = item.CarYear,
                CarDisplayName = item.CarDisplayName,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt ?? item.CreatedAt,
                TimeAgo = CalculateTimeAgo(item.CreatedAt),
                RelevanceScore = relevanceScore,
                ReasonForShowing = GetReasonForShowing(relevanceScore, userInterests, item.Tags.ToList()),
                IsAnswered = item.IsSolved,
                IsTrending = item.VoteScore > 5
            });
        }

        return feedItems;
    }

    private async Task<List<FeedItemVM>> GetPersonalizedStoriesAsync(FeedRequest request, List<string> userInterests, List<Guid> friendIds)
    {
        var stories = await _unitOfWork.Stories.GetActiveAsync();
        var feedItems = new List<FeedItemVM>();

        foreach (var item in stories.Take(5))
        {
            var relevanceScore = CalculateRelevanceScore(item.Tags.ToList(), item.CarMake, userInterests);
            
            feedItems.Add(new FeedItemVM
            {
                Id = item.Id,
                ContentType = "Story",
                Title = item.Caption ?? "Story",
                Content = item.Caption ?? "",
                ImageUrl = item.MediaUrl,
                AuthorId = item.AuthorId,
                AuthorName = "Story Author", // TODO: Get from user service
                ViewCount = item.ViewCount,
                LikeCount = item.LikeCount,
                Tags = item.Tags.ToList(),
                CarMake = item.CarMake,
                CarModel = item.CarModel,
                CarYear = item.CarYear,
                CarDisplayName = item.CarDisplayName,
                Location = item.LocationName,
                CreatedAt = item.CreatedAt,
                TimeAgo = CalculateTimeAgo(item.CreatedAt),
                RelevanceScore = relevanceScore,
                ReasonForShowing = GetReasonForShowing(relevanceScore, userInterests, item.Tags.ToList()),
                IsExpired = item.IsExpired,
                IsTrending = item.IsFeatured
            });
        }

        return feedItems;
    }

    // Simplified implementations for trending and friends feeds
    private async Task<List<FeedItemVM>> GetTrendingNewsAsync(FeedRequest request) => 
        await GetPersonalizedNewsAsync(request, new List<string>(), new List<Guid>());
    
    private async Task<List<FeedItemVM>> GetTrendingReviewsAsync(FeedRequest request) => 
        await GetPersonalizedReviewsAsync(request, new List<string>(), new List<Guid>());
    
    private async Task<List<FeedItemVM>> GetTrendingQAAsync(FeedRequest request) => 
        await GetPersonalizedQAAsync(request, new List<string>(), new List<Guid>());
    
    private async Task<List<FeedItemVM>> GetTrendingStoriesAsync(FeedRequest request) => 
        await GetPersonalizedStoriesAsync(request, new List<string>(), new List<Guid>());

    private async Task<List<FeedItemVM>> GetFriendsNewsAsync(FeedRequest request, List<Guid> friendIds) => 
        await GetPersonalizedNewsAsync(request, new List<string>(), friendIds);
    
    private async Task<List<FeedItemVM>> GetFriendsReviewsAsync(FeedRequest request, List<Guid> friendIds) => 
        await GetPersonalizedReviewsAsync(request, new List<string>(), friendIds);
    
    private async Task<List<FeedItemVM>> GetFriendsQAAsync(FeedRequest request, List<Guid> friendIds) => 
        await GetPersonalizedQAAsync(request, new List<string>(), friendIds);
    
    private async Task<List<FeedItemVM>> GetFriendsStoriesAsync(FeedRequest request, List<Guid> friendIds) => 
        await GetPersonalizedStoriesAsync(request, new List<string>(), friendIds);

    // Helper methods
    private double CalculateRelevanceScore(List<string> contentTags, string? carMake, List<string> userInterests)
    {
        var score = 50.0; // Base score

        // Boost for matching interests
        foreach (var interest in userInterests)
        {
            if (contentTags.Any(t => t.Contains(interest, StringComparison.OrdinalIgnoreCase)))
                score += 20;
            
            if (carMake?.Contains(interest, StringComparison.OrdinalIgnoreCase) == true)
                score += 25;
        }

        return Math.Min(score, 100.0);
    }

    private string GetReasonForShowing(double relevanceScore, List<string> userInterests, List<string> contentTags)
    {
        if (relevanceScore > 80) return "Matches your interests";
        if (relevanceScore > 60) return "Similar to content you liked";
        if (contentTags.Any(t => userInterests.Contains(t))) return "Based on your preferences";
        return "Trending in community";
    }

    private List<FeedItemVM> ApplySorting(List<FeedItemVM> items, FeedSortBy sortBy)
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

    private List<FeedItemVM> ApplyPagination(List<FeedItemVM> items, int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;
        return items.Skip(skip).Take(pageSize).ToList();
    }

    private PaginationInfo CreatePaginationInfo(int currentPage, int pageSize, int totalItems)
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

    private string CalculateTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;
        
        if (timeSpan.TotalMinutes < 1) return "Just now";
        if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes}m ago";
        if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours}h ago";
        if (timeSpan.TotalDays < 7) return $"{(int)timeSpan.TotalDays}d ago";
        
        return dateTime.ToString("MMM dd");
    }

    private string CalculateTimeRemaining(DateTime expiresAt)
    {
        var timeSpan = expiresAt - DateTime.UtcNow;
        
        if (timeSpan.TotalMinutes < 1) return "Expiring soon";
        if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes}m left";
        if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours}h left";
        
        return $"{(int)timeSpan.TotalDays}d left";
    }
}