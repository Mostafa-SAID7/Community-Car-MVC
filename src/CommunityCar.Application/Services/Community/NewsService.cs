using AutoMapper;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Features.Community.News.ViewModels;
using CommunityCar.Application.Common.Models;
using CommunityCar.Domain.Entities.Community.News;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Services.Community;

public class NewsService : INewsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly INewsNotificationService _newsNotificationService;

    public NewsService(IUnitOfWork unitOfWork, IMapper mapper, INewsNotificationService newsNotificationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _newsNotificationService = newsNotificationService;
    }

    public async Task<NewsSearchResponse> SearchNewsAsync(NewsSearchVM request)
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
            "newest" => queryable.OrderByDescending(n => n.PublishedAt ?? n.CreatedAt),
            "oldest" => queryable.OrderBy(n => n.PublishedAt ?? n.CreatedAt),
            "mostViewed" => queryable.OrderByDescending(n => n.ViewCount),
            "mostLiked" => queryable.OrderByDescending(n => n.LikeCount),
            "featured" => queryable.OrderByDescending(n => n.IsFeatured).ThenByDescending(n => n.PublishedAt ?? n.CreatedAt),
            _ => queryable.OrderByDescending(n => n.IsPinned).ThenByDescending(n => n.IsFeatured).ThenByDescending(n => n.PublishedAt ?? n.CreatedAt)
        };

        // Apply pagination
        var skip = (request.Page - 1) * request.PageSize;
        var pagedNews = queryable.Skip(skip).Take(request.PageSize).ToList();

        // Map to ViewModels
        var newsVMs = _mapper.Map<List<NewsItemVM>>(pagedNews);

        // Set computed properties
        foreach (var vm in newsVMs)
        {
            SetComputedProperties(vm);
        }

        // Calculate pagination info
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new NewsSearchResponse
        {
            Items = newsVMs,
            TotalCount = totalCount,
            CurrentPage = request.Page,
            PageSize = request.PageSize,
            TotalPages = totalPages,
            HasPreviousPage = request.Page > 1,
            HasNextPage = request.Page < totalPages
        };
    }

    public async Task<NewsItemVM?> GetByIdAsync(Guid id, Guid? currentUserId = null)
    {
        var newsItem = await _unitOfWork.News.GetByIdAsync(id);
        if (newsItem == null) return null;

        var vm = _mapper.Map<NewsItemVM>(newsItem);
        
        // Set computed properties
        SetComputedProperties(vm);
        
        // Set user-specific properties if currentUserId is provided
        if (currentUserId.HasValue)
        {
            vm.CanEdit = newsItem.AuthorId == currentUserId.Value;
            vm.CanDelete = newsItem.AuthorId == currentUserId.Value;
            // TODO: Check if user has liked/bookmarked this news item
        }

        return vm;
    }

    public async Task<NewsItemVM?> GetBySlugAsync(string slug)
    {
        var newsItem = await _unitOfWork.News.GetBySlugAsync(slug);
        if (newsItem == null) return null;
        
        var vm = _mapper.Map<NewsItemVM>(newsItem);
        SetComputedProperties(vm);
        return vm;
    }

    public async Task<Guid> CreateAsync(NewsCreateVM model, Guid authorId)
    {
        var newsItem = new NewsItem(model.Headline, model.Body, authorId, model.Category);
        
        newsItem.UpdateArabicContent(model.HeadlineAr, model.BodyAr, model.SummaryAr);
        
        if (!string.IsNullOrWhiteSpace(model.Summary))
            newsItem.UpdateContent(model.Headline, model.Body, model.Summary);
        
        if (!string.IsNullOrWhiteSpace(model.ImageUrl))
            newsItem.SetMainImage(model.ImageUrl);
        
        newsItem.SetSource(model.Source, model.SourceUrl);
        newsItem.UpdateSeoData(model.MetaTitle, model.MetaDescription);
        newsItem.SetCarInfo(model.CarMake, model.CarModel, model.CarYear);
        
        foreach (var tag in model.GetTagsList())
            newsItem.AddTag(tag);
        
        foreach (var imageUrl in model.ImageUrls)
            newsItem.AddImage(imageUrl);
        
        if (model.IsFeatured)
            newsItem.SetFeatured(true);
        
        if (model.IsPinned)
            newsItem.SetPinned(true);
        
        if (model.PublishImmediately)
        {
            newsItem.Publish();
            
            // Send notification for published news
            var author = await _unitOfWork.Users.GetByIdAsync(authorId);
            if (author != null)
            {
                await _newsNotificationService.NotifyNewsPublishedAsync(newsItem, author.Profile.FullName);
            }
        }

        await _unitOfWork.News.AddAsync(newsItem);
        await _unitOfWork.SaveChangesAsync();

        return newsItem.Id;
    }

    public async Task UpdateAsync(Guid id, NewsEditVM model, Guid currentUserId)
    {
        var newsItem = await _unitOfWork.News.GetByIdAsync(id);
        if (newsItem == null)
            throw new ArgumentException("News item not found");

        if (newsItem.AuthorId != currentUserId)
            throw new UnauthorizedAccessException("You can only edit your own news items");

        newsItem.UpdateContent(model.Headline, model.Body, model.Summary);
        newsItem.UpdateArabicContent(model.HeadlineAr, model.BodyAr, model.SummaryAr);
        newsItem.UpdateCategory(model.Category);
        newsItem.SetSource(model.Source, model.SourceUrl);
        newsItem.UpdateSeoData(model.MetaTitle, model.MetaDescription);
        newsItem.SetCarInfo(model.CarMake, model.CarModel, model.CarYear);
        newsItem.SetFeatured(model.IsFeatured);
        newsItem.SetPinned(model.IsPinned);

        if (!string.IsNullOrWhiteSpace(model.ImageUrl))
            newsItem.SetMainImage(model.ImageUrl);

        // Clear and update tags
        newsItem.ClearTags();
        foreach (var tag in model.GetTagsList())
            newsItem.AddTag(tag);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id, Guid currentUserId)
    {
        var newsItem = await _unitOfWork.News.GetByIdAsync(id);
        if (newsItem == null)
            throw new ArgumentException("News item not found");

        if (newsItem.AuthorId != currentUserId)
            throw new UnauthorizedAccessException("You can only delete your own news items");

        await _unitOfWork.News.DeleteAsync(newsItem);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task PublishAsync(Guid id, Guid currentUserId)
    {
        var newsItem = await _unitOfWork.News.GetByIdAsync(id);
        if (newsItem == null)
            throw new ArgumentException("News item not found");

        if (newsItem.AuthorId != currentUserId)
            throw new UnauthorizedAccessException("You can only publish your own news items");

        newsItem.Publish();
        await _unitOfWork.SaveChangesAsync();

        // Send notification for published news
        var author = await _unitOfWork.Users.GetByIdAsync(currentUserId);
        if (author != null)
        {
            await _newsNotificationService.NotifyNewsPublishedAsync(newsItem, author.Profile.FullName);
        }
    }

    public async Task UnpublishAsync(Guid id, Guid currentUserId)
    {
        var newsItem = await _unitOfWork.News.GetByIdAsync(id);
        if (newsItem == null)
            throw new ArgumentException("News item not found");

        if (newsItem.AuthorId != currentUserId)
            throw new UnauthorizedAccessException("You can only unpublish your own news items");

        newsItem.Unpublish();
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task SetFeaturedAsync(Guid id, bool featured)
    {
        var newsItem = await _unitOfWork.News.GetByIdAsync(id);
        if (newsItem == null)
            throw new ArgumentException("News item not found");

        var wasFeatured = newsItem.IsFeatured;
        newsItem.SetFeatured(featured);
        await _unitOfWork.SaveChangesAsync();

        // Send notification if news item was just featured
        if (featured && !wasFeatured)
        {
            var author = await _unitOfWork.Users.GetByIdAsync(newsItem.AuthorId);
            if (author != null)
            {
                await _newsNotificationService.NotifyNewsFeaturedAsync(newsItem, author.Profile.FullName);
            }
        }
    }

    public async Task SetPinnedAsync(Guid id, bool pinned)
    {
        var newsItem = await _unitOfWork.News.GetByIdAsync(id);
        if (newsItem == null)
            throw new ArgumentException("News item not found");

        newsItem.SetPinned(pinned);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task IncrementViewCountAsync(Guid id)
    {
        var newsItem = await _unitOfWork.News.GetByIdAsync(id);
        if (newsItem == null)
            return;

        newsItem.IncrementViewCount();
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task LikeAsync(Guid id, Guid userId)
    {
        var newsItem = await _unitOfWork.News.GetByIdAsync(id);
        if (newsItem == null)
            throw new ArgumentException("News item not found");

        // TODO: Check if user already liked this news item
        // For now, just increment the count
        newsItem.IncrementLikeCount();
        await _unitOfWork.SaveChangesAsync();

        // Send notification to author
        var liker = await _unitOfWork.Users.GetByIdAsync(userId);
        var author = await _unitOfWork.Users.GetByIdAsync(newsItem.AuthorId);
        if (liker != null && author != null)
        {
            await _newsNotificationService.NotifyNewsLikedAsync(newsItem, author.Profile.FullName, liker.Profile.FullName, userId);
        }
    }

    public async Task UnlikeAsync(Guid id, Guid userId)
    {
        var newsItem = await _unitOfWork.News.GetByIdAsync(id);
        if (newsItem == null)
            throw new ArgumentException("News item not found");

        // TODO: Check if user has liked this news item
        // For now, just decrement the count
        newsItem.DecrementLikeCount();
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task CommentAsync(Guid id, Guid userId)
    {
        var newsItem = await _unitOfWork.News.GetByIdAsync(id);
        if (newsItem == null)
            throw new ArgumentException("News item not found");

        newsItem.IncrementCommentCount();
        await _unitOfWork.SaveChangesAsync();

        // Send notification to author
        var commenter = await _unitOfWork.Users.GetByIdAsync(userId);
        var author = await _unitOfWork.Users.GetByIdAsync(newsItem.AuthorId);
        if (commenter != null && author != null)
        {
            await _newsNotificationService.NotifyNewsCommentedAsync(newsItem, author.Profile.FullName, commenter.Profile.FullName, userId);
        }
    }

    public async Task ShareAsync(Guid id, Guid userId)
    {
        var newsItem = await _unitOfWork.News.GetByIdAsync(id);
        if (newsItem == null)
            throw new ArgumentException("News item not found");

        newsItem.IncrementShareCount();
        await _unitOfWork.SaveChangesAsync();

        // Send notification to author
        var sharer = await _unitOfWork.Users.GetByIdAsync(userId);
        var author = await _unitOfWork.Users.GetByIdAsync(newsItem.AuthorId);
        if (sharer != null && author != null)
        {
            await _newsNotificationService.NotifyNewsSharedAsync(newsItem, author.Profile.FullName, sharer.Profile.FullName, userId);
        }
    }

    public async Task BookmarkAsync(Guid id, Guid userId)
    {
        var newsItem = await _unitOfWork.News.GetByIdAsync(id);
        if (newsItem == null)
            throw new ArgumentException("News item not found");

        // TODO: Implement bookmark functionality with a separate Bookmark entity
        // For now, we'll just track it in memory or use a simple approach
        // This would typically involve creating a Bookmark entity and repository
        
        // Placeholder implementation - in a real scenario, you'd save to a Bookmarks table
        await Task.CompletedTask;
    }

    public async Task UnbookmarkAsync(Guid id, Guid userId)
    {
        var newsItem = await _unitOfWork.News.GetByIdAsync(id);
        if (newsItem == null)
            throw new ArgumentException("News item not found");

        // TODO: Implement unbookmark functionality with a separate Bookmark entity
        // For now, we'll just track it in memory or use a simple approach
        
        // Placeholder implementation - in a real scenario, you'd remove from a Bookmarks table
        await Task.CompletedTask;
    }

    public async Task<int> GetLikeCountAsync(Guid id)
    {
        var newsItem = await _unitOfWork.News.GetByIdAsync(id);
        return newsItem?.LikeCount ?? 0;
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

    private static void SetComputedProperties(NewsItemVM vm)
    {
        // Set TimeAgo
        vm.TimeAgo = GetTimeAgo(vm.PublishedAt ?? vm.CreatedAt);

        // Set ReadingTime (estimate based on word count)
        var wordCount = CountWords(vm.Body);
        vm.ReadingTime = Math.Max(1, (int)Math.Ceiling(wordCount / 200.0)); // Assuming 200 words per minute

        // Set Excerpt (safe substring)
        vm.Excerpt = CreateExcerpt(vm.Summary ?? vm.Body);
    }

    private static string GetTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;

        if (timeSpan.TotalDays >= 365)
            return $"{(int)(timeSpan.TotalDays / 365)} year{((int)(timeSpan.TotalDays / 365) == 1 ? "" : "s")} ago";

        if (timeSpan.TotalDays >= 30)
            return $"{(int)(timeSpan.TotalDays / 30)} month{((int)(timeSpan.TotalDays / 30) == 1 ? "" : "s")} ago";

        if (timeSpan.TotalDays >= 1)
            return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays == 1 ? "" : "s")} ago";

        if (timeSpan.TotalHours >= 1)
            return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours == 1 ? "" : "s")} ago";

        if (timeSpan.TotalMinutes >= 1)
            return $"{(int)timeSpan.TotalMinutes} minute{((int)timeSpan.TotalMinutes == 1 ? "" : "s")} ago";

        return "Just now";
    }

    private static int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        return text.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }

    private static string CreateExcerpt(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Remove HTML tags if any
        var plainText = System.Text.RegularExpressions.Regex.Replace(text, "<.*?>", string.Empty);
        
        // Limit to 150 characters
        if (plainText.Length <= 150)
            return plainText;

        // Find the last space before 150 characters to avoid cutting words
        var excerpt = plainText.Substring(0, 150);
        var lastSpace = excerpt.LastIndexOf(' ');
        
        if (lastSpace > 100) // Only use the last space if it's not too early
            excerpt = excerpt.Substring(0, lastSpace);

        return excerpt + "...";
    }
}


