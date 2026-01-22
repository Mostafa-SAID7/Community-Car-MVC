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

    // Added to match ManagementController expectations
    Task<List<SystemHealthVM>> GetSystemHealthAsync();
    Task<List<UserManagementVM>> GetUsersAsync(int page, int pageSize, string? search);
    Task<UserManagementVM?> GetUserAsync(Guid userId);
    Task<bool> BlockUserAsync(Guid userId, string reason);
    Task<bool> UnblockUserAsync(Guid userId);
    Task<bool> UpdateUserRoleAsync(Guid userId, string role);
    Task<List<ContentModerationVM>> GetContentModerationAsync(int page, int pageSize);
    Task<bool> ModerateContentAsync(ModerateContentRequest request);
    Task<List<SystemAlertVM>> GetSystemAlertsAsync();
}