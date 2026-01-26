using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Domain.Enums;
using CommunityCar.Domain.Entities.Shared;

namespace CommunityCar.Web.Controllers.Shared.Bookmarks;

[Route("api/shared/bookmarks")]
[ApiController]
public class BookmarksController : ControllerBase
{
    private readonly IInteractionService _interactionService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IBookmarkRepository _bookmarkRepository;

    public BookmarksController(
        IInteractionService interactionService,
        ICurrentUserService currentUserService,
        IBookmarkRepository bookmarkRepository)
    {
        _interactionService = interactionService;
        _currentUserService = currentUserService;
        _bookmarkRepository = bookmarkRepository;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddBookmark([FromBody] CreateBookmarkRequest request)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            var bookmark = new Bookmark(request.EntityId, request.EntityType, userId, request.Notes);
            await _bookmarkRepository.AddAsync(bookmark);
            
            return Ok(new { success = true, bookmark });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpDelete("{entityType}/{entityId}")]
    [Authorize]
    public async Task<IActionResult> RemoveBookmark(EntityType entityType, Guid entityId)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            var bookmark = await _bookmarkRepository.GetUserBookmarkAsync(entityId, entityType, userId);
            if (bookmark == null)
                return NotFound(new { success = false, message = "Bookmark not found" });

            await _bookmarkRepository.DeleteAsync(bookmark);
            return Ok(new { success = true, message = "Bookmark removed successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("user")]
    [Authorize]
    public async Task<IActionResult> GetUserBookmarks([FromQuery] EntityType? entityType = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            var bookmarks = await _bookmarkRepository.GetUserBookmarksAsync(userId, entityType);
            
            // Apply pagination
            var totalCount = bookmarks.Count();
            var paginatedBookmarks = bookmarks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new { 
                success = true, 
                data = paginatedBookmarks,
                totalCount = totalCount,
                page = page,
                pageSize = pageSize
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{entityType}/{entityId}/check")]
    [Authorize]
    public async Task<IActionResult> CheckBookmark(EntityType entityType, Guid entityId)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            var bookmark = await _bookmarkRepository.GetUserBookmarkAsync(entityId, entityType, userId);
            return Ok(new { isBookmarked = bookmark != null, bookmarkId = bookmark?.Id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{entityType}/{entityId}/count")]
    public async Task<IActionResult> GetBookmarkCount(EntityType entityType, Guid entityId)
    {
        try
        {
            var count = await _bookmarkRepository.GetBookmarkCountAsync(entityId, entityType);
            return Ok(new { count });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
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

public class CreateBookmarkRequest
{
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public string? Notes { get; set; }
}