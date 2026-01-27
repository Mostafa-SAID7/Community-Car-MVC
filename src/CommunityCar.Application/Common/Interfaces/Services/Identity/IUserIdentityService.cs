using CommunityCar.Application.Features.Identity.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Identity;

/// <summary>
/// Interface for user identity operations
/// </summary>
public interface IUserIdentityService
{
    Task<UserIdentityVM?> GetUserIdentityAsync(Guid userId);
    Task<IEnumerable<UserIdentityVM>> GetAllUsersAsync(int page = 1, int pageSize = 20);
    Task<bool> IsUserActiveAsync(Guid userId);
    Task<bool> LockUserAsync(Guid userId, string reason);
    Task<bool> UnlockUserAsync(Guid userId);
}


