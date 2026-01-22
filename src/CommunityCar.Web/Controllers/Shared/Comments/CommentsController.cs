using Microsoft.AspNetCore.Mvc;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Interactions.DTOs;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Web.Controllers.Shared.Comments;

[Route("api/[controller]")]
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly IInteractionService _interactionService;
    private readonly ICurrentUserService _currentUserService;

    public CommentsController(
        IInteractionService interactionService,
        ICurrentUserService currentUserService)
    {
        _interactionService = interactionService;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    public async Task<IActionResult> AddComment([FromBody] CreateCommentRequest request)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized();

            request.AuthorId = userId;
            var comment = await _interactionService.AddCommentAsync(request);
            return Ok(new { success = true, comment });
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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComment(Guid id, [FromBody] UpdateCommentContentRequest request)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized();

            var comment = await _interactionService.UpdateCommentAsync(id, request.Content, userId);
            return Ok(new { success = true, comment });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(Guid id)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized();

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
}