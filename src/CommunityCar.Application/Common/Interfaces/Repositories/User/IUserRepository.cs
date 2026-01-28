using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Auth;

namespace CommunityCar.Application.Common.Interfaces.Repositories.User;

/// <summary>
/// Unified interface for User entity operations
/// </summary>
public interface IUserRepository : IBaseRepository<Domain.Entities.Auth.User>
{
    #region Generic User Operations
    Task<Domain.Entities.Auth.User?> GetByEmailAsync(string email);
    Task<Domain.Entities.Auth.User?> GetByUserNameAsync(string userName);
    Task<IEnumerable<Domain.Entities.Auth.User>> GetActiveUsersAsync();
    Task<IEnumerable<Domain.Entities.Auth.User>> GetUsersByRoleAsync(string roleName);
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null);
    Task<bool> IsUserNameUniqueAsync(string userName, Guid? excludeUserId = null);
    #endregion

    #region Profile Operations
    Task<Domain.Entities.Auth.User?> GetUserWithProfileAsync(Guid userId);
    Task<IEnumerable<Domain.Entities.Auth.User>> SearchUsersAsync(string searchTerm, int page = 1, int pageSize = 20);
    Task<bool> UpdateProfilePictureAsync(Guid userId, string imageUrl);
    Task<bool> RemoveProfilePictureAsync(Guid userId);
    Task<bool> UpdateCoverImageAsync(Guid userId, string imageUrl);
    Task<bool> RemoveCoverImageAsync(Guid userId);
    #endregion

    #region Account Management
    Task<IEnumerable<Domain.Entities.Auth.User>> GetDeactivatedUsersAsync();
    Task<IEnumerable<Domain.Entities.Auth.User>> GetUsersForDeletionAsync(DateTime cutoffDate);
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


