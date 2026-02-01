using CommunityCar.Application.Features.Shared.Interactions.ViewModels;

namespace CommunityCar.Application.Services.Community.Feed;

/// <summary>
/// Service responsible for handling feed content interactions
/// </summary>
public interface IFeedInteractionService
{
    /// <summary>
    /// Marks content as seen by user
    /// </summary>
    Task<bool> MarkAsSeenAsync(Guid userId, Guid contentId, string contentType);

    /// <summary>
    /// Handles user interaction with content (like, share, bookmark)
    /// </summary>
    Task<bool> InteractWithContentAsync(Guid userId, Guid contentId, string contentType, string interactionType);

    /// <summary>
    /// Adds a comment to content
    /// </summary>
    Task<bool> AddCommentAsync(Guid userId, Guid contentId, string contentType, string comment);

    /// <summary>
    /// Gets comments for content
    /// </summary>
    Task<IEnumerable<object>> GetCommentsAsync(Guid contentId, string contentType);

    /// <summary>
    /// Bookmarks or unbookmarks content
    /// </summary>
    Task<bool> BookmarkContentAsync(Guid userId, Guid contentId, string contentType);

    /// <summary>
    /// Hides content from user's feed
    /// </summary>
    Task<bool> HideContentAsync(Guid userId, Guid contentId, string contentType);

    /// <summary>
    /// Reports content for moderation
    /// </summary>
    Task<bool> ReportContentAsync(Guid userId, Guid contentId, string contentType, string reason);

    /// <summary>
    /// Loads initial comments for feed items
    /// </summary>
    Task LoadInitialCommentsAsync(List<CommunityCar.Application.Features.Community.Feed.ViewModels.FeedItemVM> feedItems, Guid? userId);
}


