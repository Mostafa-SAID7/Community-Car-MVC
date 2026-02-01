using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Features.Community.Feed.ViewModels;
using CommunityCar.Application.Features.Shared.Interactions.ViewModels;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Services.Community.Feed;

/// <summary>
/// Service responsible for handling feed content interactions
/// </summary>
public class FeedInteractionService : IFeedInteractionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInteractionService _interactionService;

    public FeedInteractionService(IUnitOfWork unitOfWork, IInteractionService interactionService)
    {
        _unitOfWork = unitOfWork;
        _interactionService = interactionService;
    }

    public Task<bool> MarkAsSeenAsync(Guid userId, Guid contentId, string contentType)
    {
        // TODO: Implement marking content as seen
        // This would typically involve creating a UserContentView record
        return Task.FromResult(true);
    }

    public Task<bool> InteractWithContentAsync(Guid userId, Guid contentId, string contentType, string interactionType)
    {
        // TODO: Implement content interaction (like, share, bookmark)
        // This would involve updating the appropriate content and creating interaction records
        return Task.FromResult(true);
    }

    public async Task<bool> AddCommentAsync(Guid userId, Guid contentId, string contentType, string comment)
    {
        try
        {
            // TODO: Implement adding comments to content
            // This would involve creating a comment record and updating comment counts
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<object>> GetCommentsAsync(Guid contentId, string contentType)
    {
        try
        {
            // TODO: Implement getting comments for content
            // For now, return mock data
            var comments = new List<object>
            {
                new
                {
                    id = Guid.NewGuid(),
                    authorName = "John Doe",
                    authorAvatar = "/images/default-avatar.png",
                    content = "Great post! Thanks for sharing.",
                    timeAgo = "2 hours ago",
                    createdAt = DateTime.UtcNow.AddHours(-2)
                },
                new
                {
                    id = Guid.NewGuid(),
                    authorName = "Jane Smith",
                    authorAvatar = "/images/default-avatar.png",
                    content = "I completely agree with this review.",
                    timeAgo = "1 hour ago",
                    createdAt = DateTime.UtcNow.AddHours(-1)
                }
            };
            
            return comments;
        }
        catch
        {
            return new List<object>();
        }
    }

    public async Task<bool> BookmarkContentAsync(Guid userId, Guid contentId, string contentType)
    {
        try
        {
            if (!Enum.TryParse<EntityType>(contentType, true, out var entityType))
                return false;

            var existingBookmark = await _unitOfWork.Bookmarks.GetUserBookmarkAsync(contentId, entityType, userId);
            
            if (existingBookmark != null)
            {
                await _unitOfWork.Bookmarks.DeleteAsync(existingBookmark);
            }
            else
            {
                var bookmark = new CommunityCar.Domain.Entities.Shared.Bookmark(contentId, entityType, userId);
                await _unitOfWork.Bookmarks.AddAsync(bookmark);
            }
            
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error bookmarking content: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> HideContentAsync(Guid userId, Guid contentId, string contentType)
    {
        try
        {
            // Placeholder: UserPreferences or HiddenContent would be needed
            // For now, simulate success so the UI hides it for the session
            await Task.Delay(100); 
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ReportContentAsync(Guid userId, Guid contentId, string contentType, string reason)
    {
        try
        {
            // Placeholder: Reports repository would be needed
            // For now, simulate success
            Console.WriteLine($"[REPORT] User {userId} reported content {contentType}/{contentId}: {reason}");
            await Task.Delay(100);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task LoadInitialCommentsAsync(List<CommunityCar.Application.Features.Community.Feed.ViewModels.FeedItemVM> feedItems, Guid? userId)
    {
        if (feedItems == null || !feedItems.Any())
            return;
            
        foreach (var item in feedItems)
        {
            try
            {
                // Parse EntityType from ContentType string
                if (!Enum.TryParse<EntityType>(item.ContentType, true, out var entityType))
                    continue;
                    
                // Load first 3 comments
                var comments = await _interactionService.GetEntityCommentsAsync(item.Id, entityType, page: 1, pageSize: 3);
                item.InitialComments = comments?.Select(c => new CommentItemVM
                {
                    Id = c.Id,
                    Content = c.Content,
                    AuthorName = c.AuthorName,
                    AuthorAvatar = c.AuthorAvatar,
                    CreatedAt = c.CreatedAt,
                    LikeCount = c.LikeCount
                }).ToList() ?? new List<CommentItemVM>();
            }
            catch (Exception ex)
            {
                // Log error but don't fail the whole feed
                Console.WriteLine($"Error loading comments for feed item {item.Id}: {ex.Message}");
                item.InitialComments = new List<CommentItemVM>();
            }
        }
    }
}


