using CommunityCar.Application.Features.Account.ViewModels.Media;

namespace CommunityCar.Application.Common.Interfaces.Services.Account.Media;

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
    Task<bool> ToggleItemVisibilityAsync(Guid userId, Guid imageId);
    Task<bool> ReorderImagesAsync(Guid userId, IEnumerable<Guid> imageIds);
    Task<bool> DeleteGalleryItemAsync(Guid userId, Guid imageId);

    // Statistics & Validation
    Task<int> GetImageCountAsync(Guid userId);
    Task<long> GetTotalStorageUsedAsync(Guid userId);
    Task<bool> IsStorageLimitExceededAsync(Guid userId);
    Task<bool> IsValidImageAsync(byte[] imageData);
    Task<bool> IsImageSizeAllowedAsync(long fileSize);
    Task<IEnumerable<string>> GetAllowedImageFormatsAsync();
}