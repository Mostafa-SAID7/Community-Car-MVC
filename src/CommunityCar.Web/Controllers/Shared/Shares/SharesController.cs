using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Application.Features.Shared.Interactions.DTOs;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Web.Controllers.Shared.Shares;

public class SharesController : Controller
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Track(string entityId, string entityType, string platform = "native")
    {
        try
        {
            if (!Guid.TryParse(entityId, out var parsedEntityId))
                return Json(new { success = false, message = "Invalid entity ID" });

            if (!Enum.TryParse<EntityType>(entityType, out var parsedEntityType))
                return Json(new { success = false, message = "Invalid entity type" });

            Guid? userId = null;
            if (Guid.TryParse(_currentUserService.UserId, out var parsedUserId))
                userId = parsedUserId;

            var request = new ShareEntityRequest
            {
                EntityId = parsedEntityId,
                EntityType = parsedEntityType,
                UserId = userId ?? Guid.Empty,
                Platform = platform,
                ShareType = ShareType.External
            };

            var result = await _interactionService.ShareEntityAsync(request);
            return Json(result);
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Share(ShareEntityRequest request)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "User must be authenticated" });
                
                return RedirectToAction("Login", "Account");
            }

            request.UserId = userId;
            var result = await _interactionService.ShareEntityAsync(request);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(result);

            TempData["SuccessMessage"] = "Content shared successfully";
            return Redirect(Request.Headers["Referer"].ToString());
        }
        catch (Exception ex)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = false, message = ex.Message });

            TempData["ErrorMessage"] = ex.Message;
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetSummary(EntityType entityType, Guid entityId)
    {
        try
        {
            var summary = await _interactionService.GetShareSummaryAsync(entityId, entityType);
            return Json(summary);
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetMetadata(EntityType entityType, Guid entityId)
    {
        try
        {
            var metadata = await _interactionService.GetShareMetadataAsync(entityId, entityType);
            return Json(metadata);
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}


