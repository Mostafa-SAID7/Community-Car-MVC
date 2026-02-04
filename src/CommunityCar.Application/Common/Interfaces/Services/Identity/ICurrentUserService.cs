namespace CommunityCar.Application.Common.Interfaces.Services.Identity;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
    bool HasPermission(string permission);
    Task<bool> HasPermissionAsync(string permission);
    List<string> GetRoles();
    List<string> GetPermissions();
    Task<List<string>> GetPermissionsAsync();
    Dictionary<string, object> GetUserClaims();
    string? GetClaimValue(string claimType);
}