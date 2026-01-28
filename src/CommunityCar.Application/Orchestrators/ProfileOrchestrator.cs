using CommunityCar.Application.Common.Interfaces.Orchestrators;
using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;

namespace CommunityCar.Application.Orchestrators;

public class ProfileOrchestrator : IProfileOrchestrator
{
    private readonly IProfileService _profileService;
    private readonly IUserGalleryService _galleryService;
    private readonly IGamificationService _gamificationService;
    private readonly IAccountManagementService _accountManagementService;

    public ProfileOrchestrator(
        IProfileService profileService,
        IUserGalleryService galleryService,
        IGamificationService gamificationService,
        IAccountManagementService accountManagementService)
    {
        _profileService = profileService;
        _galleryService = galleryService;
        _gamificationService = gamificationService;
        _accountManagementService = accountManagementService;
    }

    public Task<ProfileVM?> GetProfileAsync(Guid userId)
        => _profileService.GetProfileAsync(userId);

    public async Task<ProfileSettingsVM?> GetProfileSettingsAsync(Guid userId)
    {
        var info = await _accountManagementService.GetAccountInfoAsync(userId);
        var stats = await GetProfileStatsAsync(userId);
        var profile = await GetProfileAsync(userId);
        
        if (profile == null) return null;

        return new ProfileSettingsVM
        {
            Id = profile.Id,
            UserName = profile.UserName,
            FullName = profile.FullName,
            Email = profile.Email,
            Bio = profile.Bio,
            City = profile.City,
            Country = profile.Country,
            ProfilePictureUrl = profile.ProfilePictureUrl,
            IsEmailConfirmed = profile.IsEmailConfirmed,
            IsActive = profile.IsActive
        };
    }

    public Task<bool> UpdateProfileAsync(UpdateProfileRequest request)
        => _profileService.UpdateProfileAsync(request);

    public Task<bool> UpdateProfilePictureAsync(Guid userId, string imageUrl)
        => _profileService.UpdateProfilePictureAsync(userId, imageUrl);

    public Task<bool> DeleteProfilePictureAsync(Guid userId)
        => _profileService.DeleteProfilePictureAsync(userId);

    public async Task<ProfileStatsVM> GetProfileStatsAsync(Guid userId)
        => await _profileService.GetProfileStatsAsync(userId);

    public Task<IEnumerable<UserGalleryItemVM>> GetUserGalleryAsync(Guid userId)
        => _galleryService.GetUserGalleryAsync(userId);

    public async Task<bool> UploadGalleryItemAsync(CommunityCar.Application.Common.Models.Profile.UploadImageRequest request)
        => (await _galleryService.UploadImageAsync(request)) != null;

    public Task<bool> ToggleGalleryItemVisibilityAsync(Guid itemId, Guid userId)
        => _galleryService.ToggleItemVisibilityAsync(userId, itemId);

    public Task<bool> DeleteGalleryItemAsync(Guid itemId, Guid userId)
        => _galleryService.DeleteImageAsync(userId, itemId);

    public Task<IEnumerable<UserBadgeVM>> GetUserBadgesAsync(Guid userId)
        => _gamificationService.GetUserBadgesAsync(userId);

    public Task<IEnumerable<UserAchievementVM>> GetUserAchievementsAsync(Guid userId)
        => _gamificationService.GetUserAchievementsAsync(userId);

    public async Task<UserGamificationStatsVM> GetGamificationStatsAsync(Guid userId)
    {
        var stats = await _gamificationService.GetUserStatsAsync(userId);
        return new UserGamificationStatsVM(); // Map accordingly
    }
}
