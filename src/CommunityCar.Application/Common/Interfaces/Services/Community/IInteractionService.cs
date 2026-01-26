using CommunityCar.Domain.Enums;
using CommunityCar.Application.Features.Interactions.ViewModels;
using CommunityCar.Application.Features.Interactions.DTOs;

namespace CommunityCar.Application.Common.Interfaces.Services.Community;

public interface IInteractionService
{
    // Reaction methods
    Task<ReactionResultVM> AddReactionAsync(Guid entityId, EntityType entityType, Guid userId, ReactionType reactionType);
    Task<ReactionResultVM> RemoveReactionAsync(Guid entityId, EntityType entityType, Guid userId);
    Task<ReactionResultVM> UpdateReactionAsync(Guid entityId, EntityType entityType, Guid userId, ReactionType newReactionType);
    Task<ReactionSummaryVM> GetReactionSummaryAsync(Guid entityId, EntityType entityType, Guid? userId = null);
    Task<List<ReactionVM>> GetEntityReactionsAsync(Guid entityId, EntityType entityType);

    // Comment methods
    Task<CommentVM> AddCommentAsync(CreateCommentRequest request);
    Task<CommentVM> UpdateCommentAsync(Guid commentId, string newContent, Guid userId);
    Task<bool> DeleteCommentAsync(Guid commentId, Guid userId);
    Task<CommentVM> AddReplyAsync(CreateReplyRequest request);
    Task<List<CommentVM>> GetEntityCommentsAsync(Guid entityId, EntityType entityType, int page = 1, int pageSize = 20);
    Task<List<CommentVM>> GetCommentRepliesAsync(Guid parentCommentId);
    Task<int> GetEntityCommentCountAsync(Guid entityId, EntityType entityType);

    // Share methods
    Task<ShareResultVM> ShareEntityAsync(ShareEntityRequest request);
    Task<ShareSummaryVM> GetShareSummaryAsync(Guid entityId, EntityType entityType);
    Task<List<ShareVM>> GetEntitySharesAsync(Guid entityId, EntityType entityType);
    Task<string> GenerateShareUrlAsync(Guid entityId, EntityType entityType);
    Task<ShareMetadataVM> GetShareMetadataAsync(Guid entityId, EntityType entityType);

    // Bookmark methods
    Task<bool> BookmarkEntityAsync(Guid entityId, EntityType entityType, Guid userId);
    Task<bool> RemoveBookmarkAsync(Guid entityId, EntityType entityType, Guid userId);

    // View tracking methods
    Task TrackViewAsync(Guid entityId, EntityType entityType, Guid? userId = null);

    // Combined interaction summary
    Task<InteractionSummaryVM> GetInteractionSummaryAsync(Guid entityId, EntityType entityType, Guid? userId = null);
}