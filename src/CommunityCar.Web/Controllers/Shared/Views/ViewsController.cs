using Microsoft.AspNetCore.Mvc;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Web.Controllers.Shared.Views;

[Route("api/[controller]")]
[ApiController]
public class ViewsController : ControllerBase
{
    private readonly IInteractionService _interactionService;

    public ViewsController(IInteractionService interactionService)
    {
        _interactionService = interactionService;
    }

    [HttpGet("{entityType}/{entityId}/url")]
    public async Task<IActionResult> GenerateShareUrl(EntityType entityType, Guid entityId)
    {
        try
        {
            var url = await _interactionService.GenerateShareUrlAsync(entityId, entityType);
            return Ok(new { shareUrl = url });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}