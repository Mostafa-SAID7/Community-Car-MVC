using AutoMapper;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Features.Community.Stories.ViewModels;
using CommunityCar.Application.Features.Shared.ViewModels;
using CommunityCar.Application.Common.Models;
using CommunityCar.Domain.Entities.Community.Stories;
using CommunityCar.Domain.Enums.Community;
using StoriesSearchVM = CommunityCar.Application.Features.Community.Stories.ViewModels.StoriesSearchVM;

namespace CommunityCar.Application.Services.Community.Stories;

public class StoriesService : IStoriesService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public StoriesService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<StoriesSearchVM> SearchStoriesAsync(StoriesSearchVM request)
    {
        var stories = await _unitOfWork.Stories.GetAllAsync();
        var queryable = stories.AsQueryable();

        // Apply search term filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLowerInvariant();
            queryable = queryable.Where(s => 
                (s.Caption != null && s.Caption.ToLowerInvariant().Contains(searchTerm)) ||
                (s.LocationName != null && s.LocationName.ToLowerInvariant().Contains(searchTerm)) ||
                (s.CarMake != null && s.CarMake.ToLowerInvariant().Contains(searchTerm)) ||
                (s.CarModel != null && s.CarModel.ToLowerInvariant().Contains(searchTerm)) ||
                (s.EventType != null && s.EventType.ToLowerInvariant().Contains(searchTerm)) ||
                s.Tags.Any(t => t.ToLowerInvariant().Contains(searchTerm))
            );
        }

        // Apply filters
        if (request.AuthorId.HasValue)
            queryable = queryable.Where(s => s.AuthorId == request.AuthorId.Value);

        if (request.Type.HasValue)
            queryable = queryable.Where(s => s.Type == request.Type.Value);

        if (request.Visibility.HasValue)
            queryable = queryable.Where(s => s.Visibility == request.Visibility.Value);

        if (!string.IsNullOrWhiteSpace(request.CarMake))
            queryable = queryable.Where(s => s.CarMake != null && s.CarMake.ToLowerInvariant() == request.CarMake.ToLowerInvariant());

        if (!string.IsNullOrWhiteSpace(request.CarModel))
            queryable = queryable.Where(s => s.CarModel != null && s.CarModel.ToLowerInvariant() == request.CarModel.ToLowerInvariant());

        if (request.CarYear.HasValue)
            queryable = queryable.Where(s => s.CarYear == request.CarYear.Value);

        if (request.Tags.Any())
        {
            var lowerTags = request.Tags.Select(t => t.ToLowerInvariant()).ToList();
            queryable = queryable.Where(s => s.Tags.Any(t => lowerTags.Contains(t.ToLowerInvariant())));
        }

        if (!string.IsNullOrWhiteSpace(request.EventType))
            queryable = queryable.Where(s => s.EventType != null && s.EventType.ToLowerInvariant() == request.EventType.ToLowerInvariant());

        if (request.IsActive.HasValue)
            queryable = queryable.Where(s => s.IsActive == request.IsActive.Value);

        if (request.IsArchived.HasValue)
            queryable = queryable.Where(s => s.IsArchived == request.IsArchived.Value);

        if (request.IsFeatured.HasValue)
            queryable = queryable.Where(s => s.IsFeatured == request.IsFeatured.Value);

        if (request.IsHighlighted.HasValue)
            queryable = queryable.Where(s => s.IsHighlighted == request.IsHighlighted.Value);

        // Apply date filters
        if (request.CreatedAfter.HasValue)
            queryable = queryable.Where(s => s.CreatedAt >= request.CreatedAfter.Value);

        if (request.CreatedBefore.HasValue)
            queryable = queryable.Where(s => s.CreatedAt <= request.CreatedBefore.Value);

        if (request.ExpiresAfter.HasValue)
            queryable = queryable.Where(s => s.ExpiresAt >= request.ExpiresAfter.Value);

        if (request.ExpiresBefore.HasValue)
            queryable = queryable.Where(s => s.ExpiresAt <= request.ExpiresBefore.Value);

        // Apply location filter
        if (request.Latitude.HasValue && request.Longitude.HasValue && request.RadiusKm.HasValue)
        {
            queryable = queryable.Where(s => s.Latitude.HasValue && s.Longitude.HasValue);
            // Note: In a real implementation, you'd use a proper geospatial query
        }

        // Apply engagement filters
        if (request.MinViews.HasValue)
            queryable = queryable.Where(s => s.ViewCount >= request.MinViews.Value);

        if (request.MaxViews.HasValue)
            queryable = queryable.Where(s => s.ViewCount <= request.MaxViews.Value);

        if (request.MinLikes.HasValue)
            queryable = queryable.Where(s => s.LikeCount >= request.MinLikes.Value);

        if (request.MaxLikes.HasValue)
            queryable = queryable.Where(s => s.LikeCount <= request.MaxLikes.Value);

        // Get total count before pagination
        var totalCount = queryable.Count();

        // Apply sorting
        queryable = request.SortBy.ToLower() switch
        {
            "newest" => queryable.OrderByDescending(s => s.CreatedAt),
            "oldest" => queryable.OrderBy(s => s.CreatedAt),
            "mostviews" => queryable.OrderByDescending(s => s.ViewCount),
            "mostlikes" => queryable.OrderByDescending(s => s.LikeCount),
            "mostcomments" => queryable.OrderByDescending(s => s.ReplyCount), // Fixed: CommentCount -> ReplyCount
            "trending" => queryable.OrderByDescending(s => s.ViewCount + s.LikeCount),
            "popular" => queryable.OrderByDescending(s => s.LikeCount + s.ReplyCount), // Fixed: CommentCount -> ReplyCount
            "engagement" => queryable.OrderByDescending(s => s.LikeCount + s.ReplyCount + s.ShareCount), // Fixed: CommentCount -> ReplyCount
            "relevance" => !string.IsNullOrWhiteSpace(request.SearchTerm) 
                ? queryable.OrderByDescending(s => s.Caption.Contains(request.SearchTerm) ? 2 : 1) // Fixed: Title -> Caption
                : queryable.OrderByDescending(s => s.CreatedAt),
            _ => queryable.OrderByDescending(s => s.CreatedAt)
        };

        // Apply pagination
        var skip = (request.Page - 1) * request.PageSize;
        var pagedStories = queryable.Skip(skip).Take(request.PageSize).ToList();

        // Map to ViewModels
        var storyVMs = _mapper.Map<List<StoryVM>>(pagedStories);

        // Calculate pagination info
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
        var pagination = new PaginationInfo
        {
            CurrentPage = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalCount,
            TotalPages = totalPages,
            HasPreviousPage = request.Page > 1,
            HasNextPage = request.Page < totalPages,
            StartItem = skip + 1,
            EndItem = Math.Min(skip + request.PageSize, totalCount)
        };

        // Get stats and available filters
        var stats = await GetStoriesStatsAsync();
        var availableTags = await GetPopularTagsAsync(50);
        var availableCarMakes = await GetAvailableCarMakesAsync();

        return new StoriesSearchVM
        {
            Stories = storyVMs,
            Pagination = new PaginationVM
            {
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
            },
            Stats = stats,
            AvailableTags = availableTags.ToList(),
            AvailableCarMakes = availableCarMakes.ToList()
        };
    }

    public async Task<StoryVM?> GetByIdAsync(Guid id)
    {
        var story = await _unitOfWork.Stories.GetByIdAsync(id);
        return story != null ? _mapper.Map<StoryVM>(story) : null;
    }

    public async Task<StoryVM?> GetBySlugAsync(string slug)
    {
        var story = await _unitOfWork.Stories.GetBySlugAsync(slug);
        return story != null ? _mapper.Map<StoryVM>(story) : null;
    }

    public async Task<StoryVM> CreateAsync(CreateStoryVM request)
    {
        var story = new Story(request.MediaUrl, request.AuthorId, request.Type, request.Duration);
        
        story.UpdateCaption(request.Caption);
        if (!string.IsNullOrEmpty(request.CaptionAr))
        {
            story.UpdateArabicContent(request.CaptionAr);
        }
        story.SetThumbnail(request.ThumbnailUrl);
        story.SetLocation(request.Latitude, request.Longitude, request.LocationName);
        story.SetCarInfo(request.CarMake, request.CarModel, request.CarYear, request.EventType);
        story.SetVisibility(request.Visibility);
        story.SetInteractionSettings(request.AllowReplies, request.AllowSharing);
        story.SetFeatured(request.IsFeatured);
        story.SetHighlighted(request.IsHighlighted);

        foreach (var tag in request.Tags)
            story.AddTag(tag);

        foreach (var userIdStr in request.MentionedUsers)
        {
            if (Guid.TryParse(userIdStr, out var userId))
                story.MentionUser(userId);
        }

        foreach (var mediaUrl in request.AdditionalMediaUrls)
            story.AddMedia(mediaUrl);

        await _unitOfWork.Stories.AddAsync(story);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<StoryVM>(story);
    }

    public async Task<StoryVM> UpdateAsync(Guid id, UpdateStoryVM request)
    {
        var story = await _unitOfWork.Stories.GetByIdAsync(id);
        if (story == null)
            throw new ArgumentException("Story not found");

        story.UpdateCaption(request.Caption);
        if (!string.IsNullOrEmpty(request.CaptionAr))
        {
            story.UpdateArabicContent(request.CaptionAr);
        }
        story.SetThumbnail(request.ThumbnailUrl);
        story.SetLocation(request.Latitude, request.Longitude, request.LocationName);
        story.SetCarInfo(request.CarMake, request.CarModel, request.CarYear, request.EventType);
        story.SetVisibility(request.Visibility);
        story.SetInteractionSettings(request.AllowReplies, request.AllowSharing);
        story.SetFeatured(request.IsFeatured);
        story.SetHighlighted(request.IsHighlighted);

        // Update tags, mentions, and media
        foreach (var tag in request.Tags)
            story.AddTag(tag);

        foreach (var userIdStr in request.MentionedUsers)
        {
            if (Guid.TryParse(userIdStr, out var userId))
                story.MentionUser(userId);
        }

        foreach (var mediaUrl in request.AdditionalMediaUrls)
            story.AddMedia(mediaUrl);

        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<StoryVM>(story);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var story = await _unitOfWork.Stories.GetByIdAsync(id);
        if (story == null)
            return false;

        await _unitOfWork.Stories.DeleteAsync(story);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ArchiveAsync(Guid id)
    {
        var story = await _unitOfWork.Stories.GetByIdAsync(id);
        if (story == null)
            return false;

        story.Archive();
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RestoreAsync(Guid id)
    {
        var story = await _unitOfWork.Stories.GetByIdAsync(id);
        if (story == null)
            return false;

        story.Restore();
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetFeaturedAsync(Guid id, bool featured)
    {
        var story = await _unitOfWork.Stories.GetByIdAsync(id);
        if (story == null)
            return false;

        story.SetFeatured(featured);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetHighlightedAsync(Guid id, bool highlighted)
    {
        var story = await _unitOfWork.Stories.GetByIdAsync(id);
        if (story == null)
            return false;

        story.SetHighlighted(highlighted);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExtendDurationAsync(Guid id, int additionalHours)
    {
        var story = await _unitOfWork.Stories.GetByIdAsync(id);
        if (story == null)
            return false;

        story.ExtendDuration(additionalHours);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IncrementViewCountAsync(Guid id)
    {
        var story = await _unitOfWork.Stories.GetByIdAsync(id);
        if (story == null)
            return false;

        story.IncrementViewCount();
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> LikeAsync(Guid id, Guid userId)
    {
        var story = await _unitOfWork.Stories.GetByIdAsync(id);
        if (story == null)
            return false;

        story.IncrementLikeCount();
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnlikeAsync(Guid id, Guid userId)
    {
        var story = await _unitOfWork.Stories.GetByIdAsync(id);
        if (story == null)
            return false;

        story.DecrementLikeCount();
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<string>> GetPopularTagsAsync(int count = 20)
    {
        return await _unitOfWork.Stories.GetPopularTagsAsync(count);
    }

    public async Task<IEnumerable<string>> GetAvailableCarMakesAsync()
    {
        return await _unitOfWork.Stories.GetAvailableCarMakesAsync();
    }

    public async Task<StoriesStatsVM> GetStoriesStatsAsync()
    {
        var allStories = await _unitOfWork.Stories.GetAllAsync();
        var thisMonth = allStories.Where(s => s.CreatedAt >= DateTime.UtcNow.AddMonths(-1));
        var thisWeek = allStories.Where(s => s.CreatedAt >= DateTime.UtcNow.AddDays(-7));
        var today = allStories.Where(s => s.CreatedAt.Date == DateTime.UtcNow.Date);

        return new StoriesStatsVM
        {
            TotalStories = allStories.Count(),
            ActiveStories = allStories.Count(s => s.IsActive),
            ExpiredStories = allStories.Count(s => s.IsExpired),
            ArchivedStories = allStories.Count(s => s.IsArchived),
            FeaturedStories = allStories.Count(s => s.IsFeatured),
            HighlightedStories = allStories.Count(s => s.IsHighlighted),
            TotalViews = allStories.Sum(s => s.ViewCount),
            TotalLikes = allStories.Sum(s => s.LikeCount),
            TotalReplies = allStories.Sum(s => s.ReplyCount),
            TotalShares = allStories.Sum(s => s.ShareCount),
            StoriesThisMonth = thisMonth.Count(),
            StoriesThisWeek = thisWeek.Count(),
            StoriesToday = today.Count(),
            AverageViewsPerStory = allStories.Any() ? allStories.Average(s => s.ViewCount) : 0,
            AverageLikesPerStory = allStories.Any() ? allStories.Average(s => s.LikeCount) : 0,
            AverageLifespanHours = allStories.Any() ? allStories.Average(s => s.Duration) : 0
        };
    }

    public async Task<IEnumerable<StoryVM>> GetActiveStoriesAsync()
    {
        var activeStories = await _unitOfWork.Stories.GetActiveAsync();
        return _mapper.Map<List<StoryVM>>(activeStories);
    }

    public async Task<IEnumerable<StoryVM>> GetStoriesByAuthorAsync(Guid authorId)
    {
        var stories = await _unitOfWork.Stories.GetByAuthorAsync(authorId);
        return _mapper.Map<List<StoryVM>>(stories);
    }

    public async Task CleanupExpiredStoriesAsync()
    {
        await _unitOfWork.Stories.DeleteExpiredAsync();
        await _unitOfWork.SaveChangesAsync();
    }

    private static double CalculateRelevanceScore(Story story, string searchTerm)
    {
        var score = 0.0;
        var lowerSearchTerm = searchTerm.ToLowerInvariant();

        if (story.Caption?.ToLowerInvariant().Contains(lowerSearchTerm) == true)
            score += 10;

        if (story.LocationName?.ToLowerInvariant().Contains(lowerSearchTerm) == true)
            score += 8;

        if (story.EventType?.ToLowerInvariant().Contains(lowerSearchTerm) == true)
            score += 7;

        if (story.Tags.Any(t => t.ToLowerInvariant().Contains(lowerSearchTerm)))
            score += 9;

        if (story.CarMake?.ToLowerInvariant().Contains(lowerSearchTerm) == true ||
            story.CarModel?.ToLowerInvariant().Contains(lowerSearchTerm) == true)
            score += 6;

        score += story.ViewCount * 0.01;
        score += story.LikeCount * 0.1;
        score += story.ShareCount * 0.2;

        return score;
    }
}