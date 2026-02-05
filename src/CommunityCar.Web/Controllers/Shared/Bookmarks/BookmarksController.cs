using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community.Interactions;
using CommunityCar.Web.Areas.Identity.Interfaces.Services.Core;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Domain.Enums.Shared;
using CommunityCar.Domain.Entities.Shared;

namespace CommunityCar.Web.Controllers.Shared.Bookmarks;

public class BookmarksController : Controller
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle([FromForm] string entityId, [FromForm] string entityType)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Json(new { success = false, message = "User must be authenticated" });

            if (!Guid.TryParse(entityId, out var parsedEntityId))
                return Json(new { success = false, message = "Invalid entity ID" });

            if (!Enum.TryParse<EntityType>(entityType, out var parsedEntityType))
                return Json(new { success = false, message = "Invalid entity type" });

            // Check if bookmark already exists
            var existingBookmark = await _bookmarkRepository.GetUserBookmarkAsync(parsedEntityId, parsedEntityType, userId);
            
            if (existingBookmark != null)
            {
                // Remove bookmark
                await _bookmarkRepository.DeleteAsync(existingBookmark);
                return Json(new { 
                    success = true, 
                    data = new { 
                        isBookmarked = false 
                    },
                    message = "Bookmark removed" 
                });
            }
            else
            {
                // Add bookmark
                var bookmark = new Bookmark(parsedEntityId, parsedEntityType, userId, null);
                await _bookmarkRepository.AddAsync(bookmark);
                return Json(new { 
                    success = true, 
                    data = new { 
                        isBookmarked = true 
                    },
                    message = "Content bookmarked!" 
                });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Check(string entityId, string entityType)
    {
        try
        {
            if (!Guid.TryParse(entityId, out var parsedEntityId))
                return Json(new { success = false, message = "Invalid entity ID" });

            if (!Enum.TryParse<EntityType>(entityType, out var parsedEntityType))
                return Json(new { success = false, message = "Invalid entity type" });

            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Json(new { isBookmarked = false });

            var bookmark = await _bookmarkRepository.GetUserBookmarkAsync(parsedEntityId, parsedEntityType, userId);
            return Json(new { isBookmarked = bookmark != null, bookmarkId = bookmark?.Id });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}



