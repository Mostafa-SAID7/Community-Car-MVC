using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Management.Users.Core;
using CommunityCar.Application.Features.Dashboard.Management.Users.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Management.Users.Core;

[Route("{culture=en-US}/dashboard/management/users/core")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class UserManagementCoreController : Controller
{
    private readonly IUserManagementCoreService _userManagementService;
    private readonly ILogger<UserManagementCoreController> _logger;

    public UserManagementCoreController(
        IUserManagementCoreService userManagementService,
        ILogger<UserManagementCoreController> logger)
    {
        _userManagementService = userManagementService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? search = null, string? role = null, bool? isActive = null, int page = 1, int pageSize = 20)
    {
        try
        {
            var result = await _userManagementService.GetUsersAsync(search, role, isActive, page, pageSize);
            
            ViewBag.Search = search;
            ViewBag.Role = role;
            ViewBag.IsActive = isActive;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = result.TotalCount;
            
            return View(result.Users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading users");
            TempData["ErrorMessage"] = "Failed to load users. Please try again.";
            return View(new List<UserManagementVM>());
        }
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> Details(string userId)
    {
        try
        {
            var user = await _userManagementService.GetUserManagementByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user details for ID: {UserId}", userId);
            return NotFound();
        }
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View(new CreateUserVM());
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var userId = await _userManagementService.CreateUserAsync(model);
            TempData["SuccessMessage"] = "User created successfully.";
            return RedirectToAction(nameof(Details), new { userId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            ModelState.AddModelError("", "Failed to create user. Please try again.");
            return View(model);
        }
    }

    [HttpGet("edit/{userId}")]
    public async Task<IActionResult> Edit(string userId)
    {
        try
        {
            var user = await _userManagementService.GetUserManagementByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var model = new DashboardUpdateUserVM
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                IsActive = user.IsActive
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user for edit: {UserId}", userId);
            return NotFound();
        }
    }

    [HttpPost("edit/{userId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string userId, DashboardUpdateUserVM model)
    {
        if (userId != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var result = await _userManagementService.UpdateUserAsync(model);
            if (result)
            {
                TempData["SuccessMessage"] = "User updated successfully.";
                return RedirectToAction(nameof(Details), new { userId });
            }

            ModelState.AddModelError("", "Failed to update user.");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user: {UserId}", userId);
            ModelState.AddModelError("", "Failed to update user. Please try again.");
            return View(model);
        }
    }

    [HttpPost("delete/{userId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string userId)
    {
        try
        {
            var result = await _userManagementService.SoftDeleteUserAsync(userId);
            if (result)
            {
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete user.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user: {UserId}", userId);
            TempData["ErrorMessage"] = "Failed to delete user. Please try again.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost("restore/{userId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(string userId)
    {
        try
        {
            var result = await _userManagementService.RestoreUserAsync(userId);
            if (result)
            {
                TempData["SuccessMessage"] = "User restored successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to restore user.";
            }

            return RedirectToAction(nameof(Details), new { userId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring user: {UserId}", userId);
            TempData["ErrorMessage"] = "Failed to restore user. Please try again.";
            return RedirectToAction(nameof(Details), new { userId });
        }
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetUserStats()
    {
        try
        {
            var stats = await _userManagementService.GetUserStatsAsync();
            return Json(new { success = true, data = stats });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user stats");
            return Json(new { success = false, message = "Failed to load user stats" });
        }
    }
}