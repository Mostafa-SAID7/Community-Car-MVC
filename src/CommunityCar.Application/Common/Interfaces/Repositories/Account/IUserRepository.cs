using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using UserEntity = CommunityCar.Domain.Entities.Account.Core.User;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Account;

/// <summary>
/// Repository interface for User entity operations
/// </summary>
public interface IUserRepository : IBaseRepository<UserEntity>
{
    #region Basic User Operations
    Task<UserEntity?> GetByEmailAsync(string email);
    Task<UserEntity?> GetByUserNameAsync(string userName);
    Task<IEnumerable<UserEntity>> GetActiveUsersAsync();
    Task<IEnumerable<UserEntity>> GetUsersByRoleAsync(string roleName);
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null);
    Task<bool> IsUserNameUniqueAsync(string userName, Guid? excludeUserId = null);
    #endregion

    #region Profile Operations
    Task<UserEntity?> GetUserWithProfileAsync(Guid userId);
    Task<IEnumerable<UserEntity>> SearchUsersAsync(string searchTerm, int page = 1, int pageSize = 20);
    Task<bool> UpdateProfilePictureAsync(Guid userId, string imageUrl);
    Task<bool> RemoveProfilePictureAsync(Guid userId);
    Task<bool> UpdateCoverImageAsync(Guid userId, string imageUrl);
    Task<bool> RemoveCoverImageAsync(Guid userId);
    #endregion

    #region Account Management
    Task<IEnumerable<UserEntity>> GetDeactivatedUsersAsync();
    Task<IEnumerable<UserEntity>> GetUsersForDeletionAsync(DateTime cutoffDate);
    Task<bool> DeactivateUserAsync(Guid userId, string? reason = null);
    Task<bool> ReactivateUserAsync(Guid userId);
    #endregion

    #region Security Operations
    Task<DateTime?> GetLastPasswordChangeAsync(Guid userId);
    Task<bool> UpdateLastLoginAsync(Guid userId, string? ipAddress = null, string? userAgent = null);
    Task<bool> IsAccountLockedAsync(Guid userId);
    Task<DateTime?> GetLockoutEndAsync(Guid userId);
    #endregion

    #region OAuth Operations
    Task<bool> LinkOAuthAccountAsync(Guid userId, string provider, string providerId, string? profilePictureUrl = null);
    Task<bool> UnlinkOAuthAccountAsync(Guid userId, string provider);
    Task<bool> IsOAuthAccountLinkedAsync(Guid userId, string provider);
    Task<string?> GetOAuthAccountIdAsync(Guid userId, string provider);
    #endregion

    #region Statistics
    Task<int> GetTotalUsersCountAsync();
    Task<int> GetActiveUsersCountAsync();
    Task<int> GetNewUsersCountAsync(DateTime fromDate);
    #endregion
}