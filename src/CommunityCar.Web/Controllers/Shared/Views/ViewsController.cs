using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Domain.Enums;
using CommunityCar.Domain.Entities.Shared;

namespace CommunityCar.Web.Controllers.Shared.Views;

[Route("api/shared/views")]
[ApiController]
public class ViewsController : ControllerBase
{
    private readonly IInteractionService _interactionService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IViewRepository _viewRepository;

    public ViewsController(
        IInteractionService interactionService,
        ICurrentUserService currentUserService,
        IViewRepository viewRepository)
    {
        _interactionService = interactionService;
        _currentUserService = currentUserService;
        _viewRepository = viewRepository;
    }

    [HttpPost]
    public async Task<IActionResult> RecordView([FromBody] CreateViewRequest request)
    {
        try
        {
            Guid? userId = null;
            if (Guid.TryParse(_currentUserService.UserId, out var parsedUserId))
                userId = parsedUserId;

            var view = new View(request.EntityId, request.EntityType, request.IpAddress ?? "", request.UserAgent ?? "", userId);
            await _viewRepository.AddAsync(view);
            
            return Ok(new { success = true, view });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{entityType}/{entityId}/count")]
    public async Task<IActionResult> GetViewCount(EntityType entityType, Guid entityId)
    {
        try
        {
            var count = await _viewRepository.GetEntityViewCountAsync(entityId, entityType);
            return Ok(new { count });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{entityType}/{entityId}/unique-count")]
    public async Task<IActionResult> GetUniqueViewCount(EntityType entityType, Guid entityId)
    {
        try
        {
            var count = await _viewRepository.GetUniqueViewCountAsync(entityId, entityType);
            return Ok(new { uniqueCount = count });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{entityType}/{entityId}/stats")]
    public async Task<IActionResult> GetViewStats(EntityType entityType, Guid entityId, [FromQuery] int days = 30)
    {
        try
        {
            var startDate = DateTime.UtcNow.AddDays(-days);
            var stats = await _viewRepository.GetViewStatsAsync(entityId, entityType, startDate);
            
            return Ok(stats);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{entityType}/{entityId}/recent")]
    public async Task<IActionResult> GetRecentViews(EntityType entityType, Guid entityId, [FromQuery] int count = 10)
    {
        try
        {
            var views = await _viewRepository.GetRecentViewsAsync(entityId, entityType, count);
            return Ok(views);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("user")]
    [Authorize]
    public async Task<IActionResult> GetUserViews([FromQuery] EntityType? entityType = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            var views = await _viewRepository.GetUserViewsAsync(userId, entityType);
            return Ok(views);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("popular/{entityType}")]
    public async Task<IActionResult> GetPopularContent(EntityType entityType, [FromQuery] int days = 7, [FromQuery] int count = 10)
    {
        try
        {
            var startDate = DateTime.UtcNow.AddDays(-days);
            var popular = await _viewRepository.GetMostViewedAsync(entityType, startDate, count);
            
            return Ok(popular);
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
            var url = await _interactionService.GenerateShareUrlAsync(entityId, entityType);
            return Ok(new { shareUrl = url });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}

public class CreateViewRequest
{
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}