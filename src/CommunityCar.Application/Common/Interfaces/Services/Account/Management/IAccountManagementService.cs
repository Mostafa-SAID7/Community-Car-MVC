using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Features.Account.ViewModels.Core;
using CommunityCar.Application.Features.Account.ViewModels.Management;

namespace CommunityCar.Application.Common.Interfaces.Services.Account.Management;

public interface IAccountManagementService
{
    Task<Result<UserVM>> GetUserByIdAsync(Guid userId);
    Task<Result<UserVM>> GetUserByEmailAsync(string email);
    Task<Result<UserVM>> GetUserByUsernameAsync(string username);
    Task<Result<(IEnumerable<UserVM> Users, PaginationInfo Pagination)>> GetUsersAsync(int page = 1, int pageSize = 20, string? search = null);
    Task<Result<UserVM>> CreateUserAsync(CreateUserVM request);
    Task<Result<UserVM>> UpdateUserAsync(Guid userId, UpdateUserVM request);
    Task<Result> DeleteUserAsync(Guid userId);
    Task<Result> SoftDeleteUserAsync(Guid userId);
    Task<Result> RestoreUserAsync(Guid userId);
    Task<Result> LockUserAsync(Guid userId, string reason);
    Task<Result> UnlockUserAsync(Guid userId);
    Task<Result> AssignRoleAsync(Guid userId, string roleName);
    Task<Result> RemoveRoleAsync(Guid userId, string roleName);
    Task<Result<List<string>>> GetUserRolesAsync(Guid userId);
    Task<Result<ExportDataVM>> ExportUserDataAsync(Guid userId);
    Task<Result> ProcessDataDeletionRequestAsync(Guid userId);
    Task<Result> UpdateConsentAsync(Guid userId, ConsentVM consent);
}