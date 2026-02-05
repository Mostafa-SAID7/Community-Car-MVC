using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using UserEntity = CommunityCar.Domain.Entities.Account.Core.User;

namespace CommunityCar.Web.Areas.Identity.Interfaces.Repositories;

/// <summary>
/// Repository interface for User entity operations - Data Access Only
/// </summary>
public interface IUserRepository : IBaseRepository<UserEntity>
{
    #region Basic User Operations
    Task<UserEntity?> GetByEmailAsync(string email);
    Task<UserEntity?> GetByUserNameAsync(string userName);
    Task<UserEntity?> GetBySlugAsync(string slug);
    Task<IEnumerable<UserEntity>> GetActiveUsersAsync();
    Task<IEnumerable<UserEntity>> GetUsersByRoleAsync(string roleName);
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null);
    Task<bool> IsUserNameUniqueAsync(string userName, Guid? excludeUserId = null);
    #endregion

    #region Profile Operations
    Task<UserEntity?> GetUserWithProfileAsync(Guid userId);
    Task<IEnumerable<UserEntity>> SearchUsersAsync(string searchTerm, int page = 1, int pageSize = 20);
    Task<bool> UpdateProfilePictureAsync(Guid userId, string imageUrl);
    Task<bool> UpdateCoverImageAsync(Guid userId, string imageUrl);
    Task<bool> RemoveProfilePictureAsync(Guid userId);
    Task<bool> RemoveCoverImageAsync(Guid userId);
    #endregion

    #region Account Management
    Task<IEnumerable<UserEntity>> GetDeactivatedUsersAsync();
    Task<IEnumerable<UserEntity>> GetUsersForDeletionAsync(DateTime cutoffDate);
    Task<bool> IsAccountLockedAsync(Guid userId);
    Task<DateTime?> GetLockoutEndAsync(Guid userId);
    Task<DateTime?> GetLastPasswordChangeAsync(Guid userId);
    #endregion

    #region Social Operations
    Task<IEnumerable<UserEntity>> GetFollowersAsync(Guid userId);
    Task<IEnumerable<UserEntity>> GetFollowingAsync(Guid userId);
    #endregion
}
