using Microsoft.AspNetCore.Mvc;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Web.Controllers.Shared.Votes;

[Route("api/[controller]")]
[ApiController]
public class VotesController : ControllerBase
{
    private readonly IInteractionService _interactionService;

    public VotesController(IInteractionService interactionService)
    {
        _interactionService = interactionService;
    }

    [HttpGet("{entityType}/{entityId}/reactions")]
    public async Task<IActionResult> GetEntityReactions(EntityType entityType, Guid entityId)
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
}