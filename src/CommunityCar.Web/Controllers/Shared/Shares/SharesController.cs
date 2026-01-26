using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Application.Features.Interactions.DTOs;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Web.Controllers.Shared.Shares;

[Route("api/shared/shares")]
[ApiController]
public class SharesController : ControllerBase
{
    private readonly IInteractionService _interactionService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IShareRepository _shareRepository;

    public SharesController(
        IInteractionService interactionService,
        ICurrentUserService currentUserService,
        IShareRepository shareRepository)
    {
        _interactionService = interactionService;
        _currentUserService = currentUserService;
        _shareRepository = shareRepository;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ShareContent([FromBody] ShareEntityRequest request)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

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
    public async Task<IActionResult> GetShares(EntityType entityType, Guid entityId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
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

    [HttpGet("{entityType}/{entityId}/summary")]
    public async Task<IActionResult> GetShareSummary(EntityType entityType, Guid entityId)
    {
        try
        {
            var summary = await _interactionService.GetShareSummaryAsync(entityId, entityType);
            return Ok(summary);
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

    [HttpGet("{entityType}/{entityId}/url")]
    public async Task<IActionResult> GenerateShareUrl(EntityType entityType, Guid entityId)
    {
        try
        {
            var shareUrl = await _interactionService.GenerateShareUrlAsync(entityId, entityType);
            return Ok(new { shareUrl });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{entityType}/{entityId}/count")]
    public async Task<IActionResult> GetShareCount(EntityType entityType, Guid entityId)
    {
        try
        {
            var count = await _shareRepository.GetShareCountAsync(entityId, entityType);
            return Ok(new { count });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("user")]
    [Authorize]
    public async Task<IActionResult> GetUserShares([FromQuery] EntityType? entityType = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            var shares = await _shareRepository.GetUserSharesAsync(userId, entityType);
            return Ok(shares);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("platforms")]
    public async Task<IActionResult> GetSupportedPlatforms()
    {
        try
        {
            var platforms = new[]
            {
                new { name = "Facebook", icon = "fab fa-facebook", color = "#1877F2" },
                new { name = "Twitter", icon = "fab fa-twitter", color = "#1DA1F2" },
                new { name = "LinkedIn", icon = "fab fa-linkedin", color = "#0A66C2" },
                new { name = "WhatsApp", icon = "fab fa-whatsapp", color = "#25D366" },
                new { name = "Telegram", icon = "fab fa-telegram", color = "#0088CC" },
                new { name = "Email", icon = "fas fa-envelope", color = "#EA4335" },
                new { name = "Copy Link", icon = "fas fa-link", color = "#6B7280" }
            };
            
            return Ok(platforms);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteShare(Guid id)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            var share = await _shareRepository.GetByIdAsync(id);
            if (share == null)
                return NotFound(new { success = false, message = "Share not found" });

            if (share.UserId != userId)
                return Forbid("You can only delete your own shares");

            await _shareRepository.DeleteAsync(share);
            return Ok(new { success = true, message = "Share deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}