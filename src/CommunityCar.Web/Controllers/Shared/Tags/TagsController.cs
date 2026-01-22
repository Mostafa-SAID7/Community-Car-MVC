using Microsoft.AspNetCore.Mvc;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Web.Controllers.Shared.Tags;

[Route("api/[controller]")]
[ApiController]
public class TagsController : ControllerBase
{
    private readonly IInteractionService _interactionService;

    public TagsController(IInteractionService interactionService)
    {
        _interactionService = interactionService;
    }

    [HttpGet("{entityType}/{entityId}/metadata")]
    public async Task<IActionResult> GetShareMetadata(EntityType entityType, Guid entityId)
    {
        try
        {
            var metadata = await _interactionService.GetShareMetadataAsync(entityId, entityType);
            return Ok(metadata);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}