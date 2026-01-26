using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Services.Communication;
using CommunityCar.Application.Features.Interactions.ViewModels;
using CommunityCar.Application.Features.Interactions.DTOs;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Services.Community;

public partial class InteractionService : IInteractionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public InteractionService(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    #region Reactions

    public async Task<ReactionResultVM> AddReactionAsync(Guid entityId, EntityType entityType, Guid userId, ReactionType reactionType)
    {
        try
        {
            // Check if user already reacted
            var existingReaction = await _unitOfWork.Reactions.GetUserReactionAsync(entityId, entityType, userId);
            
            if (existingReaction != null)
            {
                // Update existing reaction
                existingReaction.UpdateType(reactionType);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                // Create new reaction
                var reaction = new Reaction(entityId, entityType, userId, reactionType);
                await _unitOfWork.Reactions.AddAsync(reaction);
                await _unitOfWork.SaveChangesAsync();

                // Send notification to content owner (if not self-reaction)
                await SendReactionNotificationAsync(entityId, entityType, userId, reactionType);
            }

            var summary = await GetReactionSummaryAsync(entityId, entityType, userId);
            
            return new ReactionResultVM
            {
                Success = true,
                Message = "Reaction added successfully",
                Summary = summary
            };
        }
        catch (Exception ex)
        {
            return new ReactionResultVM
            {
                Success = false,
                Message = $"Failed to add reaction: {ex.Message}"
            };
        }
    }

    public async Task<ReactionResultVM> RemoveReactionAsync(Guid entityId, EntityType entityType, Guid userId)
    {
        try
        {
            await _unitOfWork.Reactions.RemoveUserReactionAsync(entityId, entityType, userId);
            await _unitOfWork.SaveChangesAsync();

            var summary = await GetReactionSummaryAsync(entityId, entityType, userId);
            
            return new ReactionResultVM
            {
                Success = true,
                Message = "Reaction removed successfully",
                Summary = summary
            };
        }
        catch (Exception ex)
        {
            return new ReactionResultVM
            {
                Success = false,
                Message = $"Failed to remove reaction: {ex.Message}"
            };
        }
    }

    public async Task<ReactionResultVM> UpdateReactionAsync(Guid entityId, EntityType entityType, Guid userId, ReactionType newReactionType)
    {
        return await AddReactionAsync(entityId, entityType, userId, newReactionType);
    }

    public async Task<ReactionSummaryVM> GetReactionSummaryAsync(Guid entityId, EntityType entityType, Guid? userId = null)
    {
        var reactionCounts = await _unitOfWork.Reactions.GetReactionCountsAsync(entityId, entityType);
        var totalReactions = reactionCounts.Values.Sum();
        
        ReactionType? userReaction = null;
        bool hasUserReacted = false;

        if (userId.HasValue)
        {
            var userReactionEntity = await _unitOfWork.Reactions.GetUserReactionAsync(entityId, entityType, userId.Value);
            if (userReactionEntity != null)
            {
                userReaction = userReactionEntity.Type;
                hasUserReacted = true;
            }
        }

        var availableReactions = Enum.GetValues<ReactionType>()
            .Select(rt => new ReactionTypeInfoVM
            {
                Type = rt,
                Display = GetReactionDisplay(rt),
                Icon = GetReactionIcon(rt),
                Color = GetReactionColor(rt),
                Count = reactionCounts.GetValueOrDefault(rt, 0)
            })
            .ToList();

        return new ReactionSummaryVM
        {
            ReactionCounts = reactionCounts,
            TotalReactions = totalReactions,
            UserReaction = userReaction,
            HasUserReacted = hasUserReacted,
            AvailableReactions = availableReactions
        };
    }

    public async Task<List<ReactionVM>> GetEntityReactionsAsync(Guid entityId, EntityType entityType)
    {
        var reactions = await _unitOfWork.Reactions.GetEntityReactionsAsync(entityId, entityType);
        var result = new List<ReactionVM>();

        foreach (var reaction in reactions)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(reaction.UserId);
            result.Add(new ReactionVM
            {
                Id = reaction.Id,
                UserId = reaction.UserId,
                UserName = user?.FullName ?? "Unknown User",
                UserAvatar = user?.ProfilePictureUrl,
                Type = reaction.Type,
                TypeDisplay = GetReactionDisplay(reaction.Type),
                TypeIcon = GetReactionIcon(reaction.Type),
                CreatedAt = reaction.CreatedAt,
                TimeAgo = GetTimeAgo(reaction.CreatedAt)
            });
        }

        return result;
    }

    #endregion

    #region Comments

    public async Task<CommentVM> AddCommentAsync(CreateCommentRequest request)
    {
        var comment = new Comment(request.Content, request.EntityId, request.EntityType, request.AuthorId, request.ParentCommentId);
        await _unitOfWork.Comments.AddAsync(comment);
        await _unitOfWork.SaveChangesAsync();

        // Send notification to content owner
        await SendCommentNotificationAsync(request.EntityId, request.EntityType, request.AuthorId);

        return await MapCommentToVMAsync(comment, request.AuthorId);
    }

    public async Task<CommentVM> UpdateCommentAsync(Guid commentId, string newContent, Guid userId)
    {
        var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);
        if (comment == null || comment.AuthorId != userId)
            throw new UnauthorizedAccessException("Cannot update this comment");

        comment.UpdateContent(newContent);
        await _unitOfWork.SaveChangesAsync();

        return await MapCommentToVMAsync(comment, userId);
    }

    public async Task<bool> DeleteCommentAsync(Guid commentId, Guid userId)
    {
        var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);
        if (comment == null || comment.AuthorId != userId)
            return false;

        await _unitOfWork.Comments.DeleteAsync(comment);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<CommentVM> AddReplyAsync(CreateReplyRequest request)
    {
        var parentComment = await _unitOfWork.Comments.GetByIdAsync(request.ParentCommentId);
        if (parentComment == null)
            throw new ArgumentException("Parent comment not found");

        var reply = new Comment(request.Content, parentComment.EntityId, parentComment.EntityType, request.AuthorId, request.ParentCommentId);
        await _unitOfWork.Comments.AddAsync(reply);
        await _unitOfWork.SaveChangesAsync();

        // Send notification to parent comment author
        if (parentComment.AuthorId != request.AuthorId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.AuthorId);
            await _notificationService.NotifyCommentReceivedAsync(parentComment.AuthorId, "your comment", user?.FullName ?? "Someone");
        }

        return await MapCommentToVMAsync(reply, request.AuthorId);
    }

    public async Task<List<CommentVM>> GetEntityCommentsAsync(Guid entityId, EntityType entityType, int page = 1, int pageSize = 20)
    {
        var comments = await _unitOfWork.Comments.GetTopLevelCommentsAsync(entityId, entityType);
        var result = new List<CommentVM>();

        foreach (var comment in comments)
        {
            var commentVM = await MapCommentToVMAsync(comment);
            
            // Load replies
            var replies = await _unitOfWork.Comments.GetCommentRepliesAsync(comment.Id);
            foreach (var reply in replies)
            {
                commentVM.Replies.Add(await MapCommentToVMAsync(reply));
            }
            
            result.Add(commentVM);
        }

        return result;
    }

    public async Task<List<CommentVM>> GetCommentRepliesAsync(Guid parentCommentId)
    {
        var replies = await _unitOfWork.Comments.GetCommentRepliesAsync(parentCommentId);
        var result = new List<CommentVM>();

        foreach (var reply in replies)
        {
            result.Add(await MapCommentToVMAsync(reply));
        }

        return result;
    }

    public async Task<int> GetEntityCommentCountAsync(Guid entityId, EntityType entityType)
    {
        return await _unitOfWork.Comments.GetEntityCommentCountAsync(entityId, entityType);
    }

    #endregion

    #region Shares

    public async Task<ShareResultVM> ShareEntityAsync(ShareEntityRequest request)
    {
        try
        {
            var share = new Share(request.EntityId, request.EntityType, request.UserId, request.ShareType, request.ShareMessage, request.Platform);
            await _unitOfWork.Shares.AddAsync(share);
            await _unitOfWork.SaveChangesAsync();

            var summary = await GetShareSummaryAsync(request.EntityId, request.EntityType);
            
            return new ShareResultVM
            {
                Success = true,
                Message = "Content shared successfully",
                ShareUrl = share.ShareUrl,
                Summary = summary
            };
        }
        catch (Exception ex)
        {
            return new ShareResultVM
            {
                Success = false,
                Message = $"Failed to share content: {ex.Message}"
            };
        }
    }

    public async Task<ShareSummaryVM> GetShareSummaryAsync(Guid entityId, EntityType entityType)
    {
        var shareTypeCounts = await _unitOfWork.Shares.GetShareTypeCountsAsync(entityId, entityType);
        var totalShares = shareTypeCounts.Values.Sum();
        var shareUrl = await GenerateShareUrlAsync(entityId, entityType);

        return new ShareSummaryVM
        {
            TotalShares = totalShares,
            ShareTypeCounts = shareTypeCounts,
            ShareUrl = shareUrl
        };
    }

    public async Task<List<ShareVM>> GetEntitySharesAsync(Guid entityId, EntityType entityType)
    {
        var shares = await _unitOfWork.Shares.GetEntitySharesAsync(entityId, entityType);
        var result = new List<ShareVM>();

        foreach (var share in shares)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(share.UserId);
            result.Add(new ShareVM
            {
                Id = share.Id,
                UserId = share.UserId,
                UserName = user?.FullName ?? "Unknown User",
                UserAvatar = user?.ProfilePictureUrl,
                ShareType = share.ShareType,
                ShareTypeDisplay = GetShareTypeDisplay(share.ShareType),
                ShareMessage = share.ShareMessage,
                Platform = share.Platform,
                CreatedAt = share.CreatedAt,
                TimeAgo = GetTimeAgo(share.CreatedAt)
            });
        }

        return result;
    }

    public async Task<string> GenerateShareUrlAsync(Guid entityId, EntityType entityType)
    {
        var baseUrl = "https://localhost:5000"; // TODO: Get from configuration
        return entityType switch
        {
            EntityType.Post => $"{baseUrl}/posts/{entityId}",
            EntityType.Question => $"{baseUrl}/qa/{entityId}",
            EntityType.Answer => $"{baseUrl}/qa/{entityId}#answer",
            EntityType.Story => $"{baseUrl}/feed#story-{entityId}",
            EntityType.Review => $"{baseUrl}/reviews/{entityId}",
            EntityType.Event => $"{baseUrl}/events/{entityId}",
            EntityType.Guide => $"{baseUrl}/guides/{entityId}",
            EntityType.Group => $"{baseUrl}/groups/{entityId}",
            _ => $"{baseUrl}/{entityType.ToString().ToLower()}/{entityId}"
        };
    }

    public async Task<ShareMetadataVM> GetShareMetadataAsync(Guid entityId, EntityType entityType)
    {
        var url = await GenerateShareUrlAsync(entityId, entityType);
        var title = await GetEntityTitleAsync(entityId, entityType);
        var description = await GetEntityDescriptionAsync(entityId, entityType);
        var imageUrl = await GetEntityImageUrlAsync(entityId, entityType);

        var socialMediaUrls = new Dictionary<string, string>
        {
            ["facebook"] = $"https://www.facebook.com/sharer/sharer.php?u={Uri.EscapeDataString(url)}",
            ["twitter"] = $"https://twitter.com/intent/tweet?url={Uri.EscapeDataString(url)}&text={Uri.EscapeDataString(title)}",
            ["linkedin"] = $"https://www.linkedin.com/sharing/share-offsite/?url={Uri.EscapeDataString(url)}",
            ["whatsapp"] = $"https://wa.me/?text={Uri.EscapeDataString($"{title} {url}")}",
            ["telegram"] = $"https://t.me/share/url?url={Uri.EscapeDataString(url)}&text={Uri.EscapeDataString(title)}"
        };

        return new ShareMetadataVM
        {
            Title = title,
            Description = description,
            ImageUrl = imageUrl,
            Url = url,
            Type = entityType.ToString(),
            SocialMediaUrls = socialMediaUrls
        };
    }

    #endregion

    #region Combined Summary

    public async Task<InteractionSummaryVM> GetInteractionSummaryAsync(Guid entityId, EntityType entityType, Guid? userId = null)
    {
        var reactions = await GetReactionSummaryAsync(entityId, entityType, userId);
        var commentCount = await GetEntityCommentCountAsync(entityId, entityType);
        var shares = await GetShareSummaryAsync(entityId, entityType);

        return new InteractionSummaryVM
        {
            Reactions = reactions,
            CommentCount = commentCount,
            Shares = shares,
            CanComment = userId.HasValue,
            CanShare = userId.HasValue,
            CanReact = userId.HasValue
        };
    }

    #endregion

    #region Private Helper Methods

    private async Task<CommentVM> MapCommentToVMAsync(Comment comment, Guid? currentUserId = null)
    {
        var author = await _unitOfWork.Users.GetByIdAsync(comment.AuthorId);
        var reactions = await GetReactionSummaryAsync(comment.Id, EntityType.Comment, currentUserId);

        return new CommentVM
        {
            Id = comment.Id,
            Content = comment.Content,
            AuthorId = comment.AuthorId,
            AuthorName = author?.FullName ?? "Unknown User",
            AuthorAvatar = author?.ProfilePictureUrl,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            TimeAgo = GetTimeAgo(comment.CreatedAt),
            IsEdited = comment.UpdatedAt.HasValue,
            CanEdit = currentUserId == comment.AuthorId,
            CanDelete = currentUserId == comment.AuthorId,
            ParentCommentId = comment.ParentCommentId,
            Reactions = reactions
        };
    }

    private async Task SendReactionNotificationAsync(Guid entityId, EntityType entityType, Guid userId, ReactionType reactionType)
    {
        var entityOwnerId = await GetEntityOwnerIdAsync(entityId, entityType);
        if (entityOwnerId.HasValue && entityOwnerId != userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            var entityTitle = await GetEntityTitleAsync(entityId, entityType);
            var reactionDisplay = GetReactionDisplay(reactionType);
            
            await _notificationService.SendToUserAsync(
                entityOwnerId.Value,
                $"New {reactionDisplay} Reaction",
                $"{user?.FullName ?? "Someone"} reacted with {reactionDisplay} to your {entityType.ToString().ToLower()}",
                NotificationType.VoteReceived
            );
        }
    }

    private async Task SendCommentNotificationAsync(Guid entityId, EntityType entityType, Guid userId)
    {
        var entityOwnerId = await GetEntityOwnerIdAsync(entityId, entityType);
        if (entityOwnerId.HasValue && entityOwnerId != userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            var entityTitle = await GetEntityTitleAsync(entityId, entityType);
            
            await _notificationService.NotifyCommentReceivedAsync(
                entityOwnerId.Value,
                entityTitle,
                user?.FullName ?? "Someone"
            );
        }
    }

    private async Task<Guid?> GetEntityOwnerIdAsync(Guid entityId, EntityType entityType)
    {
        return entityType switch
        {
            EntityType.Question => (await _unitOfWork.QA.GetQuestionByIdAsync(entityId))?.AuthorId,
            EntityType.Answer => (await _unitOfWork.QA.GetAnswerByIdAsync(entityId))?.AuthorId,
            EntityType.Story => (await _unitOfWork.Stories.GetByIdAsync(entityId))?.AuthorId,
            EntityType.Review => (await _unitOfWork.Reviews.GetByIdAsync(entityId))?.ReviewerId,
            // Add other entity types as needed
            _ => null
        };
    }

    private async Task<string> GetEntityTitleAsync(Guid entityId, EntityType entityType)
    {
        return entityType switch
        {
            EntityType.Question => (await _unitOfWork.QA.GetQuestionByIdAsync(entityId))?.Title ?? "Question",
            EntityType.Story => (await _unitOfWork.Stories.GetByIdAsync(entityId))?.Caption ?? "Story",
            EntityType.Review => (await _unitOfWork.Reviews.GetByIdAsync(entityId))?.Title ?? "Review",
            _ => entityType.ToString()
        };
    }

    private async Task<string> GetEntityDescriptionAsync(Guid entityId, EntityType entityType)
    {
        return entityType switch
        {
            EntityType.Question => (await _unitOfWork.QA.GetQuestionByIdAsync(entityId))?.Body ?? "",
            EntityType.Answer => (await _unitOfWork.QA.GetAnswerByIdAsync(entityId))?.Body ?? "",
            EntityType.Review => (await _unitOfWork.Reviews.GetByIdAsync(entityId))?.Comment ?? "",
            _ => ""
        };
    }

    private async Task<string?> GetEntityImageUrlAsync(Guid entityId, EntityType entityType)
    {
        // Return appropriate image URL based on entity type
        return null; // TODO: Implement based on entity types
    }

    private static string GetReactionDisplay(ReactionType reactionType)
    {
        return reactionType switch
        {
            ReactionType.Like => "Like",
            ReactionType.Love => "Love",
            ReactionType.Haha => "Haha",
            ReactionType.Wow => "Wow",
            ReactionType.Sad => "Sad",
            ReactionType.Angry => "Angry",
            _ => reactionType.ToString()
        };
    }

    private static string GetReactionIcon(ReactionType reactionType)
    {
        return reactionType switch
        {
            ReactionType.Like => "thumbs-up",
            ReactionType.Love => "heart",
            ReactionType.Haha => "laugh",
            ReactionType.Wow => "zap",
            ReactionType.Sad => "frown",
            ReactionType.Angry => "angry",
            _ => "thumbs-up"
        };
    }

    private static string GetReactionColor(ReactionType reactionType)
    {
        return reactionType switch
        {
            ReactionType.Like => "text-blue-500",
            ReactionType.Love => "text-red-500",
            ReactionType.Haha => "text-yellow-500",
            ReactionType.Wow => "text-purple-500",
            ReactionType.Sad => "text-gray-500",
            ReactionType.Angry => "text-orange-500",
            _ => "text-gray-500"
        };
    }

    private static string GetShareTypeDisplay(ShareType shareType)
    {
        return shareType switch
        {
            ShareType.Internal => "Shared internally",
            ShareType.External => "Shared externally",
            ShareType.DirectLink => "Link copied",
            ShareType.Email => "Shared via email",
            ShareType.SocialMedia => "Shared on social media",
            _ => shareType.ToString()
        };
    }

    private static string GetTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;
        
        if (timeSpan.TotalMinutes < 1)
            return "Just now";
        if (timeSpan.TotalMinutes < 60)
            return $"{(int)timeSpan.TotalMinutes}m ago";
        if (timeSpan.TotalHours < 24)
            return $"{(int)timeSpan.TotalHours}h ago";
        if (timeSpan.TotalDays < 7)
            return $"{(int)timeSpan.TotalDays}d ago";
        if (timeSpan.TotalDays < 30)
            return $"{(int)(timeSpan.TotalDays / 7)}w ago";
        if (timeSpan.TotalDays < 365)
            return $"{(int)(timeSpan.TotalDays / 30)}mo ago";
        
        return $"{(int)(timeSpan.TotalDays / 365)}y ago";
    }

    #endregion
}