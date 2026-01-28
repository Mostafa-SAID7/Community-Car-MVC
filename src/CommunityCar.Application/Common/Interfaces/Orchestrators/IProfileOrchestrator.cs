using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;

namespace CommunityCar.Application.Common.Interfaces.Orchestrators;

public interface IProfileOrchestrator
{
    // Profile Management
    Task<ProfileVM?> GetProfileAsync(Guid userId);
    Task<ProfileSettingsVM?> GetProfileSettingsAsync(Guid userId);
    Task<bool> UpdateProfileAsync(UpdateProfileRequest request);
    Task<bool> UpdateProfilePictureAsync(Guid userId, string imageUrl);
    Task<bool> DeleteProfilePictureAsync(Guid userId);
    Task<ProfileStatsVM> GetProfileStatsAsync(Guid userId);
    
    // Gallery
    Task<IEnumerable<UserGalleryItemVM>> GetUserGalleryAsync(Guid userId);
    Task<bool> UploadGalleryItemAsync(UploadImageRequest request);
    Task<bool> ToggleGalleryItemVisibilityAsync(Guid itemId, Guid userId);
    Task<bool> DeleteGalleryItemAsync(Guid itemId, Guid userId);

    // Gamification
    Task<IEnumerable<UserBadgeVM>> GetUserBadgesAsync(Guid userId);
    Task<IEnumerable<UserAchievementVM>> GetUserAchievementsAsync(Guid userId);
    Task<UserGamificationStatsVM> GetGamificationStatsAsync(Guid userId);
}



