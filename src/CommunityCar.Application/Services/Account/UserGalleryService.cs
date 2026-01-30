using CommunityCar.Application.Common.Interfaces.Repositories.Account;
using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Interfaces.Services.Storage;
using CommunityCar.Application.Features.Account.ViewModels.Media;
using CommunityCar.Application.Features.Account.ViewModels.Core;
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
    private readonly IProfileService _profileService;
    private readonly ILogger<UserGalleryService> _logger;

    public UserGalleryService(
        IUserRepository userRepository,
        IUserGalleryRepository galleryRepository,
        IFileStorageService fileStorageService,
        ICurrentUserService currentUserService,
        IProfileService profileService,
        ILogger<UserGalleryService> logger)
    {
        _userRepository = userRepository;
        _galleryRepository = galleryRepository;
        _fileStorageService = fileStorageService;
        _currentUserService = currentUserService;
        _profileService = profileService;
        _logger = logger;
    }

    #region Gallery Management

    public async Task<IEnumerable<UserGalleryItemVM>> GetUserGalleryAsync(Guid userId)
    {
        var currentUserId = Guid.TryParse(_currentUserService.UserId, out var id) ? id : (Guid?)null;
        var galleryItems = (currentUserId != userId) 
            ? await _galleryRepository.GetPublicGalleryAsync(userId)
            : await _galleryRepository.GetUserGalleryAsync(userId);
        
        return galleryItems.Select(item => new UserGalleryItemVM
        {
            Id = item.Id,
            UserId = item.UserId,
            ImageUrl = item.MediaUrl,
            ThumbnailUrl = item.ThumbnailUrl ?? item.MediaUrl,
            Caption = item.Description ?? item.Title,
            CreatedAt = item.UploadedAt,
            ViewCount = item.ViewCount,
            // LikeCount = item.LikeCount, // Not in VM
            IsPublic = item.IsPublic,
            // IsFeatured = item.IsFeatured // Not in VM
        });
    }

    public async Task<UserGalleryItemVM?> GetGalleryItemAsync(Guid userId, Guid imageId)
    {
        var currentUserId = Guid.TryParse(_currentUserService.UserId, out var id) ? id : Guid.Empty;
        var item = await _galleryRepository.GetGalleryItemAsync(imageId, currentUserId);
        if (item == null || item.UserId != userId) return null;

        return new UserGalleryItemVM
        {
            Id = item.Id,
            UserId = item.UserId,
            ImageUrl = item.MediaUrl,
            ThumbnailUrl = item.ThumbnailUrl ?? item.MediaUrl,
            Caption = item.Description ?? item.Title,
            CreatedAt = item.UploadedAt,
            IsPublic = item.IsPublic
        };
    }

    public async Task<UserGalleryItemVM?> UploadImageAsync(UploadImageRequest request)
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

            return new UserGalleryItemVM { Id = item.Id, UserId = item.UserId, ImageUrl = item.MediaUrl, CreatedAt = item.UploadedAt };
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
        return await _profileService.UpdateProfilePictureAsync(userId, item.MediaUrl);
    }

    public async Task<bool> SetAsCoverImageAsync(Guid userId, Guid imageId)
    {
        var item = await _galleryRepository.GetByIdAsync(imageId);
        if (item == null || item.UserId != userId) return false;
        return await _profileService.UpdateCoverImageAsync(userId, item.MediaUrl);
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

    public async Task<int> GetImageCountAsync(Guid userId) => (await _galleryRepository.GetUserGalleryAsync(userId, 1, int.MaxValue)).Count();
    public Task<long> GetTotalStorageUsedAsync(Guid userId) => Task.FromResult(0L);
    public Task<bool> IsStorageLimitExceededAsync(Guid userId) => Task.FromResult(false);
    public Task<bool> IsValidImageAsync(byte[] imageData) => Task.FromResult(true);
    public Task<bool> IsImageSizeAllowedAsync(long fileSize) => Task.FromResult(fileSize < 5 * 1024 * 1024);
    public Task<IEnumerable<string>> GetAllowedImageFormatsAsync() => Task.FromResult<IEnumerable<string>>(new[] { ".jpg", ".png", ".webp" });

    #endregion
}
