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

    public async Task<NewsSearchVM> SearchNewsAsync(string searchTerm, string? category = null, int page = 1, int pageSize = 20)
    {
        var news = new List<NewsVM>();
        var random = new Random();

        for (int i = 0; i < pageSize; i++)
        {
            news.Add(new NewsVM
            {
                Id = Guid.NewGuid(),
                Title = $"Search Result {i + 1}: {searchTerm}",
                Content = $"This is the content for search result {i + 1} matching '{searchTerm}'.",
                Category = category ?? ((NewsCategory)(random.Next(0, 5))).ToString(),
                PublishDate = DateTime.UtcNow.AddDays(-random.Next(0, 30)),
                AuthorName = $"Author {i + 1}",
                ViewCount = random.Next(100, 1000),
                IsPublished = true,
                Tags = new List<string> { searchTerm.ToLower(), "search", "result" }
            });
        }

        return new NewsSearchVM
        {
            SearchTerm = searchTerm,
            Category = category,
            Page = page,
            PageSize = pageSize,
            TotalCount = 100, // Mock total
            Items = news
        };
    }

    public async Task<NewsVM?> GetByIdAsync(Guid id)
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

    public async Task<NewsVM> CreateAsync(CreateNewsVM request)
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
            var newsItem = new NewsItem("Sample Headline", "Sample Content", Guid.NewGuid(), NewsCategory.General);
            await _newsNotificationService.NotifyNewsPublishedAsync(newsItem, request.Title);
        }

        return news;
    }

    public async Task<bool> UpdateAsync(Guid id, EditNewsVM request)
    {
        // Mock implementation
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        // Mock implementation
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> PublishAsync(Guid id)
    {
        // Send notification
        var newsItem = new NewsItem("Sample Headline", "Sample Content", Guid.NewGuid(), NewsCategory.General);
        await _newsNotificationService.NotifyNewsPublishedAsync(newsItem, "News Article Title");
        return true;
    }

    public async Task<bool> UnpublishAsync(Guid id)
    {
        // Mock implementation
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> LikeAsync(Guid id, Guid userId)
    {
        // Mock implementation
        await Task.CompletedTask;
        return true;
    }

    public async Task<int> GetLikeCountAsync(Guid id)
    {
        var random = new Random();
        return random.Next(10, 500);
    }

    public async Task<bool> UnlikeAsync(Guid id, Guid userId)
    {
        // Mock implementation
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> BookmarkAsync(Guid id, Guid userId)
    {
        // Mock implementation
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> UnbookmarkAsync(Guid id, Guid userId)
    {
        // Mock implementation
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> CommentAsync(Guid id, Guid userId, string comment)
    {
        // Mock implementation
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> ShareAsync(Guid id, Guid userId, string platform)
    {
        // Mock implementation
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> IncrementViewCountAsync(Guid id)
    {
        // Mock implementation
        await Task.CompletedTask;
        return true;
    }
}