using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Identity.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace CommunityCar.Application.Services.Identity.Claims;

/// <summary>
/// Service for user claims management operations
/// </summary>
public class ClaimsManagementService : IClaimsManagementService
{
    private readonly UserManager<Domain.Entities.Auth.User> _userManager;
    private readonly ILogger<ClaimsManagementService> _logger;

    public ClaimsManagementService(
        UserManager<Domain.Entities.Auth.User> userManager,
        ILogger<ClaimsManagementService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IEnumerable<UserClaimVM>> GetUserClaimsAsync(Guid userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", userId);
                return Enumerable.Empty<UserClaimVM>();
            }

            var claims = await _userManager.GetClaimsAsync(user);
            return claims.Select(c => new UserClaimVM
            {
                Type = c.Type,
                Value = c.Value,
                Issuer = c.Issuer
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting claims for user {UserId}", userId);
            return Enumerable.Empty<UserClaimVM>();
        }
    }

    public async Task<bool> AddClaimToUserAsync(Guid userId, string claimType, string claimValue)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", userId);
                return false;
            }

            var claim = new Claim(claimType, claimValue);
            var result = await _userManager.AddClaimAsync(user, claim);

            if (result.Succeeded)
            {
                _logger.LogInformation("Claim {ClaimType}:{ClaimValue} added to user {UserId}", 
                    claimType, claimValue, userId);
                return true;
            }

            _logger.LogWarning("Failed to add claim {ClaimType}:{ClaimValue} to user {UserId}: {Errors}", 
                claimType, claimValue, userId, string.Join(", ", result.Errors.Select(e => e.Description)));
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding claim {ClaimType}:{ClaimValue} to user {UserId}", 
                claimType, claimValue, userId);
            return false;
        }
    }

    public async Task<bool> RemoveClaimFromUserAsync(Guid userId, string claimType, string claimValue)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", userId);
                return false;
            }

            var claim = new Claim(claimType, claimValue);
            var result = await _userManager.RemoveClaimAsync(user, claim);

            if (result.Succeeded)
            {
                _logger.LogInformation("Claim {ClaimType}:{ClaimValue} removed from user {UserId}", 
                    claimType, claimValue, userId);
                return true;
            }

            _logger.LogWarning("Failed to remove claim {ClaimType}:{ClaimValue} from user {UserId}: {Errors}", 
                claimType, claimValue, userId, string.Join(", ", result.Errors.Select(e => e.Description)));
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing claim {ClaimType}:{ClaimValue} from user {UserId}", 
                claimType, claimValue, userId);
            return false;
        }
    }

    public async Task<bool> UpdateUserClaimAsync(Guid userId, string oldClaimType, string oldClaimValue, 
        string newClaimType, string newClaimValue)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", userId);
                return false;
            }

            var oldClaim = new Claim(oldClaimType, oldClaimValue);
            var newClaim = new Claim(newClaimType, newClaimValue);
            
            var result = await _userManager.ReplaceClaimAsync(user, oldClaim, newClaim);

            if (result.Succeeded)
            {
                _logger.LogInformation("Claim updated for user {UserId}: {OldClaimType}:{OldClaimValue} -> {NewClaimType}:{NewClaimValue}", 
                    userId, oldClaimType, oldClaimValue, newClaimType, newClaimValue);
                return true;
            }

            _logger.LogWarning("Failed to update claim for user {UserId}: {Errors}", 
                userId, string.Join(", ", result.Errors.Select(e => e.Description)));
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating claim for user {UserId}", userId);
            return false;
        }
    }
}


