using CommunityCar.Application.Common.Interfaces.Repositories.Profile;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Account.Gallery;

/// <summary>
/// Service for gallery storage management
/// </summary>
public class GalleryStorageService : IGalleryStorageService
{
    private readonly IUserGalleryRepository _galleryRepository;
    private readonly ILogger<GalleryStorageService> _logger;

    // Configuration constants
    private const long MaxStoragePerUser = 100 * 1024 * 1024; // 100MB

    public GalleryStorageService(
        IUserGalleryRepository galleryRepository,
        ILogger<GalleryStorageService> logger)
    {
        _galleryRepository = galleryRepository;
        _logger = logger;
    }

    public async Task<int> GetImageCountAsync(Guid userId)
    {
        try
        {
            return await _galleryRepository.GetGalleryCountAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting image count for user {UserId}", userId);
            return 0;
        }
    }

    public async Task<long> GetTotalStorageUsedAsync(Guid userId)
    {
        try
        {
            // TODO: Implement actual storage calculation
            // This would require storing file sizes in the UserGallery entity
            // or querying the file storage service for each file
            
            var imageCount = await GetImageCountAsync(userId);
            var estimatedSize = imageCount * 500 * 1024; // Estimate 500KB per image
            
            _logger.LogDebug("Estimated storage used for user {UserId}: {Size} bytes", userId, estimatedSize);
            return estimatedSize;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating storage used for user {UserId}", userId);
            return 0;
        }
    }

    public async Task<bool> IsStorageLimitExceededAsync(Guid userId)
    {
        try
        {
            var storageUsed = await GetTotalStorageUsedAsync(userId);
            var isExceeded = storageUsed >= MaxStoragePerUser;
            
            if (isExceeded)
            {
                _logger.LogWarning("Storage limit exceeded for user {UserId}: {Used}/{Limit} bytes", 
                    userId, storageUsed, MaxStoragePerUser);
            }
            
            return isExceeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking storage limit for user {UserId}", userId);
            return true; // Err on the side of caution
        }
    }
}


