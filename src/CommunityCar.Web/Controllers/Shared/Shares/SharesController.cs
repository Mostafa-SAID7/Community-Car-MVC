using Microsoft.AspNetCore.Mvc;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Interactions.DTOs;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Web.Controllers.Shared.Shares;

[Route("api/[controller]")]
[ApiController]
public class SharesController : ControllerBase
{
    private readonly IInteractionService _interactionService;
    private readonly ICurrentUserService _currentUserService;

    public SharesController(
        IInteractionService interactionService,
        ICurrentUserService currentUserService)
    {
        _interactionService = interactionService;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    public async Task<IActionResult> ShareContent([FromBody] ShareEntityRequest request)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized();

            request.UserId = userId;
            var result = await _interactionService.ShareEntityAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{entityType}/{entityId}")]
    public async Task<IActionResult> GetShares(EntityType entityType, Guid entityId)
    {
        try
        {
            var shares = await _interactionService.GetEntitySharesAsync(entityId, entityType);
            return Ok(shares);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
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