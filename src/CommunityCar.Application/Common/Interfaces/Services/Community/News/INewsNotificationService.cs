using CommunityCar.Domain.Entities.Community.News;

namespace CommunityCar.Application.Common.Interfaces.Services.Community.News;

public interface INewsNotificationService
{
    Task NotifyNewsPublishedAsync(NewsItem newsItem, string authorName);
    Task NotifyNewsFeaturedAsync(NewsItem newsItem, string authorName);
    Task NotifyNewsLikedAsync(NewsItem newsItem, string authorName, string likerName, Guid likerId);
    Task NotifyNewsCommentedAsync(NewsItem newsItem, string authorName, string commenterName, Guid commenterId);
    Task NotifyNewsSharedAsync(NewsItem newsItem, string authorName, string sharerName, Guid sharerId);
    Task NotifyBreakingNewsAsync(NewsItem newsItem);
    Task NotifyFollowersOfNewNewsAsync(NewsItem newsItem, string authorName);
}