using CommunityCar.Application.Common.Interfaces.Orchestrators;
using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;

namespace CommunityCar.Application.Orchestrators;

public class ProfileOrchestrator : IProfileOrchestrator
{
    private readonly IProfileService _profileService;
    private readonly IUserGalleryService _galleryService;
    private readonly IGamificationService _gamificationService;

    public ProfileOrchestrator(
        IProfileService profileService,
        IUserGalleryService galleryService,
        IGamificationService gamificationService)
    {
        _profileService = profileService;
        _galleryService = galleryService;
        _gamificationService = gamificationService;
    }

    public Task<ProfileVM?> GetProfileAsync(Guid userId)
        => _profileService.GetProfileAsync(userId);

    public Task<ProfileSettingsVM?> GetProfileSettingsAsync(Guid userId)
        => _profileService.GetProfileSettingsAsync(userId);

    public Task<bool> UpdateProfileAsync(UpdateProfileRequest request)
        => _profileService.UpdateProfileAsync(request);

    public Task<bool> UpdateProfilePictureAsync(Guid userId, string imageUrl)
        => _profileService.UpdateProfilePictureAsync(userId, imageUrl);

    public Task<bool> DeleteProfilePictureAsync(Guid userId)
        => _profileService.DeleteProfilePictureAsync(userId);

    public Task<ProfileStatsVM> GetProfileStatsAsync(Guid userId)
        => _profileService.GetProfileStatsAsync(userId);

    public Task<bool> UpdatePrivacySettingsAsync(Guid userId, UpdatePrivacySettingsRequest request)
        => _profileService.UpdatePrivacySettingsAsync(userId, request);

    public Task<bool> UpdateNotificationSettingsAsync(Guid userId, UpdateNotificationSettingsRequest request)
        => _profileService.UpdateNotificationSettingsAsync(userId, request);

    public Task<IEnumerable<UserGalleryItemVM>> GetUserGalleryAsync(Guid userId)
        => _galleryService.GetUserGalleryAsync(userId);

    public Task<bool> UploadGalleryItemAsync(UploadImageRequest request)
        => _galleryService.UploadImageAsync(request);

    public Task<bool> ToggleGalleryItemVisibilityAsync(Guid itemId, Guid userId)
        => _galleryService.ToggleVisibilityAsync(itemId, userId);

    public Task<bool> DeleteGalleryItemAsync(Guid itemId, Guid userId)
        => _galleryService.DeleteImageAsync(itemId, userId);

    public Task<IEnumerable<UserBadgeVM>> GetUserBadgesAsync(Guid userId)
        => _gamificationService.GetUserBadgesAsync(userId);

    public Task<IEnumerable<UserAchievementVM>> GetUserAchievementsAsync(Guid userId)
        => _gamificationService.GetUserAchievementsAsync(userId);

    public Task<UserGamificationStatsVM> GetGamificationStatsAsync(Guid userId)
        => _gamificationService.GetUserStatsAsync(userId);
}



