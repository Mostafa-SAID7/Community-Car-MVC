using Microsoft.AspNetCore.Mvc;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Web.Controllers.Shared;

[Route("api/[controller]")]
[ApiController]
public class InteractionSummaryController : ControllerBase
{
    private readonly IInteractionService _interactionService;
    private readonly ICurrentUserService _currentUserService;

    public InteractionSummaryController(
        IInteractionService interactionService,
        ICurrentUserService currentUserService)
    {
        _interactionService = interactionService;
        _currentUserService = currentUserService;
    }

    [HttpGet("{entityType}/{entityId}")]
    public async Task<IActionResult> GetInteractionSummary(EntityType entityType, Guid entityId)
    {
        try
        {
            Guid? userId = null;
            if (Guid.TryParse(_currentUserService.UserId, out var parsedUserId))
                userId = parsedUserId;

            var summary = await _interactionService.GetInteractionSummaryAsync(entityId, entityType, userId);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{entityType}/{entityId}/reactions")]
    public async Task<IActionResult> GetReactionSummary(EntityType entityType, Guid entityId)
    {
        try
        {
            Guid? userId = null;
            if (Guid.TryParse(_currentUserService.UserId, out var parsedUserId))
                userId = parsedUserId;

            var summary = await _interactionService.GetReactionSummaryAsync(entityId, entityType, userId);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{entityType}/{entityId}/shares")]
    public async Task<IActionResult> GetShareSummary(EntityType entityType, Guid entityId)
    {
        try
        {
            var summary = await _interactionService.GetShareSummaryAsync(entityId, entityType);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{entityType}/{entityId}/comments/count")]
    public async Task<IActionResult> GetCommentCount(EntityType entityType, Guid entityId)
    {
        try
        {
            var count = await _interactionService.GetEntityCommentCountAsync(entityId, entityType);
            return Ok(new { commentCount = count });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}