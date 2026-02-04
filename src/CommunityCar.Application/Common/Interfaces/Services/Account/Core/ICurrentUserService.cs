namespace CommunityCar.Application.Common.Interfaces.Services.Account.Core;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
    bool HasPermission(string permission);
    Task<bool> HasPermissionAsync(string permission);
    IEnumerable<string> GetRoles();
    IEnumerable<string> GetPermissions();
    Task<IEnumerable<string>> GetPermissionsAsync();
    IEnumerable<System.Security.Claims.Claim> GetUserClaims();
    string? GetClaimValue(string claimType);
}