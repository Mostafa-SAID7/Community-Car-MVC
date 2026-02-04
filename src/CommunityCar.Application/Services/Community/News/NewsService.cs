using AutoMapper;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Services.Community.News;
using CommunityCar.Application.Features.Community.News.ViewModels;
using CommunityCar.Application.Common.Models;
using CommunityCar.Domain.Entities.Community.News;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Services.Community.News;

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

    public async Task<object> GetNewsAsync()
    {
        // Return sample news data
        return new
        {
            RecentNews = new[]
            {
                new { Id = 1, Title = "Latest Car News", Summary = "Summary of latest car news", PublishedAt = DateTime.UtcNow.AddHours(-2) },
                new { Id = 2, Title = "Automotive Updates", Summary = "Recent automotive industry updates", PublishedAt = DateTime.UtcNow.AddHours(-5) }
            },
            TotalCount = 2
        };
    }

    public async Task<(IEnumerable<NewsVM> News, PaginationInfo Pagination)> GetNewsAsync(int page = 1, int pageSize = 20, NewsCategory? category = null, string? search = null)
    {
        var news = new List<NewsVM>();
        var random = new Random();

        for (int i = 0; i < pageSize; i++)
        {
            news.Add(new NewsVM
            {
                Id = Guid.NewGuid(),
                Title = $"News Article {i + 1}",
                Content = $"This is the content for news article {i + 1}. Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
                Category = ((NewsCategory)(random.Next(0, 5))).ToString(),
                PublishDate = DateTime.UtcNow.AddDays(-random.Next(0, 30)),
                AuthorName = $"Author {i + 1}",
                ViewCount = random.Next(100, 1000),
                IsPublished = true,
                Tags = new List<string> { "tag1", "tag2", "tag3" }
            });
        }

        var pagination = new PaginationInfo
        {
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = 100,
            TotalPages = (int)Math.Ceiling(100.0 / pageSize)
        };

        return (news, pagination);
    }

    public async Task<NewsVM?> GetNewsByIdAsync(Guid id)
    {
        return new NewsVM
        {
            Id = id,
            Title = "Sample News Article",
            Content = "This is a sample news article content. Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
            Category = NewsCategory.General.ToString(),
            PublishDate = DateTime.UtcNow.AddDays(-1),
            AuthorName = "Sample Author",
            ViewCount = 150,
            IsPublished = true,
            Tags = new List<string> { "sample", "news", "article" }
        };
    }

    public async Task<NewsVM> CreateNewsAsync(CreateNewsVM request)
    {
        var newsId = Guid.NewGuid();
        
        var news = new NewsVM
        {
            Id = newsId,
            Title = request.Title,
            Content = request.Content,
            Category = request.Category,
            PublishDate = DateTime.UtcNow,
            AuthorName = "Current User",
            ViewCount = 0,
            IsPublished = request.IsPublished,
            Tags = request.Tags ?? new List<string>()
        };

        // Send notification if published
        if (request.IsPublished)
        {
            var newsItem = new NewsItem("Sample Headline", "Sample Content", Guid.NewGuid(), NewsCategory.General); // Create a mock NewsItem for notification
            await _newsNotificationService.NotifyNewsPublishedAsync(newsItem, request.Title);
        }

        return news;
    }

    public async Task<bool> UpdateNewsAsync(Guid id, UpdateNewsVM request)
    {
        // Mock implementation
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> DeleteNewsAsync(Guid id)
    {
        // Mock implementation
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> PublishNewsAsync(Guid id)
    {
        // Send notification
        var newsItem = new NewsItem("Sample Headline", "Sample Content", Guid.NewGuid(), NewsCategory.General); // Create a mock NewsItem for notification
        await _newsNotificationService.NotifyNewsPublishedAsync(newsItem, "News Article Title");
        return true;
    }

    public async Task<bool> UnpublishNewsAsync(Guid id)
    {
        // Mock implementation
        await Task.CompletedTask;
        return true;
    }

    public async Task<IEnumerable<NewsVM>> GetFeaturedNewsAsync(int count = 5)
    {
        var news = new List<NewsVM>();
        var random = new Random();

        for (int i = 0; i < count; i++)
        {
            news.Add(new NewsVM
            {
                Id = Guid.NewGuid(),
                Title = $"Featured News {i + 1}",
                Content = $"This is featured news content {i + 1}.",
                Category = ((NewsCategory)(random.Next(0, 5))).ToString(),
                PublishDate = DateTime.UtcNow.AddDays(-random.Next(0, 7)),
                AuthorName = $"Featured Author {i + 1}",
                ViewCount = random.Next(500, 2000),
                IsPublished = true,
                Tags = new List<string> { "featured", "news" }
            });
        }

        return news;
    }

    public async Task<IEnumerable<NewsVM>> GetNewsByAuthorAsync(string authorId, int page = 1, int pageSize = 10)
    {
        var news = new List<NewsVM>();
        var random = new Random();

        for (int i = 0; i < pageSize; i++)
        {
            news.Add(new NewsVM
            {
                Id = Guid.NewGuid(),
                Title = $"Author News {i + 1}",
                Content = $"This is news content by author {authorId}.",
                Category = ((NewsCategory)(random.Next(0, 5))).ToString(),
                PublishDate = DateTime.UtcNow.AddDays(-random.Next(0, 30)),
                AuthorName = "Author Name",
                ViewCount = random.Next(100, 1000),
                IsPublished = true,
                Tags = new List<string> { "author", "news" }
            });
        }

        return news;
    }

    public async Task<IEnumerable<NewsVM>> GetNewsByCategoryAsync(NewsCategory category, int page = 1, int pageSize = 10)
    {
        var news = new List<NewsVM>();
        var random = new Random();

        for (int i = 0; i < pageSize; i++)
        {
            news.Add(new NewsVM
            {
                Id = Guid.NewGuid(),
                Title = $"{category} News {i + 1}",
                Content = $"This is {category} news content.",
                Category = category.ToString(),
                PublishDate = DateTime.UtcNow.AddDays(-random.Next(0, 30)),
                AuthorName = $"Category Author {i + 1}",
                ViewCount = random.Next(100, 1000),
                IsPublished = true,
                Tags = new List<string> { category.ToString().ToLower(), "news" }
            });
        }

        return news;
    }

    public async Task<bool> IncrementViewCountAsync(Guid id)
    {
        // Mock implementation
        await Task.CompletedTask;
        return true;
    }
}