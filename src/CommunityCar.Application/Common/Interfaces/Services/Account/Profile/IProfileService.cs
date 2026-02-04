using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Features.Account.ViewModels.Core;
using CommunityCar.Application.Features.Account.ViewModels.Management;

namespace CommunityCar.Application.Common.Interfaces.Services.Account.Profile;

public interface IProfileService
{
    Task<Result<ProfileVM>> GetProfileAsync(Guid userId);
    Task<Result<ProfileVM>> GetProfileByUsernameAsync(string username);
    Task<Result<ProfileVM>> UpdateProfileAsync(Guid userId, UpdateProfileVM request);
    Task<Result<ProfileVM>> UpdateProfilePictureAsync(Guid userId, string imageUrl);
    Task<Result<ProfileVM>> UpdateCoverPhotoAsync(Guid userId, string imageUrl);
    Task<Result<List<ProfileVM>>> SearchProfilesAsync(string query, int page = 1, int pageSize = 20);
    Task<Result<ProfileStatsVM>> GetProfileStatsAsync(Guid userId);
    Task<Result<List<ProfileVM>>> GetSuggestedProfilesAsync(Guid userId, int count = 10);
    Task<Result> UpdatePrivacySettingsAsync(Guid userId, PrivacySettingsVM privacy);
    Task<Result<PrivacySettingsVM>> GetPrivacySettingsAsync(Guid userId);
}