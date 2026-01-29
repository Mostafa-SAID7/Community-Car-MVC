using CommunityCar.Application.Common.Interfaces.Repositories.User;
using CommunityCar.Application.Common.Interfaces.Repositories.Account;
using CommunityCar.Application.Common.Interfaces.Repositories.Profile;
using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Interfaces.Services.Storage;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Features.Account.ViewModels.Media;
using CommunityCar.Domain.Entities.Account.Media;
using CommunityCar.Domain.Enums.Account;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CommunityCar.Application.Services.Account;

public class UserGalleryService : IUserGalleryService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserGalleryRepository _galleryRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UserGalleryService> _logger;

    public UserGalleryService(
        IUserRepository userRepository,
        IUserGalleryRepository galleryRepository,
        IFileStorageService fileStorageService,
        ICurrentUserService currentUserService,
        ILogger<UserGalleryService> logger)
    {
        _userRepository = userRepository;
        _galleryRepository = galleryRepository;
        _fileStorageService = fileStorageService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    #region Gallery Management

    public async Task<IEnumerable<UserGalleryVM>> GetUserGalleryAsync(Guid userId)
    {
        var currentUserId = Guid.TryParse(_currentUserService.UserId, out var id) ? id : (Guid?)null;
        var galleryItems = await _galleryRepository.GetUserGalleryAsync(userId, currentUserId != userId);
        
        return galleryItems.Select(item => new UserGalleryVM
        {
            Id = item.Id,
            UserId = item.UserId,
            ImageUrl = item.MediaUrl,
            ThumbnailUrl = item.ThumbnailUrl ?? item.MediaUrl,
            Caption = item.Description ?? item.Title,
            UploadedAt = item.UploadedAt,
            ViewCount = item.ViewCount,
            LikeCount = item.LikeCount,
            IsPublic = item.IsPublic,
            IsFeatured = item.IsFeatured
        });
    }

    public async Task<UserGalleryVM?> GetGalleryItemAsync(Guid userId, Guid imageId)
    {
        var currentUserId = Guid.TryParse(_currentUserService.UserId, out var id) ? id : (Guid?)null;
        var item = await _galleryRepository.GetGalleryItemAsync(imageId, currentUserId);
        if (item == null || item.UserId != userId) return null;

        return new UserGalleryVM
        {
            Id = item.Id,
            UserId = item.UserId,
            ImageUrl = item.MediaUrl,
            ThumbnailUrl = item.ThumbnailUrl ?? item.MediaUrl,
            Caption = item.Description ?? item.Title,
            UploadedAt = item.UploadedAt,
            IsPublic = item.IsPublic
        };
    }

    public async Task<UserGalleryVM?> UploadImageAsync(CommunityCar.Application.Common.Models.Profile.UploadImageRequest request)
    {
        try
        {
            var imageBytes = Convert.FromBase64String(request.ImageData);
            if (imageBytes.Length > 5 * 1024 * 1024) return null; // 5MB limit

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.FileName)}";
            var filePath = $"gallery/{request.UserId}/{fileName}";
            
            using var stream = new MemoryStream(imageBytes);
            var url = await _fileStorageService.UploadFileAsync(stream, filePath, request.ContentType);
            if (string.IsNullOrEmpty(url)) return null;

            var item = new UserGallery(request.UserId, request.Caption ?? "Untitled", url, MediaType.Image, request.IsPublic);
            await _galleryRepository.AddAsync(item);

            return new UserGalleryVM { Id = item.Id, UserId = item.UserId, ImageUrl = item.MediaUrl, UploadedAt = item.UploadedAt };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gallery upload failed for user {UserId}", request.UserId);
            return null;
        }
    }

    public async Task<bool> DeleteImageAsync(Guid userId, Guid imageId)
    {
        var item = await _galleryRepository.GetByIdAsync(imageId);
        if (item == null || item.UserId != userId) return false;

        await _fileStorageService.DeleteFileAsync(item.MediaUrl);
        await _galleryRepository.DeleteAsync(item);
        return true;
    }

    #endregion

    #region Image Operations

    public async Task<bool> SetAsProfilePictureAsync(Guid userId, Guid imageId)
    {
        var item = await _galleryRepository.GetByIdAsync(imageId);
        if (item == null || item.UserId != userId) return false;
        return await _userRepository.UpdateProfilePictureAsync(userId, item.MediaUrl);
    }

    public async Task<bool> SetAsCoverImageAsync(Guid userId, Guid imageId)
    {
        var item = await _galleryRepository.GetByIdAsync(imageId);
        if (item == null || item.UserId != userId) return false;
        return await _userRepository.UpdateCoverImageAsync(userId, item.MediaUrl);
    }

    public async Task<bool> UpdateImageCaptionAsync(Guid userId, Guid imageId, string caption)
    {
        var item = await _galleryRepository.GetByIdAsync(imageId);
        if (item == null || item.UserId != userId) return false;
        item.UpdateDetails(item.Title, caption, item.Tags);
        await _galleryRepository.UpdateAsync(item);
        return true;
    }

    public async Task<bool> ToggleItemVisibilityAsync(Guid userId, Guid imageId)
    {
        var item = await _galleryRepository.GetByIdAsync(imageId);
        if (item == null || item.UserId != userId) return false;
        
        // Toggle visibility
        item.ToggleVisibility();
        await _galleryRepository.UpdateAsync(item);
        
        _logger.LogInformation("Toggled visibility for gallery item {ItemId} to {IsPublic}", imageId, item.IsPublic);
        return true;
    }

    public Task<bool> ReorderImagesAsync(Guid userId, IEnumerable<Guid> imageIds) => Task.FromResult(true);
    public Task<bool> DeleteGalleryItemAsync(Guid userId, Guid imageId) => DeleteImageAsync(userId, imageId);

    #endregion

    #region Statistics & Validation

    public async Task<int> GetImageCountAsync(Guid userId) => (await _galleryRepository.GetUserGalleryAsync(userId, false)).Count();
    public Task<long> GetTotalStorageUsedAsync(Guid userId) => Task.FromResult(0L);
    public Task<bool> IsStorageLimitExceededAsync(Guid userId) => Task.FromResult(false);
    public Task<bool> IsValidImageAsync(byte[] imageData) => Task.FromResult(true);
    public Task<bool> IsImageSizeAllowedAsync(long fileSize) => Task.FromResult(fileSize < 5 * 1024 * 1024);
    public Task<IEnumerable<string>> GetAllowedImageFormatsAsync() => Task.FromResult<IEnumerable<string>>(new[] { ".jpg", ".png", ".webp" });

    #endregion
}
