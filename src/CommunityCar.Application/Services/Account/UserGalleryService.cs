using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Services.Account.Gallery;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Account;

/// <summary>
/// Orchestrator service for user gallery and media management
/// </summary>
public class UserGalleryService : IUserGalleryService
{
    private readonly IGalleryManagementService _galleryManagementService;
    private readonly IImageOperationsService _imageOperationsService;
    private readonly IGalleryStorageService _galleryStorageService;
    private readonly IImageValidationService _imageValidationService;
    private readonly ILogger<UserGalleryService> _logger;

    public UserGalleryService(
        IGalleryManagementService galleryManagementService,
        IImageOperationsService imageOperationsService,
        IGalleryStorageService galleryStorageService,
        IImageValidationService imageValidationService,
        ILogger<UserGalleryService> logger)
    {
        _galleryManagementService = galleryManagementService;
        _imageOperationsService = imageOperationsService;
        _galleryStorageService = galleryStorageService;
        _imageValidationService = imageValidationService;
        _logger = logger;
    }

    #region Gallery Management - Delegate to GalleryManagementService

    public async Task<IEnumerable<UserGalleryItemVM>> GetUserGalleryAsync(Guid userId)
        => await _galleryManagementService.GetUserGalleryAsync(userId);

    public async Task<UserGalleryItemVM?> GetGalleryItemAsync(Guid userId, Guid imageId)
        => await _galleryManagementService.GetGalleryItemAsync(userId, imageId);

    public async Task<UserGalleryItemVM?> UploadImageAsync(UploadImageRequest request)
    {
        // Convert base64 string to byte array
        byte[] imageBytes;
        try
        {
            imageBytes = Convert.FromBase64String(request.ImageData);
        }
        catch (FormatException)
        {
            _logger.LogWarning("Invalid base64 image data for user {UserId}", request.UserId);
            return null;
        }

        // Validate before upload
        if (!await _imageValidationService.IsValidImageAsync(imageBytes))
        {
            _logger.LogWarning("Invalid image format for user {UserId}", request.UserId);
            return null;
        }

        if (!await _imageValidationService.IsImageSizeAllowedAsync(imageBytes.Length))
        {
            _logger.LogWarning("Image size too large for user {UserId}", request.UserId);
            return null;
        }

        if (await _galleryStorageService.IsStorageLimitExceededAsync(request.UserId))
        {
            _logger.LogWarning("Storage limit exceeded for user {UserId}", request.UserId);
            return null;
        }

        return await _galleryManagementService.UploadImageAsync(request);
    }

    public async Task<bool> DeleteImageAsync(Guid userId, Guid imageId)
        => await _galleryManagementService.DeleteImageAsync(userId, imageId);

    #endregion

    #region Image Operations - Delegate to ImageOperationsService

    public async Task<bool> SetAsProfilePictureAsync(Guid userId, Guid imageId)
        => await _imageOperationsService.SetAsProfilePictureAsync(userId, imageId);

    public async Task<bool> SetAsCoverImageAsync(Guid userId, Guid imageId)
        => await _imageOperationsService.SetAsCoverImageAsync(userId, imageId);

    public async Task<bool> UpdateImageCaptionAsync(Guid userId, Guid imageId, string caption)
        => await _imageOperationsService.UpdateImageCaptionAsync(userId, imageId, caption);

    public async Task<bool> ReorderImagesAsync(Guid userId, IEnumerable<Guid> imageIds)
        => await _imageOperationsService.ReorderImagesAsync(userId, imageIds);

    public async Task<bool> ToggleItemVisibilityAsync(Guid userId, Guid imageId)
        => await _imageOperationsService.ToggleItemVisibilityAsync(userId, imageId);

    public async Task<bool> DeleteGalleryItemAsync(Guid userId, Guid imageId)
        => await _galleryManagementService.DeleteImageAsync(userId, imageId);

    #endregion

    #region Gallery Statistics - Delegate to GalleryStorageService

    public async Task<int> GetImageCountAsync(Guid userId)
        => await _galleryStorageService.GetImageCountAsync(userId);

    public async Task<long> GetTotalStorageUsedAsync(Guid userId)
        => await _galleryStorageService.GetTotalStorageUsedAsync(userId);

    public async Task<bool> IsStorageLimitExceededAsync(Guid userId)
        => await _galleryStorageService.IsStorageLimitExceededAsync(userId);

    #endregion

    #region Image Validation - Delegate to ImageValidationService

    public async Task<bool> IsValidImageAsync(byte[] imageData)
        => await _imageValidationService.IsValidImageAsync(imageData);

    public async Task<bool> IsImageSizeAllowedAsync(long fileSize)
        => await _imageValidationService.IsImageSizeAllowedAsync(fileSize);

    public async Task<IEnumerable<string>> GetAllowedImageFormatsAsync()
        => await _imageValidationService.GetAllowedImageFormatsAsync();

    #endregion
}


