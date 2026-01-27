using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Services.Account.Profile;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Account;

/// <summary>
/// Orchestrator service for profile management operations
/// </summary>
public class ProfileService : IProfileService
{
    private readonly IProfileDataService _profileDataService;
    private readonly IProfileStatisticsService _profileStatisticsService;
    private readonly IProfileValidationService _profileValidationService;
    private readonly ILogger<ProfileService> _logger;

    public ProfileService(
        IProfileDataService profileDataService,
        IProfileStatisticsService profileStatisticsService,
        IProfileValidationService profileValidationService,
        ILogger<ProfileService> logger)
    {
        _profileDataService = profileDataService;
        _profileStatisticsService = profileStatisticsService;
        _profileValidationService = profileValidationService;
        _logger = logger;
    }

    #region Profile Retrieval - Delegate to ProfileDataService

    public async Task<ProfileVM?> GetProfileAsync(Guid userId)
    {
        var profile = await _profileDataService.GetProfileAsync(userId);
        if (profile != null)
        {
            // Enhance with statistics
            var stats = await _profileStatisticsService.GetProfileStatsAsync(userId);
            profile.Stats = stats;
        }
        return profile;
    }

    public async Task<ProfileVM?> GetPublicProfileAsync(Guid userId)
        => await _profileDataService.GetPublicProfileAsync(userId);

    public async Task<IEnumerable<ProfileVM>> SearchProfilesAsync(string searchTerm, int page = 1, int pageSize = 20)
        => await _profileDataService.SearchProfilesAsync(searchTerm, page, pageSize);

    #endregion

    #region Profile Management - Delegate to ProfileDataService

    public async Task<bool> UpdateProfileAsync(UpdateProfileRequest request)
        => await _profileDataService.UpdateProfileAsync(request);

    #endregion

    #region Profile Statistics - Delegate to ProfileStatisticsService

    public async Task<ProfileStatsVM> GetProfileStatsAsync(Guid userId)
        => await _profileStatisticsService.GetProfileStatsAsync(userId);

    public async Task<bool> UpdateProfileStatsAsync(Guid userId)
        => await _profileStatisticsService.UpdateProfileStatsAsync(userId);

    #endregion

    #region Profile Image Management - Delegate to ProfileDataService

    public async Task<bool> UpdateProfilePictureAsync(Guid userId, string imageUrl)
        => await _profileDataService.UpdateProfilePictureAsync(userId, imageUrl);

    public async Task<bool> UpdateCoverImageAsync(Guid userId, string imageUrl)
        => await _profileDataService.UpdateCoverImageAsync(userId, imageUrl);

    public async Task<bool> RemoveProfilePictureAsync(Guid userId)
        => await _profileDataService.RemoveProfilePictureAsync(userId);

    public async Task<bool> RemoveCoverImageAsync(Guid userId)
        => await _profileDataService.RemoveCoverImageAsync(userId);

    public async Task<bool> DeleteProfilePictureAsync(Guid userId)
        => await _profileDataService.RemoveProfilePictureAsync(userId);

    #endregion

    #region Profile Settings

    public async Task<ProfileSettingsVM?> GetProfileSettingsAsync(Guid userId)
        => await _profileDataService.GetProfileSettingsAsync(userId);

    public async Task<bool> UpdatePrivacySettingsAsync(Guid userId, UpdatePrivacySettingsRequest request)
        => await _profileDataService.UpdatePrivacySettingsAsync(userId, request);

    public async Task<bool> UpdateNotificationSettingsAsync(Guid userId, UpdateNotificationSettingsRequest request)
        => await _profileDataService.UpdateNotificationSettingsAsync(userId, request);

    #endregion

    #region Profile Validation - Delegate to ProfileValidationService

    public async Task<bool> IsProfileCompleteAsync(Guid userId)
        => await _profileValidationService.IsProfileCompleteAsync(userId);

    public async Task<IEnumerable<string>> GetProfileCompletionSuggestionsAsync(Guid userId)
        => await _profileValidationService.GetProfileCompletionSuggestionsAsync(userId);

    public async Task<bool> IsUsernameAvailableAsync(string username, Guid? excludeUserId = null)
        => await _profileValidationService.IsUsernameAvailableAsync(username, excludeUserId);

    public async Task<bool> IsEmailAvailableAsync(string email, Guid? excludeUserId = null)
        => await _profileValidationService.IsEmailAvailableAsync(email, excludeUserId);

    #endregion
}


