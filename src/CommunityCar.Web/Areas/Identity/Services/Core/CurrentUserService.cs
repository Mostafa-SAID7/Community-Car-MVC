using System.Security.Claims;
using CommunityCar.Web.Areas.Identity.Interfaces.Services.Core;
using Microsoft.AspNetCore.Http;

namespace CommunityCar.Web.Areas.Identity.Services.Core;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? UserName => _httpContextAccessor.HttpContext?.User?.Identity?.Name;
    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(string role)
    {
        return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
    }

    public bool HasPermission(string permission)
    {
        return _httpContextAccessor.HttpContext?.User?.HasClaim("permission", permission) ?? false;
    }

    public async Task<bool> HasPermissionAsync(string permission)
    {
        return await Task.FromResult(HasPermission(permission));
    }

    public IEnumerable<string> GetRoles()
    {
        return _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role)?.Select(c => c.Value) ?? Enumerable.Empty<string>();
    }

    public IEnumerable<string> GetPermissions()
    {
        return _httpContextAccessor.HttpContext?.User?.FindAll("permission")?.Select(c => c.Value) ?? Enumerable.Empty<string>();
    }

    public async Task<IEnumerable<string>> GetPermissionsAsync()
    {
        return await Task.FromResult(GetPermissions());
    }

    public IEnumerable<Claim> GetUserClaims()
    {
        return _httpContextAccessor.HttpContext?.User?.Claims ?? Enumerable.Empty<Claim>();
    }

    public string? GetClaimValue(string claimType)
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirstValue(claimType);
    }
}

