using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community.Interactions;
using CommunityCar.Web.Areas.Identity.Interfaces.Services.Core;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Web.Controllers.Shared.Reactions;

public class ReactionsController : Controller
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle([FromForm] string entityId, [FromForm] string entityType, [FromForm] string reactionType = "Like")
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Json(new { success = false, message = "User must be authenticated" });

            if (!Guid.TryParse(entityId, out var parsedEntityId))
                return Json(new { success = false, message = "Invalid entity ID" });

            if (!Enum.TryParse<EntityType>(entityType, out var parsedEntityType))
                return Json(new { success = false, message = "Invalid entity type" });

            if (!Enum.TryParse<ReactionType>(reactionType, out var parsedReactionType))
                return Json(new { success = false, message = "Invalid reaction type" });

            // Get current reaction summary to check if user already reacted
            var currentSummary = await _interactionService.GetReactionSummaryAsync(parsedEntityId, parsedEntityType, userId);
            
            if (currentSummary.HasUserReacted)
            {
                // Remove existing reaction (toggle off)
                var removeResult = await _interactionService.RemoveReactionAsync(parsedEntityId, parsedEntityType, userId);
                
                return Json(new { 
                    success = removeResult.Success, 
                    isLiked = false,
                    data = new { 
                        likeCount = removeResult.Summary?.ReactionCounts?.GetValueOrDefault(ReactionType.Like, 0) ?? 0,
                        isLikedByUser = false
                    },
                    message = removeResult.Message
                });
            }
            else
            {
                // Add new reaction
                var addResult = await _interactionService.AddReactionAsync(parsedEntityId, parsedEntityType, userId, parsedReactionType);
                
                return Json(new { 
                    success = addResult.Success, 
                    isLiked = true,
                    data = new { 
                        likeCount = addResult.Summary?.ReactionCounts?.GetValueOrDefault(ReactionType.Like, 0) ?? 1,
                        isLikedByUser = true
                    },
                    message = addResult.Message
                });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetSummary(string entityId, string entityType)
    {
        try
        {
            if (!Guid.TryParse(entityId, out var parsedEntityId))
                return Json(new { success = false, message = "Invalid entity ID" });

            if (!Enum.TryParse<EntityType>(entityType, out var parsedEntityType))
                return Json(new { success = false, message = "Invalid entity type" });

            Guid? userId = null;
            if (Guid.TryParse(_currentUserService.UserId, out var parsedUserId))
                userId = parsedUserId;

            var summary = await _interactionService.GetReactionSummaryAsync(parsedEntityId, parsedEntityType, userId);
            return Json(summary);
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}



