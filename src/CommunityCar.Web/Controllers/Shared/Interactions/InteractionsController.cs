using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Web.Controllers.Shared.Interactions;

[Route("shared/interactions")]
[Controller]
public class InteractionsController : Controller
{
    private readonly IInteractionService _interactionService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<InteractionsController> _logger;

    public InteractionsController(
        IInteractionService interactionService, 
        ICurrentUserService currentUserService,
        ILogger<InteractionsController> logger)
    {
        _interactionService = interactionService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [HttpGet("{entityType}/{entityId}/summary")]
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
            _logger.LogError(ex, "Error getting interaction summary for {EntityType} {EntityId}", entityType, entityId);
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{entityType}/{entityId}/all")]
    public async Task<IActionResult> GetAllInteractions(EntityType entityType, Guid entityId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var comments = await _interactionService.GetEntityCommentsAsync(entityId, entityType, page, pageSize);
            var reactions = await _interactionService.GetEntityReactionsAsync(entityId, entityType);
            var shares = await _interactionService.GetEntitySharesAsync(entityId, entityType);

            return Ok(new
            {
                comments,
                reactions,
                shares
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all interactions for {EntityType} {EntityId}", entityType, entityId);
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("user/summary")]
    [Authorize]
    public async Task<IActionResult> GetUserInteractionSummary()
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            // This would require additional methods in the service
            return Ok(new { message = "User interaction summary endpoint - implementation needed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user interaction summary");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("stats/{entityType}")]
    public async Task<IActionResult> GetInteractionStats(EntityType entityType, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var start = startDate ?? DateTime.UtcNow.AddDays(-30);
            var end = endDate ?? DateTime.UtcNow;

            // This would require additional methods in the service for statistics
            return Ok(new { message = "Interaction statistics endpoint - implementation needed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting interaction stats for {EntityType}", entityType);
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}