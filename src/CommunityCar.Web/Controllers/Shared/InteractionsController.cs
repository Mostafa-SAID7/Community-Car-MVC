using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Features.Interactions.DTOs;
using CommunityCar.Domain.Enums;
using CommunityCar.Application.Common.Interfaces.Services.Identity;

namespace CommunityCar.Web.Controllers.Shared;

// [Authorize] - Temporarily disabled for testing
public class InteractionsController : Controller
{
    private readonly IInteractionService _interactionService;
    private readonly ICurrentUserService _currentUserService;

    public InteractionsController(IInteractionService interactionService, ICurrentUserService currentUserService)
    {
        _interactionService = interactionService;
        _currentUserService = currentUserService;
    }

    private Guid? GetCurrentUserId()
    {
        var userIdString = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            return null;
        return userId;
    }

    #region Reactions

    [HttpPost]
    public async Task<IActionResult> AddReaction(Guid entityId, EntityType entityType, ReactionType reactionType)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var result = await _interactionService.AddReactionAsync(entityId, entityType, userId.Value, reactionType);
        
        if (result.Success)
        {
            return Json(new { success = true, message = result.Message, summary = result.Summary });
        }
        
        return Json(new { success = false, message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveReaction(Guid entityId, EntityType entityType)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var result = await _interactionService.RemoveReactionAsync(entityId, entityType, userId.Value);
        
        if (result.Success)
        {
            return Json(new { success = true, message = result.Message, summary = result.Summary });
        }
        
        return Json(new { success = false, message = result.Message });
    }

    [HttpGet]
    public async Task<IActionResult> GetReactionSummary(Guid entityId, EntityType entityType)
    {
        var userId = GetCurrentUserId();
        var summary = await _interactionService.GetReactionSummaryAsync(entityId, entityType, userId);
        return Json(summary);
    }

    [HttpGet]
    public async Task<IActionResult> GetEntityReactions(Guid entityId, EntityType entityType)
    {
        var reactions = await _interactionService.GetEntityReactionsAsync(entityId, entityType);
        return Json(reactions);
    }

    #endregion

    #region Comments

    [HttpPost]
    public async Task<IActionResult> AddComment([FromBody] CreateCommentRequest request)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        request.AuthorId = userId.Value;
        var comment = await _interactionService.AddCommentAsync(request);
        return Json(new { success = true, comment });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateComment(Guid commentId, [FromBody] UpdateCommentRequest request)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        try
        {
            var comment = await _interactionService.UpdateCommentAsync(commentId, request.Content, userId.Value);
            return Json(new { success = true, comment });
        }
        catch (UnauthorizedAccessException)
        {
            return Json(new { success = false, message = "You can only edit your own comments" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteComment(Guid commentId)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var success = await _interactionService.DeleteCommentAsync(commentId, userId.Value);
        return Json(new { success, message = success ? "Comment deleted successfully" : "Failed to delete comment" });
    }

    [HttpPost]
    public async Task<IActionResult> AddReply([FromBody] CreateReplyRequest request)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        request.AuthorId = userId.Value;
        var reply = await _interactionService.AddReplyAsync(request);
        return Json(new { success = true, reply });
    }

    [HttpGet]
    public async Task<IActionResult> GetEntityComments(Guid entityId, EntityType entityType, int page = 1, int pageSize = 20)
    {
        var comments = await _interactionService.GetEntityCommentsAsync(entityId, entityType, page, pageSize);
        return Json(comments);
    }

    [HttpGet]
    public async Task<IActionResult> GetCommentReplies(Guid parentCommentId)
    {
        var replies = await _interactionService.GetCommentRepliesAsync(parentCommentId);
        return Json(replies);
    }

    #endregion

    #region Shares

    [HttpPost]
    public async Task<IActionResult> ShareEntity([FromBody] ShareEntityRequest request)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        request.UserId = userId.Value;
        var result = await _interactionService.ShareEntityAsync(request);
        
        if (result.Success)
        {
            return Json(new { success = true, message = result.Message, shareUrl = result.ShareUrl, summary = result.Summary });
        }
        
        return Json(new { success = false, message = result.Message });
    }

    [HttpGet]
    public async Task<IActionResult> GetShareSummary(Guid entityId, EntityType entityType)
    {
        var summary = await _interactionService.GetShareSummaryAsync(entityId, entityType);
        return Json(summary);
    }

    [HttpGet]
    public async Task<IActionResult> GetEntityShares(Guid entityId, EntityType entityType)
    {
        var shares = await _interactionService.GetEntitySharesAsync(entityId, entityType);
        return Json(shares);
    }

    [HttpGet]
    public async Task<IActionResult> GetShareMetadata(Guid entityId, EntityType entityType)
    {
        var metadata = await _interactionService.GetShareMetadataAsync(entityId, entityType);
        return Json(metadata);
    }

    #endregion

    #region Combined Summary

    [HttpGet]
    public async Task<IActionResult> GetInteractionSummary(Guid entityId, EntityType entityType)
    {
        var userId = GetCurrentUserId();
        var summary = await _interactionService.GetInteractionSummaryAsync(entityId, entityType, userId);
        return Json(summary);
    }

    #endregion
}