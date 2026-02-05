using CommunityCar.Application.Features.Dashboard.Management.Users.Core;
using CommunityCar.Domain.Entities.Account.Core;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Management.Users.Core;

public interface IUserManagementRepository
{
    Task<(List<UserManagementDashboardVM> Users, int TotalCount)> GetUsersAsync(
        string? search = null, 
        string? role = null, 
        bool? isActive = null, 
        int page = 1, 
        int pageSize = 20);
    
    Task<User?> GetUserByIdAsync(string userId);
    Task<UserManagementVM?> GetUserManagementByIdAsync(string userId);
    Task<string> CreateUserAsync(AdminCreateUserVM model);
    Task<bool> UpdateUserAsync(DashboardUpdateUserVM model);
    Task<bool> DeleteUserAsync(string userId);
    Task<bool> SoftDeleteUserAsync(string userId);
    Task<bool> RestoreUserAsync(string userId);
    Task<UserManagementStatsVM> GetUserStatsAsync();
    Task<List<UserManagementDashboardVM>> GetRecentUsersAsync(int count = 10);
}