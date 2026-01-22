using CommunityCar.Application.Features.Profile.DTOs;
using CommunityCar.Application.Features.Profile.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Profile;

public interface IProfileService
{
    // Profile management
    Task<ProfileVM?> GetProfileAsync(Guid userId);
    Task<ProfileSettingsVM?> GetProfileSettingsAsync(Guid userId);
    Task<bool> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
    Task<bool> UpdateProfilePictureAsync(Guid userId, Stream imageStream, string fileName);
    Task<bool> DeleteProfilePictureAsync(Guid userId);
    
    // Statistics and activity
    Task<ProfileStatsVM> GetProfileStatsAsync(Guid userId);
}