using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Management;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Dashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Management;

[Route("dashboard/management")]
public class ManagementController : Controller
{
    private readonly IManagementService _managementService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ManagementController> _logger;

    public ManagementController(
        IManagementService managementService,
        ICurrentUserService currentUserService,
        ILogger<ManagementController> logger)
    {
        _managementService = managementService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
    {
        try
        {
            var history = await _managementService.GetUserManagementHistoryAsync(page, pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            return View("~/Views/Dashboard/Management/Index.cshtml", history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user management history");
            TempData["ErrorMessage"] = "Failed to load user management history. Please try again.";
            return View();
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> UserHistory(Guid userId, int page = 1, int pageSize = 20)
    {
        try
        {
            var history = await _managementService.GetUserManagementHistoryByUserAsync(userId, page, pageSize);
            ViewBag.UserId = userId;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            return View(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user management history for user: {UserId}", userId);
            TempData["ErrorMessage"] = "Failed to load user management history.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet("action")]
    public IActionResult PerformAction()
    {
        return View();
    }

    [HttpPost("action")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PerformAction(UserManagementVM request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        try
        {
            var success = await _managementService.PerformUserActionAsync(request);
            if (success)
            {
                TempData["SuccessMessage"] = $"User action '{request.Action}' performed successfully!";
                return RedirectToAction(nameof(UserHistory), new { userId = request.UserId });
            }
            else
            {
                ModelState.AddModelError("", "Failed to perform user action. Please check the details and try again.");
                return View(request);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing user action: {Action} for user: {UserId}", request.Action, request.UserId);
            ModelState.AddModelError("", "An error occurred while performing the action. Please try again.");
            return View(request);
        }
    }

    [HttpPost("reverse/{actionId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReverseAction(Guid actionId)
    {
        try
        {
            var success = await _managementService.ReverseUserActionAsync(actionId);
            if (success)
            {
                TempData["SuccessMessage"] = "User action reversed successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to reverse user action.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reversing user action: {ActionId}", actionId);
            TempData["ErrorMessage"] = "An error occurred while reversing the action.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet("history-data")]
    public async Task<IActionResult> GetHistory(int page = 1, int pageSize = 20)
    {
        try
        {
            var history = await _managementService.GetUserManagementHistoryAsync(page, pageSize);
            return Json(new { success = true, data = history, page, pageSize });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user management history API");
            return Json(new { success = false, message = "Failed to load history" });
        }
    }

    [HttpGet("user/{userId}/history-data")]
    public async Task<IActionResult> GetUserHistory(Guid userId, int page = 1, int pageSize = 20)
    {
        try
        {
            var history = await _managementService.GetUserManagementHistoryByUserAsync(userId, page, pageSize);
            return Json(new { success = true, data = history, userId, page, pageSize });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user history API for user: {UserId}", userId);
            return Json(new { success = false, message = "Failed to load user history" });
        }
    }

    [HttpPost("action-api")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PerformActionApi([FromBody] UserManagementVM request)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Invalid request data", errors = ModelState });
        }

        try
        {
            var success = await _managementService.PerformUserActionAsync(request);
            if (success)
            {
                return Json(new { success = true, message = $"User action '{request.Action}' performed successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to perform user action" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing user action API: {Action} for user: {UserId}", request.Action, request.UserId);
            return Json(new { success = false, message = "An error occurred while performing the action" });
        }
    }

    [HttpPost("reverse-api/{actionId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReverseActionApi(Guid actionId)
    {
        try
        {
            var success = await _managementService.ReverseUserActionAsync(actionId);
            if (success)
            {
                return Json(new { success = true, message = "User action reversed successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to reverse user action" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reversing user action API: {ActionId}", actionId);
            return Json(new { success = false, message = "An error occurred while reversing the action" });
        }
    }
}


