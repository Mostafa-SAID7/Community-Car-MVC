using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CommunityCar.Application.Common.Interfaces.Services.Account.Authorization;
using System.Security.Claims;

namespace CommunityCar.Web.Attributes;

/// <summary>
/// Authorization attribute that checks for specific permissions
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string[] _permissions;
    private readonly bool _requireAll;

    /// <summary>
    /// Requires specific permissions
    /// </summary>
    /// <param name="permissions">Required permissions</param>
    /// <param name="requireAll">If true, user must have ALL permissions. If false, user needs ANY permission.</param>
    public RequirePermissionAttribute(params string[] permissions)
    {
        _permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
        _requireAll = true;
    }

    /// <summary>
    /// Requires specific permissions with option to require all or any
    /// </summary>
    /// <param name="requireAll">If true, user must have ALL permissions. If false, user needs ANY permission.</param>
    /// <param name="permissions">Required permissions</param>
    public RequirePermissionAttribute(bool requireAll, params string[] permissions)
    {
        _permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
        _requireAll = requireAll;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Check if user is authenticated
        if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Get user ID from claims
        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Get permission service
        var permissionService = context.HttpContext.RequestServices.GetService<IPermissionService>();
        if (permissionService == null)
        {
            context.Result = new StatusCodeResult(500); // Internal server error
            return;
        }

        try
        {
            bool hasPermission;

            if (_requireAll)
            {
                // User must have ALL permissions
                hasPermission = await permissionService.HasAllPermissionsAsync(userId, _permissions);
            }
            else
            {
                // User must have ANY permission
                hasPermission = await permissionService.HasAnyPermissionAsync(userId, _permissions);
            }

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
        catch (Exception)
        {
            // Log the exception in a real application
            context.Result = new StatusCodeResult(500);
            return;
        }
    }
}

/// <summary>
/// Convenience attribute for requiring ANY of the specified permissions
/// </summary>
public class RequireAnyPermissionAttribute : RequirePermissionAttribute
{
    public RequireAnyPermissionAttribute(params string[] permissions) : base(false, permissions)
    {
    }
}

/// <summary>
/// Convenience attribute for requiring ALL of the specified permissions
/// </summary>
public class RequireAllPermissionsAttribute : RequirePermissionAttribute
{
    public RequireAllPermissionsAttribute(params string[] permissions) : base(true, permissions)
    {
    }
}