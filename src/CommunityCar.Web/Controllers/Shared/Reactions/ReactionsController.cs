using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Application.Features.Interactions.DTOs;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Web.Controllers.Shared.Reactions;

[Route("api/shared/reactions")]
[ApiController]
public class ReactionsController : ControllerBase
{
    private readonly IInteractionService _interactionService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IReactionRepository _reactionRepository;

    public ReactionsController(
        IInteractionService interactionService,
        ICurrentUserService currentUserService,
        IReactionRepository reactionRepository)
    {
        _interactionService = interactionService;
        _currentUserService = currentUserService;
        _reactionRepository = reactionRepository;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddReaction([FromBody] ReactionRequest request)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

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

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateReaction([FromBody] ReactionRequest request)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            var result = await _interactionService.UpdateReactionAsync(
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

    [HttpGet("{entityType}/{entityId}/summary")]
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

    [HttpGet("{entityType}/{entityId}/check")]
    [Authorize]
    public async Task<IActionResult> CheckUserReaction(EntityType entityType, Guid entityId)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            var reaction = await _reactionRepository.GetUserReactionAsync(entityId, entityType, userId);
            return Ok(new { 
                hasReacted = reaction != null, 
                reactionType = reaction?.Type,
                reactionId = reaction?.Id 
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpDelete("{entityId}")]
    [Authorize]
    public async Task<IActionResult> RemoveReaction(Guid entityId, [FromQuery] EntityType entityType)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            var result = await _interactionService.RemoveReactionAsync(entityId, entityType, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("user")]
    [Authorize]
    public async Task<IActionResult> GetUserReactions([FromQuery] EntityType? entityType = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            var reactions = await _reactionRepository.GetUserReactionsAsync(userId, entityType);
            return Ok(reactions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("types")]
    public async Task<IActionResult> GetReactionTypes()
    {
        try
        {
            var reactionTypes = Enum.GetValues<ReactionType>()
                .Select(rt => new { 
                    type = rt, 
                    name = rt.ToString(),
                    emoji = GetReactionEmoji(rt)
                });
            
            return Ok(reactionTypes);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    private static string GetReactionEmoji(ReactionType reactionType)
    {
        return reactionType switch
        {
            ReactionType.Like => "👍",
            ReactionType.Love => "❤️",
            ReactionType.Haha => "😂",
            ReactionType.Wow => "😮",
            ReactionType.Sad => "😢",
            ReactionType.Angry => "😡",
            _ => "👍"
        };
    }
}