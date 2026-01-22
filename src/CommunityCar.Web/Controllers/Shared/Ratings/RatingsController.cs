using Microsoft.AspNetCore.Mvc;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Web.Controllers.Shared.Ratings;

[Route("api/[controller]")]
[ApiController]
public class RatingsController : ControllerBase
{
    private readonly IInteractionService _interactionService;

    public RatingsController(IInteractionService interactionService)
    {
        _interactionService = interactionService;
    }

    [HttpGet("{entityType}/{entityId}/comments")]
    public async Task<IActionResult> GetEntityComments(EntityType entityType, Guid entityId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
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
}