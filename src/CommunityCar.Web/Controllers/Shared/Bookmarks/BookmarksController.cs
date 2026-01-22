using Microsoft.AspNetCore.Mvc;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Web.Controllers.Shared.Bookmarks;

[Route("api/[controller]")]
[ApiController]
public class BookmarksController : ControllerBase
{
    private readonly IInteractionService _interactionService;
    private readonly ICurrentUserService _currentUserService;

    public BookmarksController(
        IInteractionService interactionService,
        ICurrentUserService currentUserService)
    {
        _interactionService = interactionService;
        _currentUserService = currentUserService;
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
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}