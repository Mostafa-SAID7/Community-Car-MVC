using CommunityCar.Application.Features.Shared.Interactions.ViewModels;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Common.Interfaces.Services.Community.Interactions;

public interface IInteractionService
{
    // Reactions
    Task<ReactionResultVM> AddReactionAsync(Guid entityId, EntityType entityType, Guid userId, ReactionType reactionType);
    Task<ReactionResultVM> RemoveReactionAsync(Guid entityId, EntityType entityType, Guid userId);
    Task<ReactionResultVM> UpdateReactionAsync(Guid entityId, EntityType entityType, Guid userId, ReactionType newReactionType);
    Task<ReactionSummaryVM> GetReactionSummaryAsync(Guid entityId, EntityType entityType, Guid? userId = null);
    Task<List<ReactionVM>> GetEntityReactionsAsync(Guid entityId, EntityType entityType);

    // Comments
    Task<CommentVM> AddCommentAsync(CreateCommentVM request);
    Task<CommentVM> UpdateCommentAsync(Guid commentId, string newContent, Guid userId);
    Task<bool> DeleteCommentAsync(Guid commentId, Guid userId);
    Task<CommentVM> AddReplyAsync(CreateReplyVM request);
    Task<List<CommentVM>> GetEntityCommentsAsync(Guid entityId, EntityType entityType, int page = 1, int pageSize = 10);
    Task<List<CommentVM>> GetCommentRepliesAsync(Guid parentCommentId);
    Task<int> GetEntityCommentCountAsync(Guid entityId, EntityType entityType);
    Task<int> GetTotalTopLevelCommentCountAsync(Guid entityId, EntityType entityType);

    // Shares
    Task<ShareResultVM> ShareEntityAsync(ShareEntityVM request);
    Task<ShareSummaryVM> GetShareSummaryAsync(Guid entityId, EntityType entityType);
    Task<List<ShareVM>> GetEntitySharesAsync(Guid entityId, EntityType entityType);
    Task<string> GenerateShareUrlAsync(Guid entityId, EntityType entityType);
    Task<ShareMetadataVM> GetShareMetadataAsync(Guid entityId, EntityType entityType);

    // Bookmarks
    Task<bool> BookmarkEntityAsync(Guid entityId, EntityType entityType, Guid userId);
    Task<bool> RemoveBookmarkAsync(Guid entityId, EntityType entityType, Guid userId);

    // Views
    Task TrackViewAsync(Guid entityId, EntityType entityType, Guid? userId = null);

    // Combined Summary
    Task<InteractionSummaryVM> GetInteractionSummaryAsync(Guid entityId, EntityType entityType, Guid? userId = null);
}