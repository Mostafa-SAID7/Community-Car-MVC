using CommunityCar.Application.Common.Models.Identity;

namespace CommunityCar.Application.Common.Interfaces.Services.Identity;

/// <summary>
/// Interface for identity management operations (users, roles, claims)
/// </summary>
public interface IIdentityManagementService
{
    // User Identity
    Task<UserIdentityVM?> GetUserIdentityAsync(Guid userId);
    Task<IEnumerable<UserIdentityVM>> GetAllUsersAsync(int page = 1, int pageSize = 20);
    Task<bool> IsUserActiveAsync(Guid userId);
    Task<bool> LockUserAsync(Guid userId, string reason);
    Task<bool> UnlockUserAsync(Guid userId);



    // Claims Management
    Task<IEnumerable<UserClaimVM>> GetUserClaimsAsync(Guid userId);
    Task<bool> AddClaimToUserAsync(Guid userId, string claimType, string claimValue);
    Task<bool> RemoveClaimFromUserAsync(Guid userId, string claimType, string claimValue);
    Task<bool> UpdateUserClaimAsync(Guid userId, string oldClaimType, string oldClaimValue, string newClaimType, string newClaimValue);
}


