using CommunityCar.Application.Features.Dashboard.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard;

public interface IUserManagementService
{
    Task<(List<UserManagementVM> Users, int TotalCount)> GetUsersAsync(string? search = null, string? role = null, bool? isActive = null, int page = 1, int pageSize = 20);
    Task<UserManagementVM?> GetUserByIdAsync(Guid userId);
    Task<bool> CreateUserAsync(CreateUserVM model);
    Task<bool> UpdateUserAsync(Guid userId, UpdateUserVM model);
    Task<bool> DeleteUserAsync(Guid userId);
    Task<bool> ActivateUserAsync(Guid userId);
    Task<bool> DeactivateUserAsync(Guid userId);
    Task<bool> LockUserAsync(Guid userId, string reason, DateTime? unlockDate = null);
    Task<bool> UnlockUserAsync(Guid userId);
    Task<bool> ResetPasswordAsync(Guid userId, string newPassword);
    Task<bool> SendPasswordResetEmailAsync(Guid userId);
    Task<List<string>> GetUserRolesAsync(Guid userId);
    Task<bool> AssignRoleAsync(Guid userId, string role);
    Task<bool> RemoveRoleAsync(Guid userId, string role);
    Task<List<string>> GetAvailableRolesAsync();
    Task<UserStatisticsVM> GetUserStatisticsAsync();
    Task<List<ChartDataVM>> GetUserRegistrationChartAsync(int days);
    Task<List<UserActivityVM>> GetRecentUserActivityAsync(int limit = 50);
    Task<bool> BulkUpdateUsersAsync(List<Guid> userIds, BulkUserUpdateVM model);
    Task<bool> ExportUsersAsync(string format = "csv");
    Task<bool> ImportUsersAsync(string filePath);
}