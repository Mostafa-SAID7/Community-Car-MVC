using CommunityCar.Application.Features.Account.ViewModels.Core;

namespace CommunityCar.Web.Areas.Identity.Interfaces.Services.Profile;

public interface IProfileService
{
    // Retrieval
    Task<ProfileVM?> GetProfileAsync(Guid userId);
    Task<ProfileVM?> GetProfileBySlugAsync(string slug);
    Task<ProfileVM?> GetPublicProfileAsync(Guid userId);
    Task<IEnumerable<ProfileVM>> SearchProfilesAsync(string searchTerm, int page = 1, int pageSize = 20);

    // Management
    Task<bool> UpdateProfileAsync(UpdateProfileRequest request);
    Task<bool> UpdateProfilePictureAsync(Guid userId, string imageUrl);
    Task<bool> UpdateCoverImageAsync(Guid userId, string imageUrl);
    Task<bool> RemoveProfilePictureAsync(Guid userId);
    Task<bool> RemoveCoverImageAsync(Guid userId);
    Task<bool> DeleteProfilePictureAsync(Guid userId);

    // Statistics & Validation
    Task<ProfileStatsVM> GetProfileStatsAsync(Guid userId);
    Task<bool> UpdateProfileStatsAsync(Guid userId);
    Task<bool> IsProfileCompleteAsync(Guid userId);
    Task<IEnumerable<string>> GetProfileCompletionSuggestionsAsync(Guid userId);
}

