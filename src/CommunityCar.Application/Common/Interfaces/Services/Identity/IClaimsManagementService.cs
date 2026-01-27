using CommunityCar.Application.Features.Identity.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Identity;

/// <summary>
/// Interface for claims management operations
/// </summary>
public interface IClaimsManagementService
{
    Task<IEnumerable<UserClaimVM>> GetUserClaimsAsync(Guid userId);
    Task<bool> AddClaimToUserAsync(Guid userId, string claimType, string claimValue);
    Task<bool> RemoveClaimFromUserAsync(Guid userId, string claimType, string claimValue);
    Task<bool> UpdateUserClaimAsync(Guid userId, string oldClaimType, string oldClaimValue, string newClaimType, string newClaimValue);
}


