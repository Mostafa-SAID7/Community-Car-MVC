using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Authorization;
using CommunityCar.Application.Features.Account.DTOs.Authorization;
using CommunityCar.Domain.Constants;
using CommunityCar.Web.Attributes;

namespace CommunityCar.Web.Controllers.Dashboard.System;

[Authorize]
[RequirePermission(Permissions.Roles.View)]
public class RolesController : Controller
{
    private readonly IRoleService _roleService;
    private readonly IPermissionService _permissionService;
    private readonly ILogger<RolesController> _logger;

    public RolesController(
        IRoleService roleService,
        IPermissionService permissionService,
        ILogger<RolesController> logger)
    {
        _roleService = roleService;
        _permissionService = permissionService;
        _logger = logger;
    }

    // GET: /Dashboard/System/Roles
    public async Task<IActionResult> Index()
    {
        try
        {
            var roles = await _roleService.GetAllRolesAsync();
            var categories = await _roleService.GetRoleCategoriesAsync();
            
            ViewBag.Categories = categories;
            return View(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading roles");
            TempData["Error"] = "Failed to load roles";
            return View(Enumerable.Empty<RoleDTO>());
        }
    }

    // GET: /Dashboard/System/Roles/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                TempData["Error"] = "Role not found";
                return RedirectToAction(nameof(Index));
            }

            var statistics = await _roleService.GetRoleStatisticsAsync(role.Name);
            var usersInRole = await _roleService.GetUsersInRoleAsync(role.Name, 1, 10);

            ViewBag.Statistics = statistics;
            ViewBag.UsersInRole = usersInRole;

            return View(role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading role details for {RoleId}", id);
            TempData["Error"] = "Failed to load role details";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: /Dashboard/System/Roles/Create
    [RequirePermission(Permissions.Roles.Create)]
    public async Task<IActionResult> Create()
    {
        var categories = await _roleService.GetRoleCategoriesAsync();
        var permissions = await _permissionService.GetAllPermissionsAsync();
        var permissionCategories = await _permissionService.GetPermissionCategoriesAsync();

        ViewBag.Categories = categories.Keys.ToList();
        ViewBag.Permissions = permissions;
        ViewBag.PermissionCategories = permissionCategories;

        return View();
    }

    // POST: /Dashboard/System/Roles/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Roles.Create)]
    public async Task<IActionResult> Create(CreateRoleRequest request)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCreateViewData();
            return View(request);
        }

        try
        {
            var role = await _roleService.CreateRoleAsync(request);
            TempData["Success"] = $"Role '{role.Name}' created successfully";
            return RedirectToAction(nameof(Details), new { id = role.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role {RoleName}", request.Name);
            ModelState.AddModelError("", ex.Message);
            
            await PopulateCreateViewData();
            return View(request);
        }
    }

    // GET: /Dashboard/System/Roles/Edit/5
    [RequirePermission(Permissions.Roles.Edit)]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                TempData["Error"] = "Role not found";
                return RedirectToAction(nameof(Index));
            }

            if (role.IsSystemRole)
            {
                TempData["Warning"] = "System roles have limited editing capabilities";
            }

            var request = new UpdateRoleRequest
            {
                Description = role.Description,
                Category = role.Category,
                Priority = role.Priority,
                Permissions = role.Permissions
            };

            await PopulateEditViewData();
            ViewBag.Role = role;

            return View(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading role for edit {RoleId}", id);
            TempData["Error"] = "Failed to load role";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: /Dashboard/System/Roles/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Roles.Edit)]
    public async Task<IActionResult> Edit(Guid id, UpdateRoleRequest request)
    {
        if (!ModelState.IsValid)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            await PopulateEditViewData();
            ViewBag.Role = role;
            return View(request);
        }

        try
        {
            var updatedRole = await _roleService.UpdateRoleAsync(id, request);
            TempData["Success"] = $"Role '{updatedRole.Name}' updated successfully";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role {RoleId}", id);
            ModelState.AddModelError("", ex.Message);
            
            var role = await _roleService.GetRoleByIdAsync(id);
            await PopulateEditViewData();
            ViewBag.Role = role;
            return View(request);
        }
    }

    // POST: /Dashboard/System/Roles/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Roles.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                TempData["Error"] = "Role not found";
                return RedirectToAction(nameof(Index));
            }

            var result = await _roleService.DeleteRoleAsync(id);
            if (result)
            {
                TempData["Success"] = $"Role '{role.Name}' deleted successfully";
            }
            else
            {
                TempData["Error"] = "Failed to delete role";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role {RoleId}", id);
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: /Dashboard/System/Roles/Activate/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Roles.Edit)]
    public async Task<IActionResult> Activate(Guid id)
    {
        try
        {
            var result = await _roleService.ActivateRoleAsync(id);
            if (result)
            {
                TempData["Success"] = "Role activated successfully";
            }
            else
            {
                TempData["Error"] = "Failed to activate role";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating role {RoleId}", id);
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    // POST: /Dashboard/System/Roles/Deactivate/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Roles.Edit)]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        try
        {
            var result = await _roleService.DeactivateRoleAsync(id);
            if (result)
            {
                TempData["Success"] = "Role deactivated successfully";
            }
            else
            {
                TempData["Error"] = "Failed to deactivate role";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating role {RoleId}", id);
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    // GET: /Dashboard/System/Roles/Hierarchy
    public async Task<IActionResult> Hierarchy()
    {
        try
        {
            var hierarchy = await _roleService.GetRoleHierarchyAsync();
            return View(hierarchy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading role hierarchy");
            TempData["Error"] = "Failed to load role hierarchy";
            return View(Enumerable.Empty<RoleDTO>());
        }
    }

    // GET: /Dashboard/System/Roles/AssignUser
    [RequirePermission(Permissions.Roles.Assign)]
    public async Task<IActionResult> AssignUser(Guid? userId)
    {
        var roles = await _roleService.GetActiveRolesAsync();
        ViewBag.Roles = roles;
        ViewBag.UserId = userId;
        return View();
    }

    // POST: /Dashboard/System/Roles/AssignUser
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Roles.Assign)]
    public async Task<IActionResult> AssignUser(AssignRoleRequest request)
    {
        if (!ModelState.IsValid)
        {
            var roles = await _roleService.GetActiveRolesAsync();
            ViewBag.Roles = roles;
            return View(request);
        }

        try
        {
            var result = await _roleService.AssignRoleAsync(request.UserId, request.RoleName, User.Identity?.Name);
            if (result)
            {
                TempData["Success"] = $"Role '{request.RoleName}' assigned successfully";
            }
            else
            {
                TempData["Error"] = "Failed to assign role";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {RoleName} to user {UserId}", request.RoleName, request.UserId);
            ModelState.AddModelError("", ex.Message);
            
            var roles = await _roleService.GetActiveRolesAsync();
            ViewBag.Roles = roles;
            return View(request);
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: /Dashboard/System/Roles/InitializeSystem
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(Permissions.System.SystemConfiguration)]
    public async Task<IActionResult> InitializeSystem()
    {
        try
        {
            var result = await _roleService.InitializeSystemRolesAsync();
            if (result)
            {
                TempData["Success"] = "System roles initialized successfully";
            }
            else
            {
                TempData["Error"] = "Failed to initialize system roles";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing system roles");
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    #region Private Methods

    private async Task PopulateCreateViewData()
    {
        var categories = await _roleService.GetRoleCategoriesAsync();
        var permissions = await _permissionService.GetAllPermissionsAsync();
        var permissionCategories = await _permissionService.GetPermissionCategoriesAsync();

        ViewBag.Categories = categories.Keys.ToList();
        ViewBag.Permissions = permissions;
        ViewBag.PermissionCategories = permissionCategories;
    }

    private async Task PopulateEditViewData()
    {
        var categories = await _roleService.GetRoleCategoriesAsync();
        var permissions = await _permissionService.GetAllPermissionsAsync();
        var permissionCategories = await _permissionService.GetPermissionCategoriesAsync();

        ViewBag.Categories = categories.Keys.ToList();
        ViewBag.Permissions = permissions;
        ViewBag.PermissionCategories = permissionCategories;
    }

    #endregion
}