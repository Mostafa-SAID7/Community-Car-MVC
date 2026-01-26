using Microsoft.AspNetCore.Mvc;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Domain.Enums;
using CommunityCar.Domain.Entities.Shared;

namespace CommunityCar.Web.Controllers.Shared.Views;

public class ViewsController : Controller
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Track([FromForm] string entityId, [FromForm] string entityType)
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

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            var view = new View(parsedEntityId, parsedEntityType, ipAddress, userAgent, userId);
            await _viewRepository.AddAsync(view);
            
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetCount(string entityId, string entityType)
    {
        try
        {
            if (!Guid.TryParse(entityId, out var parsedEntityId))
                return Json(new { success = false, message = "Invalid entity ID" });

            if (!Enum.TryParse<EntityType>(entityType, out var parsedEntityType))
                return Json(new { success = false, message = "Invalid entity type" });

            var count = await _viewRepository.GetEntityViewCountAsync(parsedEntityId, parsedEntityType);
            return Json(new { count });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}