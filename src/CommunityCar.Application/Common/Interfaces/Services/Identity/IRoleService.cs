using Microsoft.AspNetCore.Identity;

namespace CommunityCar.Application.Common.Interfaces.Services.Identity;

public interface IRoleService
{
    Task<IdentityResult> CreateRoleAsync(string roleName);
    Task<IdentityResult> DeleteRoleAsync(string roleName);
    Task<bool> RoleExistsAsync(string roleName);
    Task<IEnumerable<IdentityRole<Guid>>> GetAllRolesAsync();
    Task<IdentityResult> AddUserToRoleAsync(Guid userId, string roleName);
    Task<IdentityResult> RemoveUserFromRoleAsync(Guid userId, string roleName);
    Task<IEnumerable<string>> GetUserRolesAsync(Guid userId);
    Task<bool> IsUserInRoleAsync(Guid userId, string roleName);
}


