using AutoMapper;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Features.News.DTOs;
using CommunityCar.Application.Features.News.ViewModels;
using CommunityCar.Application.Common.Models;
using CommunityCar.Domain.Entities.Community.News;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Services.Community;

public class NewsService : INewsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public NewsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<NewsSearchResponse> SearchNewsAsync(NewsSearchRequest request)
    {
        var newsItems = await _unitOfWork.News.GetAllAsync();
        var queryable = newsItems.AsQueryable();

        // Apply search term filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLowerInvariant();
            queryable = queryable.Where(n => 
                n.Headline.ToLowerInvariant().Contains(searchTerm) ||
                n.Body.ToLowerInvariant().Contains(searchTerm) ||
                (n.Summary != null && n.Summary.ToLowerInvariant().Contains(searchTerm)) ||
                (n.CarMake != null && n.CarMake.ToLowerInvariant().Contains(searchTerm)) ||
                (n.CarModel != null && n.CarModel.ToLowerInvariant().Contains(searchTerm)) ||
                n.Tags.Any(t => t.ToLowerInvariant().Contains(searchTerm))
            );
        }

        // Apply category filter
        if (request.Category.HasValue)
        {
            queryable = queryable.Where(n => n.Category == request.Category.Value);
        }

        // Apply car filters
        if (!string.IsNullOrWhiteSpace(request.CarMake))
        {
            queryable = queryable.Where(n => n.CarMake != null && n.CarMake.ToLowerInvariant() == request.CarMake.ToLowerInvariant());
        }

        if (!string.IsNullOrWhiteSpace(request.CarModel))
        {
            queryable = queryable.Where(n => n.CarModel != null && n.CarModel.ToLowerInvariant() == request.CarModel.ToLowerInvariant());
        }

        if (request.CarYear.HasValue)
        {
            queryable = queryable.Where(n => n.CarYear == request.CarYear.Value);
        }

        // Apply tag filters
        if (request.Tags.Any())
        {
            var lowerTags = request.Tags.Select(t => t.ToLowerInvariant()).ToList();
            queryable = queryable.Where(n => n.Tags.Any(t => lowerTags.Contains(t.ToLowerInvariant())));
        }

        // Apply author filter
        if (request.AuthorId.HasValue)
        {
            queryable = queryable.Where(n => n.AuthorId == request.AuthorId.Value);
        }

        // Apply boolean filters
        if (request.IsPublished.HasValue)
        {
            queryable = queryable.Where(n => n.IsPublished == request.IsPublished.Value);
        }

        if (request.IsFeatured.HasValue)
        {
            queryable = queryable.Where(n => n.IsFeatured == request.IsFeatured.Value);
        }

        if (request.IsPinned.HasValue)
        {
            queryable = queryable.Where(n => n.IsPinned == request.IsPinned.Value);
        }

        // Apply date filters
        if (request.PublishedAfter.HasValue)
        {
            queryable = queryable.Where(n => n.PublishedAt >= request.PublishedAfter.Value);
        }

        if (request.PublishedBefore.HasValue)
        {
            queryable = queryable.Where(n => n.PublishedAt <= request.PublishedBefore.Value);
        }

        // Apply engagement filters
        if (request.MinViews.HasValue)
        {
            queryable = queryable.Where(n => n.ViewCount >= request.MinViews.Value);
        }

        if (request.MaxViews.HasValue)
        {
            queryable = queryable.Where(n => n.ViewCount <= request.MaxViews.Value);
        }

        if (request.MinLikes.HasValue)
        {
            queryable = queryable.Where(n => n.LikeCount >= request.MinLikes.Value);
        }

        if (request.MaxLikes.HasValue)
        {
            queryable = queryable.Where(n => n.LikeCount <= request.MaxLikes.Value);
        }

        if (request.MinComments.HasValue)
        {
            queryable = queryable.Where(n => n.CommentCount >= request.MinComments.Value);
        }

        if (request.MaxComments.HasValue)
        {
            queryable = queryable.Where(n => n.CommentCount <= request.MaxComments.Value);
        }

        // Get total count before pagination
        var totalCount = queryable.Count();

        // Apply sorting
        queryable = request.SortBy switch
        {
            NewsSortBy.Newest => queryable.OrderByDescending(n => n.PublishedAt),
            NewsSortBy.Oldest => queryable.OrderBy(n => n.PublishedAt),
            NewsSortBy.MostViews => queryable.OrderByDescending(n => n.ViewCount),
            NewsSortBy.LeastViews => queryable.OrderBy(n => n.ViewCount),
            NewsSortBy.MostLikes => queryable.OrderByDescending(n => n.LikeCount),
            NewsSortBy.LeastLikes => queryable.OrderBy(n => n.LikeCount),
            NewsSortBy.MostComments => queryable.OrderByDescending(n => n.CommentCount),
            NewsSortBy.LeastComments => queryable.OrderBy(n => n.CommentCount),
            NewsSortBy.MostShares => queryable.OrderByDescending(n => n.ShareCount),
            NewsSortBy.Relevance => !string.IsNullOrWhiteSpace(request.SearchTerm) 
                ? queryable.OrderByDescending(n => CalculateRelevanceScore(n, request.SearchTerm))
                : queryable.OrderByDescending(n => n.PublishedAt),
            _ => queryable.OrderByDescending(n => n.IsPinned).ThenByDescending(n => n.IsFeatured).ThenByDescending(n => n.PublishedAt)
        };

        // Apply pagination
        var skip = (request.Page - 1) * request.PageSize;
        var pagedNews = queryable.Skip(skip).Take(request.PageSize).ToList();

        // Map to ViewModels
        var newsVMs = _mapper.Map<List<NewsItemVM>>(pagedNews);

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

        // Get stats
        var stats = await GetNewsStatsAsync();

        // Get available filters
        var availableTags = await GetPopularTagsAsync(50);
        var availableCarMakes = await GetAvailableCarMakesAsync();

        return new NewsSearchResponse
        {
            NewsItems = newsVMs,
            Pagination = pagination,
            Stats = stats,
            AvailableTags = availableTags,
            AvailableCarMakes = availableCarMakes
        };
    }

    public async Task<NewsItemVM?> GetByIdAsync(Guid id)
    {
        var newsItem = await _unitOfWork.News.GetByIdAsync(id);
        return newsItem != null ? _mapper.Map<NewsItemVM>(newsItem) : null;
    }

    public async Task<NewsItemVM?> GetBySlugAsync(string slug)
    {
        var newsItem = await _unitOfWork.News.GetBySlugAsync(slug);
        return newsItem != null ? _mapper.Map<NewsItemVM>(newsItem) : null;
    }

    public async Task<NewsItemVM> CreateAsync(CreateNewsRequest request)
    {
        var newsItem = new NewsItem(request.Headline, request.Body, request.AuthorId, request.Category);
        
        if (!string.IsNullOrWhiteSpace(request.Summary))
            newsItem.UpdateContent(request.Headline, request.Body, request.Summary);
        
        if (!string.IsNullOrWhiteSpace(request.ImageUrl))
            newsItem.SetMainImage(request.ImageUrl);
        
        newsItem.SetSource(request.Source, request.SourceUrl);
        newsItem.UpdateSeoData(request.MetaTitle, request.MetaDescription);
        newsItem.SetCarInfo(request.CarMake, request.CarModel, request.CarYear);
        
        foreach (var tag in request.Tags)
            newsItem.AddTag(tag);
        
        foreach (var imageUrl in request.ImageUrls)
            newsItem.AddImage(imageUrl);
        
        if (request.IsFeatured)
            newsItem.SetFeatured(true);
        
        if (request.IsPinned)
            newsItem.SetPinned(true);
        
        if (request.PublishImmediately)
            newsItem.Publish();

        await _unitOfWork.News.AddAsync(newsItem);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<NewsItemVM>(newsItem);
    }

    public async Task<NewsItemVM> UpdateAsync(Guid id, UpdateNewsRequest request)
    {
        var newsItem = await _unitOfWork.News.GetByIdAsync(id);
        if (newsItem == null)
            throw new ArgumentException("News item not found");

        newsItem.UpdateContent(request.Headline, request.Body, request.Summary);
        newsItem.UpdateCategory(request.Category);
        newsItem.SetSource(request.Source, request.SourceUrl);
        newsItem.UpdateSeoData(request.MetaTitle, request.MetaDescription);
        newsItem.SetCarInfo(request.CarMake, request.CarModel, request.CarYear);
        newsItem.SetFeatured(request.IsFeatured);
        newsItem.SetPinned(request.IsPinned);

        if (!string.IsNullOrWhiteSpace(request.ImageUrl))
            newsItem.SetMainImage(request.ImageUrl);

        // Update tags
        foreach (var tag in request.Tags)
            newsItem.AddTag(tag);

        // Update images
        foreach (var imageUrl in request.ImageUrls)
            newsItem.AddImage(imageUrl);

        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<NewsItemVM>(newsItem);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var newsItem = await _unitOfWork.News.GetByIdAsync(id);
        if (newsItem == null)
            return false;

        await _unitOfWork.News.DeleteAsync(newsItem);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> PublishAsync(Guid id)
    {
        var newsItem = await _unitOfWork.News.GetByIdAsync(id);
        if (newsItem == null)
            return false;

        newsItem.Publish();
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnpublishAsync(Guid id)
    {
        var newsItem = await _unitOfWork.News.GetByIdAsync(id);
        if (newsItem == null)
            return false;

        newsItem.Unpublish();
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetFeaturedAsync(Guid id, bool featured)
    {
        var newsItem = await _unitOfWork.News.GetByIdAsync(id);
        if (newsItem == null)
            return false;

        newsItem.SetFeatured(featured);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetPinnedAsync(Guid id, bool pinned)
    {
        var newsItem = await _unitOfWork.News.GetByIdAsync(id);
        if (newsItem == null)
            return false;

        newsItem.SetPinned(pinned);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IncrementViewCountAsync(Guid id)
    {
        var newsItem = await _unitOfWork.News.GetByIdAsync(id);
        if (newsItem == null)
            return false;

        newsItem.IncrementViewCount();
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<string>> GetPopularTagsAsync(int count = 20)
    {
        return await _unitOfWork.News.GetPopularTagsAsync(count);
    }

    public async Task<IEnumerable<string>> GetAvailableCarMakesAsync()
    {
        return await _unitOfWork.News.GetAvailableCarMakesAsync();
    }

    public async Task<NewsStatsVM> GetNewsStatsAsync()
    {
        var allNews = await _unitOfWork.News.GetAllAsync();
        var published = allNews.Where(n => n.IsPublished);
        var thisMonth = allNews.Where(n => n.CreatedAt >= DateTime.UtcNow.AddMonths(-1));
        var thisWeek = allNews.Where(n => n.CreatedAt >= DateTime.UtcNow.AddDays(-7));
        var today = allNews.Where(n => n.CreatedAt.Date == DateTime.UtcNow.Date);

        return new NewsStatsVM
        {
            TotalNews = allNews.Count(),
            PublishedNews = published.Count(),
            DraftNews = allNews.Count(n => !n.IsPublished),
            FeaturedNews = allNews.Count(n => n.IsFeatured),
            PinnedNews = allNews.Count(n => n.IsPinned),
            TotalViews = allNews.Sum(n => n.ViewCount),
            TotalLikes = allNews.Sum(n => n.LikeCount),
            TotalComments = allNews.Sum(n => n.CommentCount),
            TotalShares = allNews.Sum(n => n.ShareCount),
            NewsThisMonth = thisMonth.Count(),
            NewsThisWeek = thisWeek.Count(),
            NewsToday = today.Count(),
            AverageViewsPerNews = allNews.Any() ? allNews.Average(n => n.ViewCount) : 0,
            AverageLikesPerNews = allNews.Any() ? allNews.Average(n => n.LikeCount) : 0,
            AverageCommentsPerNews = allNews.Any() ? allNews.Average(n => n.CommentCount) : 0
        };
    }

    private static double CalculateRelevanceScore(NewsItem newsItem, string searchTerm)
    {
        var score = 0.0;
        var lowerSearchTerm = searchTerm.ToLowerInvariant();

        // Title matches get higher score
        if (newsItem.Headline.ToLowerInvariant().Contains(lowerSearchTerm))
            score += 10;

        // Body matches get medium score
        if (newsItem.Body.ToLowerInvariant().Contains(lowerSearchTerm))
            score += 5;

        // Summary matches get medium score
        if (newsItem.Summary?.ToLowerInvariant().Contains(lowerSearchTerm) == true)
            score += 7;

        // Tag matches get high score
        if (newsItem.Tags.Any(t => t.ToLowerInvariant().Contains(lowerSearchTerm)))
            score += 8;

        // Car info matches get medium score
        if (newsItem.CarMake?.ToLowerInvariant().Contains(lowerSearchTerm) == true ||
            newsItem.CarModel?.ToLowerInvariant().Contains(lowerSearchTerm) == true)
            score += 6;

        // Boost score based on engagement
        score += newsItem.ViewCount * 0.01;
        score += newsItem.LikeCount * 0.1;
        score += newsItem.CommentCount * 0.2;

        return score;
    }
}