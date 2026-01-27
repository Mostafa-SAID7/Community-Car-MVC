using CommunityCar.Application.Features.Identity.ViewModels;
using CommunityCar.Application.Features.Identity.DTOs;

namespace CommunityCar.Application.Common.Interfaces.Services.Identity;

/// <summary>
/// Interface for role management operations
/// </summary>
public interface IRoleManagementService
{
    Task<IEnumerable<RoleVM>> GetAllRolesAsync();
    Task<bool> CreateRoleAsync(CreateRoleRequest request);
    Task<bool> UpdateRoleAsync(UpdateRoleRequest request);
    Task<bool> DeleteRoleAsync(string roleName);
    Task<bool> AssignRoleToUserAsync(Guid userId, string roleName);
    Task<bool> RemoveRoleFromUserAsync(Guid userId, string roleName);
    Task<IEnumerable<string>> GetUserRolesAsync(Guid userId);
}


