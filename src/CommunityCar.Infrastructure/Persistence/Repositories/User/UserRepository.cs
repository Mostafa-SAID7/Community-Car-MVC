using CommunityCar.Application.Common.Interfaces.Repositories.User;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UserEntity = CommunityCar.Domain.Entities.Account.User;

namespace CommunityCar.Infrastructure.Persistence.Repositories.User;

/// <summary>
/// Comprehensive User repository implementing all user-related operations
/// </summary>
public class UserRepository : BaseRepository<UserEntity>, IUserRepository
{
    private readonly UserManager<UserEntity> _userManager;

    public UserRepository(ApplicationDbContext context, UserManager<UserEntity> userManager) : base(context)
    {
        _userManager = userManager;
    }

    #region Generic User Operations

    public async Task<CommunityCar.Domain.Entities.Account.User?> GetByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<CommunityCar.Domain.Entities.Account.User?> GetByUserNameAsync(string userName)
    {
        return await _userManager.FindByNameAsync(userName);
    }

    public async Task<IEnumerable<CommunityCar.Domain.Entities.Account.User>> GetActiveUsersAsync()
    {
        return await Context.Users
            .Where(u => u.IsActive && !u.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<CommunityCar.Domain.Entities.Account.User>> GetUsersByRoleAsync(string roleName)
    {
        return await _userManager.GetUsersInRoleAsync(roleName);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null)
    {
        var query = Context.Users.Where(u => u.Email == email && !u.IsDeleted);
        
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }

        return !await query.AnyAsync();
    }

    public async Task<bool> IsUserNameUniqueAsync(string userName, Guid? excludeUserId = null)
    {
        var query = Context.Users.Where(u => u.UserName == userName && !u.IsDeleted);
        
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }

        return !await query.AnyAsync();
    }

    #endregion

    #region Profile Operations

    public async Task<CommunityCar.Domain.Entities.Account.User?> GetUserWithProfileAsync(Guid userId)
    {
        return await Context.Users
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
    }

    public async Task<IEnumerable<CommunityCar.Domain.Entities.Account.User>> SearchUsersAsync(string searchTerm, int page = 1, int pageSize = 20)
    {
        var query = Context.Users
            .Where(u => !u.IsDeleted && u.IsActive)
            .Where(u => 
                u.FullName.Contains(searchTerm) ||
                u.UserName!.Contains(searchTerm) ||
                u.Email!.Contains(searchTerm) ||
                (u.Bio != null && u.Bio.Contains(searchTerm)) ||
                (u.City != null && u.City.Contains(searchTerm))
            );

        return await query
            .OrderBy(u => u.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<bool> UpdateProfilePictureAsync(Guid userId, string imageUrl)
    {
        var user = await GetByIdAsync(userId);
        if (user == null) return false;

        user.ProfilePictureUrl = imageUrl;
        user.Audit(userId.ToString());
        
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> RemoveProfilePictureAsync(Guid userId)
    {
        var user = await GetByIdAsync(userId);
        if (user == null) return false;

        user.ProfilePictureUrl = null;
        user.Audit(userId.ToString());
        
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> UpdateCoverImageAsync(Guid userId, string imageUrl)
    {
        var user = await GetByIdAsync(userId);
        if (user == null) return false;

        user.CoverImageUrl = imageUrl;
        user.Audit(userId.ToString());
        
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> RemoveCoverImageAsync(Guid userId)
    {
        var user = await GetByIdAsync(userId);
        if (user == null) return false;

        user.CoverImageUrl = null;
        user.Audit(userId.ToString());
        
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    #endregion

    #region Account Management

    public async Task<IEnumerable<CommunityCar.Domain.Entities.Account.User>> GetDeactivatedUsersAsync()
    {
        return await Context.Users
            .Where(u => !u.IsActive && !u.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<CommunityCar.Domain.Entities.Account.User>> GetUsersForDeletionAsync(DateTime cutoffDate)
    {
        return await Context.Users
            .Where(u => !u.IsActive && u.UpdatedAt < cutoffDate && !u.IsDeleted)
            .ToListAsync();
    }

    public async Task<bool> DeactivateUserAsync(Guid userId, string? reason = null)
    {
        var user = await GetByIdAsync(userId);
        if (user == null) return false;

        user.Deactivate();
        user.Audit(userId.ToString());
        
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> ReactivateUserAsync(Guid userId)
    {
        var user = await GetByIdAsync(userId);
        if (user == null) return false;

        user.Restore(userId.ToString());
        // No need for separate Audit call as Restore already audits
        
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    #endregion

    #region Security Operations

    public async Task<DateTime?> GetLastPasswordChangeAsync(Guid userId)
    {
        var user = await GetByIdAsync(userId);
        return user?.LastPasswordChangeAt;
    }

    public async Task<bool> UpdateLastLoginAsync(Guid userId, string? ipAddress = null, string? userAgent = null)
    {
        var user = await GetByIdAsync(userId);
        if (user == null) return false;

        user.UpdateLastLogin();
        user.Audit(userId.ToString());
        
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> IsAccountLockedAsync(Guid userId)
    {
        var user = await GetByIdAsync(userId);
        if (user == null) return false;

        return await _userManager.IsLockedOutAsync(user);
    }

    public async Task<DateTime?> GetLockoutEndAsync(Guid userId)
    {
        var user = await GetByIdAsync(userId);
        if (user == null) return null;

        var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
        return lockoutEnd?.DateTime;
    }

    #endregion

    #region OAuth Operations

    public async Task<bool> LinkOAuthAccountAsync(Guid userId, string provider, string providerId, string? profilePictureUrl = null)
    {
        var user = await GetByIdAsync(userId);
        if (user == null) return false;

        switch (provider.ToLowerInvariant())
        {
            case "google":
                user.LinkGoogleAccount(providerId);
                if (!string.IsNullOrEmpty(profilePictureUrl))
                {
                    var updatedProfile = user.Profile.UpdateProfilePicture(profilePictureUrl);
                    user.UpdateProfile(updatedProfile);
                }
                break;
            case "facebook":
                user.LinkFacebookAccount(providerId);
                if (!string.IsNullOrEmpty(profilePictureUrl))
                {
                    var updatedProfile = user.Profile.UpdateProfilePicture(profilePictureUrl);
                    user.UpdateProfile(updatedProfile);
                }
                break;
            default:
                return false;
        }

        user.Audit(userId.ToString());
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> UnlinkOAuthAccountAsync(Guid userId, string provider)
    {
        var user = await GetByIdAsync(userId);
        if (user == null) return false;

        switch (provider.ToLowerInvariant())
        {
            case "google":
                user.UnlinkGoogleAccount();
                break;
            case "facebook":
                user.UnlinkFacebookAccount();
                break;
            default:
                return false;
        }

        user.Audit(userId.ToString());
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> IsOAuthAccountLinkedAsync(Guid userId, string provider)
    {
        var user = await GetByIdAsync(userId);
        if (user == null) return false;

        return provider.ToLowerInvariant() switch
        {
            "google" => user.OAuthInfo.HasGoogleAccount,
            "facebook" => user.OAuthInfo.HasFacebookAccount,
            _ => false
        };
    }

    public async Task<string?> GetOAuthAccountIdAsync(Guid userId, string provider)
    {
        var user = await GetByIdAsync(userId);
        if (user == null) return null;

        return provider.ToLowerInvariant() switch
        {
            "google" => user.OAuthInfo.GoogleId,
            "facebook" => user.OAuthInfo.FacebookId,
            _ => null
        };
    }

    #endregion

    #region Statistics

    public async Task<int> GetTotalUsersCountAsync()
    {
        return await Context.Users
            .Where(u => !u.IsDeleted)
            .CountAsync();
    }

    public async Task<int> GetActiveUsersCountAsync()
    {
        return await Context.Users
            .Where(u => u.IsActive && !u.IsDeleted)
            .CountAsync();
    }

    public async Task<int> GetNewUsersCountAsync(DateTime fromDate)
    {
        return await Context.Users
            .Where(u => u.CreatedAt >= fromDate && !u.IsDeleted)
            .CountAsync();
    }

    #endregion

    #region Additional Helper Methods

    public async Task<IEnumerable<CommunityCar.Domain.Entities.Account.User>> FindAsync(Expression<Func<CommunityCar.Domain.Entities.Account.User, bool>> predicate)
    {
        return await Context.Users
            .Where(predicate)
            .Where(u => !u.IsDeleted)
            .ToListAsync();
    }

    #endregion
}