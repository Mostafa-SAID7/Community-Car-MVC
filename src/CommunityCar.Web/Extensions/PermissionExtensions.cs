using CommunityCar.Application.Common.Interfaces.Services.Account.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace CommunityCar.Web.Extensions;

/// <summary>
/// Extension methods for checking permissions in views and controllers
/// </summary>
public static class PermissionExtensions
{
   
    public static async Task<bool> HasPermissionAsync(this IHtmlHelper htmlHelper, string permission)
    {
        var httpContext = htmlHelper.ViewContext.HttpContext;
        var permissionService = httpContext.RequestServices.GetService<IPermissionService>();
        
        if (permissionService == null) return false;

        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return false;

        try
        {
            return await permissionService.HasPermissionAsync(userId, permission);
        }
        catch
        {
            return false;
        }
    }

    public static async Task<bool> HasAnyPermissionAsync(this IHtmlHelper htmlHelper, params string[] permissions)
    {
        var httpContext = htmlHelper.ViewContext.HttpContext;
        var permissionService = httpContext.RequestServices.GetService<IPermissionService>();
        
        if (permissionService == null) return false;

        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return false;

        try
        {
            return await permissionService.HasAnyPermissionAsync(userId, permissions);
        }
        catch
        {
            return false;
        }
    }


    public static async Task<bool> HasAllPermissionsAsync(this IHtmlHelper htmlHelper, params string[] permissions)
    {
        var httpContext = htmlHelper.ViewContext.HttpContext;
        var permissionService = httpContext.RequestServices.GetService<IPermissionService>();
        
        if (permissionService == null) return false;

        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return false;

        try
        {
            return await permissionService.HasAllPermissionsAsync(userId, permissions);
        }
        catch
        {
            return false;
        }
    }


    public static async Task<bool> IsInRoleAsync(this IHtmlHelper htmlHelper, string roleName)
    {
        var httpContext = htmlHelper.ViewContext.HttpContext;
        var roleService = httpContext.RequestServices.GetService<IRoleService>();
        
        if (roleService == null) return false;

        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return false;

        try
        {
            return await roleService.IsInRoleAsync(userId, roleName);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Get current user's permissions
    /// </summary>
    public static async Task<IEnumerable<string>> GetUserPermissionsAsync(this IHtmlHelper htmlHelper)
    {
        var httpContext = htmlHelper.ViewContext.HttpContext;
        var permissionService = httpContext.RequestServices.GetService<IPermissionService>();
        
        if (permissionService == null) return Enumerable.Empty<string>();

        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Enumerable.Empty<string>();

        try
        {
            return await permissionService.GetUserPermissionsAsync(userId);
        }
        catch
        {
            return Enumerable.Empty<string>();
        }
    }


    public static async Task<IEnumerable<string>> GetUserRolesAsync(this IHtmlHelper htmlHelper)
    {
        var httpContext = htmlHelper.ViewContext.HttpContext;
        var roleService = httpContext.RequestServices.GetService<IRoleService>();
        
        if (roleService == null) return Enumerable.Empty<string>();

        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Enumerable.Empty<string>();

        try
        {
            return await roleService.GetUserRolesAsync(userId);
        }
        catch
        {
            return Enumerable.Empty<string>();
        }
    }
}