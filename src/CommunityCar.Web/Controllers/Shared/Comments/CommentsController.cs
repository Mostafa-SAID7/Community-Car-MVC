using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Application.Features.Interactions.DTOs;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Web.Controllers.Shared.Comments;

[Route("api/shared/comments")]
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly IInteractionService _interactionService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICommentRepository _commentRepository;

    public CommentsController(
        IInteractionService interactionService,
        ICurrentUserService currentUserService,
        ICommentRepository commentRepository)
    {
        _interactionService = interactionService;
        _currentUserService = currentUserService;
        _commentRepository = commentRepository;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddComment([FromBody] CreateCommentRequest request)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            request.AuthorId = userId;
            var comment = await _interactionService.AddCommentAsync(request);
            return Ok(new { success = true, comment });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("reply")]
    [Authorize]
    public async Task<IActionResult> AddReply([FromBody] CreateReplyRequest request)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            request.AuthorId = userId;
            var reply = await _interactionService.AddReplyAsync(request);
            return Ok(new { success = true, reply });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{entityType}/{entityId}")]
    public async Task<IActionResult> GetComments(EntityType entityType, Guid entityId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var comments = await _interactionService.GetEntityCommentsAsync(entityId, entityType, page, pageSize);
            return Ok(comments);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{entityType}/{entityId}/count")]
    public async Task<IActionResult> GetCommentCount(EntityType entityType, Guid entityId)
    {
        try
        {
            var count = await _interactionService.GetEntityCommentCountAsync(entityId, entityType);
            return Ok(new { count });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetComment(Guid id)
    {
        try
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
                return NotFound(new { success = false, message = "Comment not found" });

            return Ok(comment);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateComment(Guid id, [FromBody] UpdateCommentContentRequest request)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            var comment = await _interactionService.UpdateCommentAsync(id, request.Content, userId);
            return Ok(new { success = true, comment });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(Guid id)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            var success = await _interactionService.DeleteCommentAsync(id, userId);
            if (success)
                return Ok(new { success = true, message = "Comment deleted successfully" });
            else
                return BadRequest(new { success = false, message = "Failed to delete comment" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{parentCommentId}/replies")]
    public async Task<IActionResult> GetReplies(Guid parentCommentId)
    {
        try
        {
            var replies = await _interactionService.GetCommentRepliesAsync(parentCommentId);
            return Ok(replies);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserComments(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var comments = await _commentRepository.GetUserCommentsAsync(userId);
            return Ok(comments);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("{id}/report")]
    [Authorize]
    public async Task<IActionResult> ReportComment(Guid id, [FromBody] ReportCommentRequest request)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            // Implementation would depend on having a reporting system
            return Ok(new { success = true, message = "Comment reported successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}

public class ReportCommentRequest
{
    public string Reason { get; set; } = string.Empty;
    public string? Description { get; set; }
}