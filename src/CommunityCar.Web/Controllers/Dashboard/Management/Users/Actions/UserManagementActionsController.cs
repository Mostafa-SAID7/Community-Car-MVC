using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Management.Users.Actions;
using CommunityCar.Application.Features.Dashboard.Management.Users.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Management.Users.Actions;

[Route("{culture=en-US}/dashboard/management/users/actions")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class UserManagementActionsController : Controller
{
    private readonly IUserManagementActionsService _userActionsService;
    private readonly ILogger<UserManagementActionsController> _logger;

    public UserManagementActionsController(
        IUserManagementActionsService userActionsService,
        ILogger<UserManagementActionsController> logger)
    {
        _userActionsService = userActionsService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var history = await _userActionsService.GetUserActionHistoryAsync(page: 1, pageSize: 50);
            return View(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user actions");
            TempData["ErrorMessage"] = "Failed to load user actions. Please try again.";
            return View(new List<UserActionHistoryVM>());
        }
    }

    [HttpGet("bulk")]
    public IActionResult BulkActions()
    {
        return View(new BulkUserActionVM());
    }

    [HttpPost("bulk")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExecuteBulkAction(BulkUserActionVM model)
    {
        if (!ModelState.IsValid)
        {
            return View("BulkActions", model);
        }

        try
        {
            var result = await _userActionsService.ExecuteBulkActionAsync(model);
            if (result)
            {
                TempData["SuccessMessage"] = $"Bulk action '{model.ActionType}' executed successfully on {model.UserIds?.Count ?? 0} users.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Failed to execute bulk action.");
            return View("BulkActions", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing bulk action: {ActionType}", model.ActionType);
            ModelState.AddModelError("", "Failed to execute bulk action. Please try again.");
            return View("BulkActions", model);
        }
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetActionHistory(string? userId = null, int page = 1, int pageSize = 20)
    {
        try
        {
            var history = await _userActionsService.GetUserActionHistoryAsync(userId, page, pageSize);
            return Json(new { success = true, data = history });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading action history");
            return Json(new { success = false, message = "Failed to load action history" });
        }
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetActionStats(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var stats = await _userActionsService.GetActionStatsAsync(startDate, endDate);
            return Json(new { success = true, data = stats });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading action stats");
            return Json(new { success = false, message = "Failed to load action stats" });
        }
    }

    [HttpGet("available-actions")]
    public async Task<IActionResult> GetAvailableActions()
    {
        try
        {
            var actions = await _userActionsService.GetAvailableActionsAsync();
            return Json(new { success = true, data = actions });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading available actions");
            return Json(new { success = false, message = "Failed to load available actions" });
        }
    }
}