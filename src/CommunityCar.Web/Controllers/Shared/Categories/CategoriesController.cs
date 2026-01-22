using Microsoft.AspNetCore.Mvc;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Web.Controllers.Shared.Categories;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly IInteractionService _interactionService;

    public CategoriesController(IInteractionService interactionService)
    {
        _interactionService = interactionService;
    }

    [HttpGet("{entityType}/{entityId}/summary")]
    public async Task<IActionResult> GetInteractionSummary(EntityType entityType, Guid entityId)
    {
        try
        {
            var summary = await _interactionService.GetInteractionSummaryAsync(entityId, entityType);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}