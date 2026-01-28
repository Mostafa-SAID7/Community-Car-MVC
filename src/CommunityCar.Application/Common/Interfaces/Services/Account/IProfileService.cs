using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;

namespace CommunityCar.Application.Common.Interfaces.Services.Account;

/// <summary>
/// Unified interface for profile management operations
/// </summary>
public interface IProfileService
{
    // Profile Retrieval
    Task<ProfileVM?> GetProfileAsync(Guid userId);
    Task<ProfileVM?> GetPublicProfileAsync(Guid userId);
    Task<IEnumerable<ProfileVM>> SearchProfilesAsync(string searchTerm, int page = 1, int pageSize = 20);

    // Profile Management
    Task<bool> UpdateProfileAsync(UpdateProfileRequest request);
    Task<bool> UpdateProfilePictureAsync(Guid userId, string imageUrl);
    Task<bool> UpdateCoverImageAsync(Guid userId, string imageUrl);
    Task<bool> RemoveProfilePictureAsync(Guid userId);
    Task<bool> RemoveCoverImageAsync(Guid userId);
    Task<bool> DeleteProfilePictureAsync(Guid userId);

    // Profile Statistics
    Task<ProfileStatsVM> GetProfileStatsAsync(Guid userId);
    Task<bool> UpdateProfileStatsAsync(Guid userId);

    // Profile Validation
    Task<bool> IsProfileCompleteAsync(Guid userId);
    Task<IEnumerable<string>> GetProfileCompletionSuggestionsAsync(Guid userId);
}
