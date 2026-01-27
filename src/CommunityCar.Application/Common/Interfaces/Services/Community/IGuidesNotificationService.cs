using CommunityCar.Domain.Entities.Community.Guides;

namespace CommunityCar.Application.Common.Interfaces.Services.Community;

public interface IGuidesNotificationService
{
    Task NotifyGuidePublishedAsync(Guide guide, string authorName);
    Task NotifyGuideVerifiedAsync(Guide guide, string authorName);
    Task NotifyGuideFeaturedAsync(Guide guide, string authorName);
    Task NotifyGuideBookmarkedAsync(Guide guide, string authorName, string bookmarkerName, Guid bookmarkerId);
    Task NotifyGuideRatedAsync(Guide guide, string authorName, string raterName, Guid raterId, double rating);
    Task NotifyGuideCommentedAsync(Guide guide, string authorName, string commenterName, Guid commenterId, string commentText);
    Task NotifyGuideUpdatedAsync(Guide guide, string authorName);
    Task NotifyNewGuideFromFollowedAuthorAsync(Guide guide, string authorName, List<Guid> followerIds);
    Task NotifyGuideDeletedAsync(Guid guideId, string guideTitle, Guid authorId, string authorName);
}


