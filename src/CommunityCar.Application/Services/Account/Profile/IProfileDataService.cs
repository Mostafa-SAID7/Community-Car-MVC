using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;

namespace CommunityCar.Application.Services.Account.Profile;

/// <summary>
/// Interface for profile CRUD operations
/// </summary>
public interface IProfileDataService
{
    Task<ProfileVM?> GetProfileAsync(Guid userId);
    Task<ProfileVM?> GetPublicProfileAsync(Guid userId);
    Task<IEnumerable<ProfileVM>> SearchProfilesAsync(string searchTerm, int page = 1, int pageSize = 20);
    Task<bool> UpdateProfileAsync(UpdateProfileRequest request);

    // Profile Image Management
    Task<bool> UpdateProfilePictureAsync(Guid userId, string imageUrl);
    Task<bool> UpdateCoverImageAsync(Guid userId, string imageUrl);
    Task<bool> RemoveProfilePictureAsync(Guid userId);
    Task<bool> RemoveCoverImageAsync(Guid userId);

    // Profile Settings
    Task<ProfileSettingsVM?> GetProfileSettingsAsync(Guid userId);
    Task<bool> UpdatePrivacySettingsAsync(Guid userId, UpdatePrivacySettingsRequest request);
    Task<bool> UpdateNotificationSettingsAsync(Guid userId, UpdateNotificationSettingsRequest request);
}


