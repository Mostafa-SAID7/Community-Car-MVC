using AutoMapper;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Features.Feed.DTOs;
using CommunityCar.Application.Features.Feed.ViewModels;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Services.Community.Feed;

/// <summary>
/// Service responsible for aggregating content from different sources for feeds
/// </summary>
public class FeedContentAggregatorService : IFeedContentAggregatorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public FeedContentAggregatorService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<FeedItemVM>> GetPersonalizedContentAsync(FeedRequest request, List<string> userInterests, List<Guid> friendIds)
    {
        var feedItems = new List<FeedItemVM>();

        // Collect content from different sources based on request
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

        if (request.ContentTypes.Contains("Question") || !request.ContentTypes.Any())
        {
            var qaItems = await GetPersonalizedQAAsync(request, userInterests, friendIds);
            feedItems.AddRange(qaItems);
        }

        if (request.ContentTypes.Contains("Stories") || !request.ContentTypes.Any())
        {
            var stories = await GetPersonalizedStoriesAsync(request, userInterests, friendIds);
            feedItems.AddRange(stories);
        }

        return feedItems;
    }

    public async Task<List<FeedItemVM>> GetTrendingContentAsync(FeedRequest request)
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

        return feedItems.OrderByDescending(x => x.RelevanceScore).ToList();
    }

    public async Task<List<FeedItemVM>> GetFriendsContentAsync(FeedRequest request, List<Guid> friendIds)
    {
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

        return feedItems.OrderByDescending(x => x.CreatedAt).ToList();
    }

    public async Task<List<FeedItemVM>> GetContentFromSourceAsync(FeedRequest request, string contentType, List<string> userInterests, List<Guid> friendIds)
    {
        return contentType.ToLower() switch
        {
            "news" => await GetPersonalizedNewsAsync(request, userInterests, friendIds),
            "reviews" => await GetPersonalizedReviewsAsync(request, userInterests, friendIds),
            "question" => await GetPersonalizedQAAsync(request, userInterests, friendIds),
            "stories" => await GetPersonalizedStoriesAsync(request, userInterests, friendIds),
            _ => new List<FeedItemVM>()
        };
    }

    #region Private Helper Methods

    private async Task<List<FeedItemVM>> GetPersonalizedNewsAsync(FeedRequest request, List<string> userInterests, List<Guid> friendIds)
    {
        var news = await _unitOfWork.News.GetPublishedAsync();
        var feedItems = new List<FeedItemVM>();

        foreach (var item in news.Take(10))
        {
            var relevanceScore = CalculateRelevanceScore(item.Tags.ToList(), item.CarMake, userInterests);
            
            // Get user interaction status
            var isLikedByUser = false;
            var isBookmarkedByUser = false;
            
            if (request.UserId.HasValue)
            {
                var userReaction = await _unitOfWork.Reactions.GetUserReactionAsync(item.Id, EntityType.News, request.UserId.Value);
                isLikedByUser = userReaction != null && userReaction.Type == ReactionType.Like;
                
                var userBookmark = await _unitOfWork.Bookmarks.GetUserBookmarkAsync(item.Id, EntityType.News, request.UserId.Value);
                isBookmarkedByUser = userBookmark != null;
            }
            
            feedItems.Add(new FeedItemVM
            {
                Id = item.Id,
                ContentType = "News",
                Title = item.Headline,
                TitleAr = item.HeadlineAr,
                Content = item.Body,
                ContentAr = item.BodyAr,
                Summary = item.Summary,
                SummaryAr = item.SummaryAr,
                ImageUrl = item.ImageUrl,
                AuthorId = item.AuthorId,
                AuthorName = "News Author", // TODO: Get from user service
                ViewCount = item.ViewCount,
                LikeCount = item.LikeCount,
                CommentCount = item.CommentCount,
                ShareCount = item.ShareCount,
                IsLikedByUser = isLikedByUser,
                IsBookmarkedByUser = isBookmarkedByUser,
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
            
            // Get user interaction status
            var isLikedByUser = false;
            var isBookmarkedByUser = false;
            
            if (request.UserId.HasValue)
            {
                var userReaction = await _unitOfWork.Reactions.GetUserReactionAsync(item.Id, EntityType.Review, request.UserId.Value);
                isLikedByUser = userReaction != null && userReaction.Type == ReactionType.Like;
                
                var userBookmark = await _unitOfWork.Bookmarks.GetUserBookmarkAsync(item.Id, EntityType.Review, request.UserId.Value);
                isBookmarkedByUser = userBookmark != null;
            }
            
            feedItems.Add(new FeedItemVM
            {
                Id = item.Id,
                ContentType = "Review",
                Title = item.Title,
                TitleAr = item.TitleAr,
                Content = item.Comment,
                ContentAr = item.CommentAr,
                AuthorId = item.ReviewerId,
                AuthorName = "Reviewer", // TODO: Get from user service
                ViewCount = item.ViewCount,
                LikeCount = item.HelpfulCount,
                CommentCount = item.ReplyCount,
                IsLikedByUser = isLikedByUser,
                IsBookmarkedByUser = isBookmarkedByUser,
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
            
            // Get user interaction status
            var isLikedByUser = false;
            var isBookmarkedByUser = false;
            
            if (request.UserId.HasValue)
            {
                var userReaction = await _unitOfWork.Reactions.GetUserReactionAsync(item.Id, EntityType.Question, request.UserId.Value);
                isLikedByUser = userReaction != null && userReaction.Type == ReactionType.Like;
                
                var userBookmark = await _unitOfWork.Bookmarks.GetUserBookmarkAsync(item.Id, EntityType.Question, request.UserId.Value);
                isBookmarkedByUser = userBookmark != null;
            }
            
            feedItems.Add(new FeedItemVM
            {
                Id = item.Id,
                ContentType = "Question",
                Title = item.Title,
                TitleAr = item.TitleAr,
                Content = item.Body,
                ContentAr = item.BodyAr,
                AuthorId = item.AuthorId,
                AuthorName = "Question Author", // TODO: Get from user service
                ViewCount = item.ViewCount,
                LikeCount = item.VoteScore,
                CommentCount = item.AnswerCount,
                IsLikedByUser = isLikedByUser,
                IsBookmarkedByUser = isBookmarkedByUser,
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
            
            // Get user interaction status
            var isLikedByUser = false;
            var isBookmarkedByUser = false;
            
            if (request.UserId.HasValue)
            {
                var userReaction = await _unitOfWork.Reactions.GetUserReactionAsync(item.Id, EntityType.Story, request.UserId.Value);
                isLikedByUser = userReaction != null && userReaction.Type == ReactionType.Like;
                
                var userBookmark = await _unitOfWork.Bookmarks.GetUserBookmarkAsync(item.Id, EntityType.Story, request.UserId.Value);
                isBookmarkedByUser = userBookmark != null;
            }
            
            feedItems.Add(new FeedItemVM
            {
                Id = item.Id,
                ContentType = "Story",
                Title = item.Caption ?? "Story",
                TitleAr = item.CaptionAr,
                Content = item.Caption ?? "",
                ContentAr = item.CaptionAr,
                ImageUrl = item.MediaUrl,
                AuthorId = item.AuthorId,
                AuthorName = "Story Author", // TODO: Get from user service
                ViewCount = item.ViewCount,
                LikeCount = item.LikeCount,
                IsLikedByUser = isLikedByUser,
                IsBookmarkedByUser = isBookmarkedByUser,
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

    private string CalculateTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;
        
        if (timeSpan.TotalMinutes < 1) return "Just now";
        if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes}m ago";
        if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours}h ago";
        if (timeSpan.TotalDays < 7) return $"{(int)timeSpan.TotalDays}d ago";
        
        return dateTime.ToString("MMM dd");
    }

    #endregion
}


