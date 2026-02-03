using CommunityCar.Application.Common.Interfaces.Repositories.Account;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserEntity = CommunityCar.Domain.Entities.Account.Core.User;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Account.Core;

/// <summary>
/// Repository implementation for User entity operations - Data Access Only
/// </summary>
public class UserRepository : BaseRepository<UserEntity>, IUserRepository
{
    private readonly UserManager<UserEntity> _userManager;

    public UserRepository(ApplicationDbContext context, UserManager<UserEntity> userManager) : base(context)
    {
        _userManager = userManager;
    }

    #region Basic User Operations

    public async Task<UserEntity?> GetByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<UserEntity?> GetByUserNameAsync(string userName)
    {
        return await _userManager.FindByNameAsync(userName);
    }

    public async Task<UserEntity?> GetBySlugAsync(string slug)
    {
        return await Context.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Slug == slug);
    }

    public async Task<IEnumerable<UserEntity>> GetActiveUsersAsync()
    {
        return await Context.Users
            .Where(u => u.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserEntity>> GetUsersByRoleAsync(string roleName)
    {
        return await _userManager.GetUsersInRoleAsync(roleName);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null)
    {
        var query = Context.Users.Where(u => u.Email == email);
        
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }

        return !await query.AnyAsync();
    }

    public async Task<bool> IsUserNameUniqueAsync(string userName, Guid? excludeUserId = null)
    {
        var query = Context.Users.Where(u => u.UserName == userName);
        
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }

        return !await query.AnyAsync();
    }

    #endregion

    #region Profile Operations

    public async Task<UserEntity?> GetUserWithProfileAsync(Guid userId)
    {
        return await Context.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<IEnumerable<UserEntity>> SearchUsersAsync(string searchTerm, int page = 1, int pageSize = 20)
    {
        var normalizedSearchTerm = searchTerm.ToLower();
        
        return await Context.Users
            .Include(u => u.Profile)
            .Where(u => u.IsActive && (
                u.UserName!.ToLower().Contains(normalizedSearchTerm) ||
                u.Email!.ToLower().Contains(normalizedSearchTerm) ||
                (u.Profile != null && u.Profile.FullName != null && u.Profile.FullName.ToLower().Contains(normalizedSearchTerm)) ||
                (u.Profile != null && u.Profile.Bio != null && u.Profile.Bio.ToLower().Contains(normalizedSearchTerm))
            ))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<bool> UpdateProfilePictureAsync(Guid userId, string imageUrl)
    {
        var user = await GetUserWithProfileAsync(userId);
        if (user == null) return false;

        user.Profile.UpdateProfilePicture(imageUrl);
        await UpdateAsync(user);
        return true;
    }

    public async Task<bool> UpdateCoverImageAsync(Guid userId, string imageUrl)
    {
        var user = await GetUserWithProfileAsync(userId);
        if (user == null) return false;

        user.Profile.UpdateCoverImage(imageUrl);
        await UpdateAsync(user);
        return true;
    }

    public async Task<bool> RemoveProfilePictureAsync(Guid userId)
    {
        var user = await GetUserWithProfileAsync(userId);
        if (user == null) return false;

        user.Profile.UpdateProfilePicture(string.Empty);
        await UpdateAsync(user);
        return true;
    }

    public async Task<bool> RemoveCoverImageAsync(Guid userId)
    {
        var user = await GetUserWithProfileAsync(userId);
        if (user == null) return false;

        user.Profile.UpdateCoverImage(string.Empty);
        await UpdateAsync(user);
        return true;
    }

    /// <summary>
    /// Override base UpdateAsync to actually persist changes to database
    /// </summary>
    public new async Task UpdateAsync(UserEntity entity)
    {
        Context.Users.Update(entity);
        await Context.SaveChangesAsync();
    }

    #endregion

    #region Account Management

    public async Task<IEnumerable<UserEntity>> GetDeactivatedUsersAsync()
    {
        return await Context.Users
            .Where(u => !u.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserEntity>> GetUsersForDeletionAsync(DateTime cutoffDate)
    {
        return await Context.Users
            .Where(u => !u.IsActive && u.UpdatedAt < cutoffDate)
            .ToListAsync();
    }

    public async Task<bool> IsAccountLockedAsync(Guid userId)
    {
        var user = await GetByIdAsync(userId);
        return user != null && await _userManager.IsLockedOutAsync(user);
    }

    public async Task<DateTime?> GetLockoutEndAsync(Guid userId)
    {
        var user = await GetByIdAsync(userId);
        if (user == null) return null;
        var offset = await _userManager.GetLockoutEndDateAsync(user);
        return offset?.UtcDateTime;
    }

    public async Task<DateTime?> GetLastPasswordChangeAsync(Guid userId)
    {
        var user = await GetByIdAsync(userId);
        return user?.LastPasswordChangeAt;
    }

    #endregion
}