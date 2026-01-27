using CommunityCar.Application.Common.Interfaces.Repositories.User;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Identity.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Identity.User;

/// <summary>
/// Service for user identity operations (status, locking, basic info)
/// </summary>
public class UserIdentityService : IUserIdentityService
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<Domain.Entities.Auth.User> _userManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UserIdentityService> _logger;

    public UserIdentityService(
        IUserRepository userRepository,
        UserManager<Domain.Entities.Auth.User> userManager,
        ICurrentUserService currentUserService,
        ILogger<UserIdentityService> logger)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<UserIdentityVM?> GetUserIdentityAsync(Guid userId)
    {
        try
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
                FullName = user.FullName,
                IsActive = user.IsActive,
                IsEmailConfirmed = user.EmailConfirmed,
                IsLocked = isLocked,
                LockoutEnd = await _userManager.GetLockoutEndDateAsync(user),
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                Roles = roles.ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user identity for user {UserId}", userId);
            return null;
        }
    }

    public async Task<IEnumerable<UserIdentityVM>> GetAllUsersAsync(int page = 1, int pageSize = 20)
    {
        try
        {
            var users = await _userRepository.GetActiveUsersAsync();
            var pagedUsers = users.Skip((page - 1) * pageSize).Take(pageSize);
            var userIdentities = new List<UserIdentityVM>();

            foreach (var user in pagedUsers)
            {
                var identity = await GetUserIdentityAsync(user.Id);
                if (identity != null)
                {
                    userIdentities.Add(identity);
                }
            }

            return userIdentities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users, page {Page}, size {PageSize}", page, pageSize);
            return Enumerable.Empty<UserIdentityVM>();
        }
    }

    public async Task<bool> IsUserActiveAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user?.IsActive ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} is active", userId);
            return false;
        }
    }

    public async Task<bool> LockUserAsync(Guid userId, string reason)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            // Lock user for 24 hours
            var lockoutEnd = DateTimeOffset.UtcNow.AddHours(24);
            var result = await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserId} locked until {LockoutEnd}. Reason: {Reason}", 
                    userId, lockoutEnd, reason);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error locking user {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> UnlockUserAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.SetLockoutEndDateAsync(user, null);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserId} unlocked", userId);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlocking user {UserId}", userId);
            return false;
        }
    }
}


