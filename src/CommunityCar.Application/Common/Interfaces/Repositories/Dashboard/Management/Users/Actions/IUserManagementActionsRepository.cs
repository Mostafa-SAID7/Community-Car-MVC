using CommunityCar.Application.Features.Dashboard.Management.Users.Actions;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Management.Users.Actions;

public interface IUserManagementActionsRepository
{
    Task<bool> ExecuteBulkActionAsync(BulkUserActionVM model);
    Task<bool> BulkActivateUsersAsync(List<string> userIds);
    Task<bool> BulkDeactivateUsersAsync(List<string> userIds);
    Task<bool> BulkDeleteUsersAsync(List<string> userIds);
    Task<bool> AssignRoleAsync(string userId, string roleName);
    Task<bool> RemoveRoleAsync(string userId, string roleName);
    Task<List<BulkUserActionVM>> GetBulkActionHistoryAsync(int page = 1, int pageSize = 20);
    Task<bool> LogBulkActionAsync(BulkUserActionVM action);
}