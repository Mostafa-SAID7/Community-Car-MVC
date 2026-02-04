using CommunityCar.Application.Common.Interfaces.Services.Communication;
using CommunityCar.Application.Common.Interfaces.Services.Community.News;
using CommunityCar.Domain.Entities.Community.News;
using CommunityCar.Domain.Enums.Account;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Community.News;

public class NewsNotificationService : INewsNotificationService
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NewsNotificationService> _logger;

    public NewsNotificationService(
        INotificationService notificationService,
        ILogger<NewsNotificationService> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task NotifyNewsPublishedAsync(NewsItem newsItem, string authorName)
    {
        try
        {
            var title = "New Article Published";
            var message = $"{authorName} published a new article: {newsItem.Headline}";
            var actionUrl = $"/news/{newsItem.Id}";

            // Notify followers of the author (this would require a follower system)
            // For now, we'll just log the notification
            _logger.LogInformation("News published notification: {Title} by {Author}", newsItem.Headline, authorName);

            // In a real implementation, you would:
            // 1. Get followers of the author
            // 2. Send notifications to followers
            // await NotifyFollowersOfNewNewsAsync(newsItem, authorName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending news published notification for article {ArticleId}", newsItem.Id);
        }
    }

    public async Task NotifyNewsFeaturedAsync(NewsItem newsItem, string authorName)
    {
        try
        {
            var title = "Article Featured";
            var message = $"Congratulations! Your article '{newsItem.Headline}' has been featured.";
            var actionUrl = $"/news/{newsItem.Id}";

            await _notificationService.SendToUserAsync(
                newsItem.AuthorId,
                title,
                message,
                NotificationType.Achievement,
                actionUrl
            );

            _logger.LogInformation("News featured notification sent to author {AuthorId} for article {ArticleId}", 
                newsItem.AuthorId, newsItem.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending news featured notification for article {ArticleId}", newsItem.Id);
        }
    }

    public async Task NotifyNewsLikedAsync(NewsItem newsItem, string authorName, string likerName, Guid likerId)
    {
        try
        {
            // Don't notify if author liked their own article
            if (newsItem.AuthorId == likerId)
                return;

            var title = "Article Liked";
            var message = $"{likerName} liked your article: {newsItem.Headline}";
            var actionUrl = $"/news/{newsItem.Id}";

            await _notificationService.SendToUserAsync(
                newsItem.AuthorId,
                title,
                message,
                NotificationType.Interaction,
                actionUrl
            );

            _logger.LogInformation("News liked notification sent to author {AuthorId} from {LikerId}", 
                newsItem.AuthorId, likerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending news liked notification for article {ArticleId}", newsItem.Id);
        }
    }

    public async Task NotifyNewsCommentedAsync(NewsItem newsItem, string authorName, string commenterName, Guid commenterId)
    {
        try
        {
            // Don't notify if author commented on their own article
            if (newsItem.AuthorId == commenterId)
                return;

            var title = "New Comment";
            var message = $"{commenterName} commented on your article: {newsItem.Headline}";
            var actionUrl = $"/news/{newsItem.Id}#comments";

            await _notificationService.SendToUserAsync(
                newsItem.AuthorId,
                title,
                message,
                NotificationType.Interaction,
                actionUrl
            );

            _logger.LogInformation("News comment notification sent to author {AuthorId} from {CommenterId}", 
                newsItem.AuthorId, commenterId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending news comment notification for article {ArticleId}", newsItem.Id);
        }
    }

    public async Task NotifyNewsSharedAsync(NewsItem newsItem, string authorName, string sharerName, Guid sharerId)
    {
        try
        {
            // Don't notify if author shared their own article
            if (newsItem.AuthorId == sharerId)
                return;

            var title = "Article Shared";
            var message = $"{sharerName} shared your article: {newsItem.Headline}";
            var actionUrl = $"/news/{newsItem.Id}";

            await _notificationService.SendToUserAsync(
                newsItem.AuthorId,
                title,
                message,
                NotificationType.Interaction,
                actionUrl
            );

            _logger.LogInformation("News shared notification sent to author {AuthorId} from {SharerId}", 
                newsItem.AuthorId, sharerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending news shared notification for article {ArticleId}", newsItem.Id);
        }
    }

    public async Task NotifyBreakingNewsAsync(NewsItem newsItem)
    {
        try
        {
            var title = "Breaking News";
            var message = $"Breaking: {newsItem.Headline}";
            var actionUrl = $"/news/{newsItem.Id}";

            // In a real implementation, you would send this to all users or subscribers
            // For now, we'll just log it
            _logger.LogInformation("Breaking news notification: {Headline}", newsItem.Headline);

            // Example of how you might implement this:
            // var allUserIds = await _userRepository.GetAllUserIdsAsync();
            // await _notificationService.SendToUsersAsync(
            //     allUserIds,
            //     title,
            //     message,
            //     NotificationType.Breaking,
            //     actionUrl
            // );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending breaking news notification for article {ArticleId}", newsItem.Id);
        }
    }

    public async Task NotifyFollowersOfNewNewsAsync(NewsItem newsItem, string authorName)
    {
        try
        {
            var title = "New Article from Author You Follow";
            var message = $"{authorName} published: {newsItem.Headline}";
            var actionUrl = $"/news/{newsItem.Id}";

            // In a real implementation, you would:
            // 1. Get all followers of the author
            // 2. Send notifications to those followers
            
            _logger.LogInformation("Follower notification for new article {ArticleId} by {AuthorName}", 
                newsItem.Id, authorName);

            // Example implementation:
            // var followerIds = await _followRepository.GetFollowerIdsAsync(newsItem.AuthorId);
            // if (followerIds.Any())
            // {
            //     await _notificationService.SendToUsersAsync(
            //         followerIds,
            //         title,
            //         message,
            //         NotificationType.NewContent,
            //         actionUrl
            //     );
            // }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending follower notifications for article {ArticleId}", newsItem.Id);
        }
    }
}