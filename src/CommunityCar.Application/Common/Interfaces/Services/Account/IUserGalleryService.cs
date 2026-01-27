using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;

using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;

using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;

namespace CommunityCar.Application.Common.Interfaces.Services.Account;

/// <summary>
/// Interface for user gallery and media management
/// </summary>
public interface IUserGalleryService
{
    // Gallery Management
    Task<IEnumerable<UserGalleryItemVM>> GetUserGalleryAsync(Guid userId);
    Task<UserGalleryItemVM?> GetGalleryItemAsync(Guid userId, Guid imageId);
    Task<UserGalleryItemVM?> UploadImageAsync(UploadImageRequest request);
    Task<bool> DeleteImageAsync(Guid userId, Guid imageId);

    // Image Operations
    Task<bool> SetAsProfilePictureAsync(Guid userId, Guid imageId);
    Task<bool> SetAsCoverImageAsync(Guid userId, Guid imageId);
    Task<bool> UpdateImageCaptionAsync(Guid userId, Guid imageId, string caption);
    Task<bool> ReorderImagesAsync(Guid userId, IEnumerable<Guid> imageIds);
    Task<bool> ToggleItemVisibilityAsync(Guid userId, Guid imageId);
    Task<bool> DeleteGalleryItemAsync(Guid userId, Guid imageId);

    // Gallery Statistics
    Task<int> GetImageCountAsync(Guid userId);
    Task<long> GetTotalStorageUsedAsync(Guid userId);
    Task<bool> IsStorageLimitExceededAsync(Guid userId);

    // Image Validation
    Task<bool> IsValidImageAsync(byte[] imageData);
    Task<bool> IsImageSizeAllowedAsync(long fileSize);
    Task<IEnumerable<string>> GetAllowedImageFormatsAsync();
}


