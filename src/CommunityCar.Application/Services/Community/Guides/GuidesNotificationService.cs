using CommunityCar.Application.Common.Interfaces.Services.Community.Guides;
using CommunityCar.Domain.Entities.Community.Guides;
using CommunityCar.Domain.Enums;
using CommunityCar.Application.Common.Interfaces.Hubs;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Community.Guides;

public class GuidesNotificationService : IGuidesNotificationService
{
    private readonly INotificationHubContext _hubContext;
    private readonly ILogger<GuidesNotificationService> _logger;

    public GuidesNotificationService(
        INotificationHubContext hubContext,
        ILogger<GuidesNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifyGuidePublishedAsync(Guide guide, string authorName)
    {
        _logger.LogInformation($"Notifying guide published: {guide.Id}");
        await _hubContext.SendNotificationToAllAsync("ReceiveNotification", new
        {
            Type = "GuidePublished",
            Title = "New Guide Published",
            Message = $"{authorName} published a new guide: {guide.Title}",
            GuideId = guide.Id
        });
    }

    public async Task NotifyGuideVerifiedAsync(Guide guide, string authorName)
    {
        _logger.LogInformation($"Notifying guide verified: {guide.Id}");
        await _hubContext.SendNotificationToUserAsync(guide.AuthorId.ToString(), "ReceiveNotification", new
        {
            Type = "GuideVerified",
            Title = "Guide Verified",
            Message = $"Your guide '{guide.Title}' has been verified!",
            GuideId = guide.Id
        });
    }

    public async Task NotifyGuideFeaturedAsync(Guide guide, string authorName)
    {
        _logger.LogInformation($"Notifying guide featured: {guide.Id}");
        await _hubContext.SendNotificationToAllAsync("ReceiveNotification", new
        {
            Type = "GuideFeatured",
            Title = "Featured Guide",
            Message = $"Check out this featured guide: {guide.Title}",
            GuideId = guide.Id
        });
    }

    public async Task NotifyGuideBookmarkedAsync(Guide guide, string authorName, string bookmarkerName, Guid bookmarkerId)
    {
        _logger.LogInformation($"Notifying guide bookmarked: {guide.Id} by {bookmarkerId}");
        await _hubContext.SendNotificationToUserAsync(guide.AuthorId.ToString(), "ReceiveNotification", new
        {
            Type = "GuideBookmarked",
            Title = "Guide Bookmarked",
            Message = $"{bookmarkerName} bookmarked your guide: {guide.Title}",
            GuideId = guide.Id
        });
    }

    public async Task NotifyGuideRatedAsync(Guide guide, string authorName, string raterName, Guid raterId, double rating)
    {
        _logger.LogInformation($"Notifying guide rated: {guide.Id} by {raterId}");
        await _hubContext.SendNotificationToUserAsync(guide.AuthorId.ToString(), "ReceiveNotification", new
        {
            Type = "GuideRated",
            Title = "New Rating",
            Message = $"{raterName} rated your guide '{guide.Title}' {rating} stars.",
            GuideId = guide.Id
        });
    }

    public async Task NotifyGuideCommentedAsync(Guide guide, string authorName, string commenterName, Guid commenterId, string commentText)
    {
        _logger.LogInformation($"Notifying guide commented: {guide.Id} by {commenterId}");
        await _hubContext.SendNotificationToUserAsync(guide.AuthorId.ToString(), "ReceiveNotification", new
        {
            Type = "GuideCommented",
            Title = "New Comment",
            Message = $"{commenterName} commented on your guide: {guide.Title}",
            GuideId = guide.Id
        });
    }

    public async Task NotifyGuideUpdatedAsync(Guide guide, string authorName)
    {
        _logger.LogInformation($"Notifying guide updated: {guide.Id}");
        // Optional: Notify followers or people who bookmarked it
    }

    public async Task NotifyNewGuideFromFollowedAuthorAsync(Guide guide, string authorName, List<Guid> followerIds)
    {
        _logger.LogInformation($"Notifying new guide from followed author: {guide.Id}");
        foreach (var followerId in followerIds)
        {
            await _hubContext.SendNotificationToUserAsync(followerId.ToString(), "ReceiveNotification", new
            {
                Type = "NewGuideFromFollowed",
                Title = "New Guide",
                Message = $"{authorName} posted a new guide: {guide.Title}",
                GuideId = guide.Id
            });
        }
    }

    public async Task NotifyGuideDeletedAsync(Guid guideId, string guideTitle, Guid authorId, string authorName)
    {
        _logger.LogInformation($"Notifying guide deleted: {guideId}");
    }
}