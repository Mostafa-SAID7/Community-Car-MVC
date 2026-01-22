using Microsoft.AspNetCore.Mvc;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Interactions.DTOs;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Web.Controllers.Shared.Reactions;

[Route("api/[controller]")]
[ApiController]
public class ReactionsController : ControllerBase
{
    private readonly IInteractionService _interactionService;
    private readonly ICurrentUserService _currentUserService;

    public ReactionsController(
        IInteractionService interactionService,
        ICurrentUserService currentUserService)
    {
        _interactionService = interactionService;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    public async Task<IActionResult> AddReaction([FromBody] ReactionRequest request)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized();

            var result = await _interactionService.AddReactionAsync(
                request.EntityId, 
                request.EntityType, 
                userId,
                request.ReactionType);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{entityType}/{entityId}")]
    public async Task<IActionResult> GetReactions(EntityType entityType, Guid entityId)
    {
        try
        {
            var reactions = await _interactionService.GetEntityReactionsAsync(entityId, entityType);
            return Ok(reactions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpDelete("{entityId}")]
    public async Task<IActionResult> RemoveReaction(Guid entityId, [FromQuery] EntityType entityType)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized();

            var result = await _interactionService.RemoveReactionAsync(entityId, entityType, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}