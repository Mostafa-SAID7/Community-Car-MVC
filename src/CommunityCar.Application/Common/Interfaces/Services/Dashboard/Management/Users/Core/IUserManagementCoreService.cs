using CommunityCar.Application.Features.Dashboard.Management.Users.Core;
using CommunityCar.Application.Features.Shared.ViewModels.Users;
using CommunityCar.Application.Common.Models;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.Management.Users.Core;

public interface IUserManagementCoreService
{
    Task<(List<UserManagementDashboardVM> Users, int TotalCount)> GetUsersAsync(
        string? search = null, 
        string? role = null, 
        bool? isActive = null, 
        int page = 1, 
        int pageSize = 20);
    
    Task<UserDetailVM> GetUserAsync(string userId);
    Task<Result> CreateUserAsync(AdminCreateUserVM model);
    Task<Result> UpdateUserAsync(CommunityCar.Application.Features.Shared.ViewModels.Users.UpdateUserVM model);
    Task<Result> DeleteUserAsync(string userId);
    Task<UserManagementStatsVM> GetUserStatsAsync();
}