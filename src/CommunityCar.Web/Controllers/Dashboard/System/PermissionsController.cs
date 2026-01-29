using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Authorization;
using CommunityCar.Application.Features.Account.DTOs.Authorization;
using CommunityCar.Domain.Constants;
using CommunityCar.Web.Attributes;

namespace CommunityCar.Web.Controllers.Dashboard.System;

[Authorize]
[RequirePermission(Permissions.PermissionManagement.View)]
public class PermissionsController : Controller
{
    private readonly IPermissionService _permissionService;
    private readonly IRoleService _roleService;
    private readonly ILogger<PermissionsController> _logger;

    public PermissionsController(
        IPermissionService permissionService,
        IRoleService roleService,
        ILogger<PermissionsController> logger)
    {
        _permissionService = permissionService;
        _roleService = roleService;
        _logger = logger;
    }

    // GET: /Dashboard/System/Permissions
    public async Task<IActionResult> Index()
    {
        try
        {
            var permissions = await _permissionService.GetAllPermissionsAsync();
            var categories = await _permissionService.GetPermissionCategoriesAsync();
            
            ViewBag.Categories = categories;
            return View(permissions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading permissions");
            TempData["Error"] = "Failed to load permissions";
            return View(Enumerable.Empty<PermissionDTO>());
        }
    }

    // GET: /Dashboard/System/Permissions/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var permission = await _permissionService.GetPermissionByIdAsync(id);
            if (permission == null)
            {
                TempData["Error"] = "Permission not found";
                return RedirectToAction(nameof(Index));
            }

            var usersWithPermission = await _permissionService.GetUsersWithPermissionAsync(permission.Name);
            var rolesWithPermission = await _permissionService.GetRolesWithPermissionAsync(permission.Name);

            ViewBag.UsersWithPermission = usersWithPermission;
            ViewBag.RolesWithPermission = rolesWithPermission;

            return View(permission);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading permission details for {PermissionId}", id);
            TempData["Error"] = "Failed to load permission details";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: /Dashboard/System/Permissions/Create
    [RequirePermission(Permissions.PermissionManagement.Create)]
    public async Task<IActionResult> Create()
    {
        var categories = await _permissionService.GetPermissionCategoriesAsync();
        ViewBag.Categories = categories.Keys.ToList();
        return View();
    }

    // POST: /Dashboard/System/Permissions/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(Permissions.PermissionManagement.Create)]
    public async Task<IActionResult> Create(CreatePermissionRequest request)
    {
        if (!ModelState.IsValid)
        {
            var categories = await _permissionService.GetPermissionCategoriesAsync();
            ViewBag.Categories = categories.Keys.ToList();
            return View(request);
        }

        try
        {
            var permission = await _permissionService.CreatePermissionAsync(request);
            TempData["Success"] = $"Permission '{permission.Name}' created successfully";
            return RedirectToAction(nameof(Details), new { id = permission.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating permission {PermissionName}", request.Name);
            ModelState.AddModelError("", ex.Message);
            
            var categories = await _permissionService.GetPermissionCategoriesAsync();
            ViewBag.Categories = categories.Keys.ToList();
            return View(request);
        }
    }

    // GET: /Dashboard/System/Permissions/Edit/5
    [RequirePermission(Permissions.PermissionManagement.Edit)]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var permission = await _permissionService.GetPermissionByIdAsync(id);
            if (permission == null)
            {
                TempData["Error"] = "Permission not found";
                return RedirectToAction(nameof(Index));
            }

            if (permission.IsSystemPermission)
            {
                TempData["Warning"] = "System permissions have limited editing capabilities";
            }

            var request = new UpdatePermissionRequest
            {
                DisplayName = permission.DisplayName,
                Description = permission.Description,
                Category = permission.Category
            };

            var categories = await _permissionService.GetPermissionCategoriesAsync();
            ViewBag.Categories = categories.Keys.ToList();
            ViewBag.Permission = permission;

            return View(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading permission for edit {PermissionId}", id);
            TempData["Error"] = "Failed to load permission";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: /Dashboard/System/Permissions/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(Permissions.PermissionManagement.Edit)]
    public async Task<IActionResult> Edit(Guid id, UpdatePermissionRequest request)
    {
        if (!ModelState.IsValid)
        {
            var permission = await _permissionService.GetPermissionByIdAsync(id);
            var categories = await _permissionService.GetPermissionCategoriesAsync();
            ViewBag.Categories = categories.Keys.ToList();
            ViewBag.Permission = permission;
            return View(request);
        }

        try
        {
            var updatedPermission = await _permissionService.UpdatePermissionAsync(id, request);
            TempData["Success"] = $"Permission '{updatedPermission.Name}' updated successfully";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating permission {PermissionId}", id);
            ModelState.AddModelError("", ex.Message);
            
            var permission = await _permissionService.GetPermissionByIdAsync(id);
            var categories = await _permissionService.GetPermissionCategoriesAsync();
            ViewBag.Categories = categories.Keys.ToList();
            ViewBag.Permission = permission;
            return View(request);
        }
    }

    // POST: /Dashboard/System/Permissions/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(Permissions.PermissionManagement.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var permission = await _permissionService.GetPermissionByIdAsync(id);
            if (permission == null)
            {
                TempData["Error"] = "Permission not found";
                return RedirectToAction(nameof(Index));
            }

            var result = await _permissionService.DeletePermissionAsync(id);
            if (result)
            {
                TempData["Success"] = $"Permission '{permission.Name}' deleted successfully";
            }
            else
            {
                TempData["Error"] = "Failed to delete permission";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting permission {PermissionId}", id);
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: /Dashboard/System/Permissions/Activate/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(Permissions.PermissionManagement.Edit)]
    public async Task<IActionResult> Activate(Guid id)
    {
        try
        {
            var result = await _permissionService.ActivatePermissionAsync(id);
            if (result)
            {
                TempData["Success"] = "Permission activated successfully";
            }
            else
            {
                TempData["Error"] = "Failed to activate permission";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating permission {PermissionId}", id);
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    // POST: /Dashboard/System/Permissions/Deactivate/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(Permissions.PermissionManagement.Edit)]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        try
        {
            var result = await _permissionService.DeactivatePermissionAsync(id);
            if (result)
            {
                TempData["Success"] = "Permission deactivated successfully";
            }
            else
            {
                TempData["Error"] = "Failed to deactivate permission";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating permission {PermissionId}", id);
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    // GET: /Dashboard/System/Permissions/Categories
    public async Task<IActionResult> Categories()
    {
        try
        {
            var categories = await _permissionService.GetPermissionCategoriesAsync();
            return View(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading permission categories");
            TempData["Error"] = "Failed to load permission categories";
            return View(new Dictionary<string, List<string>>());
        }
    }

    // POST: /Dashboard/System/Permissions/InitializeSystem
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(Permissions.System.SystemConfiguration)]
    public async Task<IActionResult> InitializeSystem()
    {
        try
        {
            var result = await _permissionService.InitializeSystemPermissionsAsync();
            if (result)
            {
                TempData["Success"] = "System permissions initialized successfully";
            }
            else
            {
                TempData["Error"] = "Failed to initialize system permissions";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing system permissions");
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}