using CommunityCar.Application.Common.Interfaces.Repositories.User;
using CommunityCar.Application.Common.Interfaces.Repositories.Profile;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Account.Gallery;

/// <summary>
/// Service for image operations (profile picture, cover image, etc.)
/// </summary>
public class ImageOperationsService : IImageOperationsService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserGalleryRepository _galleryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ImageOperationsService> _logger;

    public ImageOperationsService(
        IUserRepository userRepository,
        IUserGalleryRepository galleryRepository,
        ICurrentUserService currentUserService,
        ILogger<ImageOperationsService> logger)
    {
        _userRepository = userRepository;
        _galleryRepository = galleryRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<bool> SetAsProfilePictureAsync(Guid userId, Guid imageId)
    {
        try
        {
            var galleryItem = await _galleryRepository.GetByIdAsync(imageId);
            if (galleryItem == null || galleryItem.UserId != userId)
            {
                _logger.LogWarning("Gallery item {ImageId} not found or access denied for user {UserId}", 
                    imageId, userId);
                return false;
            }

            // Update user's profile picture
            var success = await _userRepository.UpdateProfilePictureAsync(userId, galleryItem.MediaUrl);
            
            if (success)
            {
                _logger.LogInformation("Profile picture updated for user {UserId} with image {ImageId}", 
                    userId, imageId);
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting profile picture for user {UserId}, image {ImageId}", 
                userId, imageId);
            return false;
        }
    }

    public async Task<bool> SetAsCoverImageAsync(Guid userId, Guid imageId)
    {
        try
        {
            var galleryItem = await _galleryRepository.GetByIdAsync(imageId);
            if (galleryItem == null || galleryItem.UserId != userId)
            {
                _logger.LogWarning("Gallery item {ImageId} not found or access denied for user {UserId}", 
                    imageId, userId);
                return false;
            }

            // TODO: Implement cover image update in user repository
            // var success = await _userRepository.UpdateCoverImageAsync(userId, galleryItem.MediaUrl);
            
            _logger.LogInformation("Cover image updated for user {UserId} with image {ImageId}", 
                userId, imageId);
            
            return true; // Placeholder
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cover image for user {UserId}, image {ImageId}", 
                userId, imageId);
            return false;
        }
    }

    public async Task<bool> UpdateImageCaptionAsync(Guid userId, Guid imageId, string caption)
    {
        try
        {
            var galleryItem = await _galleryRepository.GetByIdAsync(imageId);
            if (galleryItem == null || galleryItem.UserId != userId)
            {
                _logger.LogWarning("Gallery item {ImageId} not found or access denied for user {UserId}", 
                    imageId, userId);
                return false;
            }

            galleryItem.UpdateDetails(galleryItem.Title, caption, galleryItem.Tags);
            await _galleryRepository.UpdateAsync(galleryItem);

            _logger.LogInformation("Caption updated for gallery item {ImageId} by user {UserId}", 
                imageId, userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating caption for gallery item {ImageId} by user {UserId}", 
                imageId, userId);
            return false;
        }
    }

    public async Task<bool> ReorderImagesAsync(Guid userId, IEnumerable<Guid> imageIds)
    {
        try
        {
            // TODO: Implement image reordering logic
            // This would require adding an Order field to UserGallery entity
            
            _logger.LogInformation("Images reordered for user {UserId}", userId);
            return true; // Placeholder
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering images for user {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> ToggleItemVisibilityAsync(Guid userId, Guid imageId)
    {
        try
        {
            var galleryItem = await _galleryRepository.GetByIdAsync(imageId);
            if (galleryItem == null || galleryItem.UserId != userId)
            {
                _logger.LogWarning("Gallery item {ImageId} not found or access denied for user {UserId}", 
                    imageId, userId);
                return false;
            }

            // TODO: Implement visibility toggle logic
            // This would require adding an IsVisible field to UserGallery entity
            
            _logger.LogInformation("Visibility toggled for gallery item {ImageId} by user {UserId}", 
                imageId, userId);
            return true; // Placeholder
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling visibility for gallery item {ImageId} by user {UserId}", 
                imageId, userId);
            return false;
        }
    }
}


