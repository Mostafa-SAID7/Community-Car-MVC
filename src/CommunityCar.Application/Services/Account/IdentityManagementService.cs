using CommunityCar.Application.Common.Interfaces.Repositories.User;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Models.Identity;
using CommunityCar.Domain.Entities.Account.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace CommunityCar.Application.Services.Identity;

public class IdentityManagementService : IIdentityManagementService
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<User> _userManager;
    public IdentityManagementService(
        IUserRepository userRepository,
        UserManager<User> userManager,
        ILogger<IdentityManagementService> logger)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _logger = logger;
    }

    #region User Identity

    public async Task<UserIdentityVM?> GetUserIdentityAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        var isLocked = await _userManager.IsLockedOutAsync(user);

        return new UserIdentityVM
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FullName = user.Profile.FullName,
            IsActive = user.IsActive,
            IsEmailConfirmed = user.EmailConfirmed,
            IsLocked = isLocked,
            LockoutEnd = await _userManager.GetLockoutEndDateAsync(user),
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.OAuthInfo.LastLoginAt,
            Roles = roles.ToList()
        };
    }

    public async Task<IEnumerable<UserIdentityVM>> GetAllUsersAsync(int page = 1, int pageSize = 20)
    {
        var users = await _userRepository.GetActiveUsersAsync();
        var pagedUsers = users.Skip((page - 1) * pageSize).Take(pageSize);
        var userIdentities = new List<UserIdentityVM>();

        foreach (var user in pagedUsers)
        {
            var identity = await GetUserIdentityAsync(user.Id);
            if (identity != null) userIdentities.Add(identity);
        }

        return userIdentities;
    }

    public async Task<bool> IsUserActiveAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user?.IsActive ?? false;
    }

    public async Task<bool> LockUserAsync(Guid userId, string reason)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        var lockoutEnd = DateTimeOffset.UtcNow.AddHours(24);
        var result = await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);
        return result.Succeeded;
    }

    public async Task<bool> UnlockUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        var result = await _userManager.SetLockoutEndDateAsync(user, null);
        return result.Succeeded;
    }

    #endregion



    #region Claims Management

    public async Task<IEnumerable<UserClaimVM>> GetUserClaimsAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Enumerable.Empty<UserClaimVM>();
        var claims = await _userManager.GetClaimsAsync(user);
        return claims.Select(c => new UserClaimVM { Type = c.Type, Value = c.Value });
    }

    public async Task<bool> AddClaimToUserAsync(Guid userId, string claimType, string claimValue)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;
        var result = await _userManager.AddClaimAsync(user, new Claim(claimType, claimValue));
        return result.Succeeded;
    }

    public async Task<bool> RemoveClaimFromUserAsync(Guid userId, string claimType, string claimValue)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;
        var result = await _userManager.RemoveClaimAsync(user, new Claim(claimType, claimValue));
        return result.Succeeded;
    }

    public async Task<bool> UpdateUserClaimAsync(Guid userId, string oldClaimType, string oldClaimValue, string newClaimType, string newClaimValue)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;
        var result = await _userManager.ReplaceClaimAsync(user, new Claim(oldClaimType, oldClaimValue), new Claim(newClaimType, newClaimValue));
        return result.Succeeded;
    }

    #endregion
}
