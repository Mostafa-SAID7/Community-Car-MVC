using CommunityCar.Application.Features.Dashboard.DTOs;
using CommunityCar.Application.Features.Dashboard.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard;

public interface IDashboardManagementService
{
    Task<List<UserManagementVM>> GetUserManagementHistoryAsync(int page = 1, int pageSize = 20);
    Task<List<UserManagementVM>> GetUserManagementHistoryByUserAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<bool> PerformUserActionAsync(UserManagementRequest request);
    Task<bool> ReverseUserActionAsync(Guid actionId);
    Task ProcessExpiredActionsAsync();
}