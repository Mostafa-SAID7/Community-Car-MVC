using CommunityCar.Application.Features.Dashboard.Management.Users.Actions;
using CommunityCar.Application.Common.Models;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.Management.Users.Actions;

public interface IUserManagementActionsService
{
    Task<Result> ExecuteBulkActionAsync(BulkUserActionVM model);
    Task<Result> BulkActivateUsersAsync(List<string> userIds);
    Task<Result> BulkDeactivateUsersAsync(List<string> userIds);
    Task<Result> BulkDeleteUsersAsync(List<string> userIds);
    Task<Result> AssignRoleAsync(string userId, string roleName);
    Task<Result> RemoveRoleAsync(string userId, string roleName);
    Task<List<BulkUserActionVM>> GetBulkActionHistoryAsync(int page = 1, int pageSize = 20);
    Task<bool> PerformUserActionAsync(string action, Guid userId, string reason);
    Task<bool> ReverseUserActionAsync(Guid actionId);
}