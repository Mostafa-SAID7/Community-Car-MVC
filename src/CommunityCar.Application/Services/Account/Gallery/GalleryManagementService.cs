using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Common.Interfaces.Repositories.User;
using CommunityCar.Application.Common.Interfaces.Repositories.Profile;
using CommunityCar.Application.Common.Interfaces.Services.Storage;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Domain.Entities.Profile;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CommunityCar.Application.Services.Account.Gallery;

/// <summary>
/// Service for gallery CRUD operations
/// </summary>
public class GalleryManagementService : IGalleryManagementService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserGalleryRepository _galleryRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<GalleryManagementService> _logger;

    public GalleryManagementService(
        IUserRepository userRepository,
        IUserGalleryRepository galleryRepository,
        IFileStorageService fileStorageService,
        ICurrentUserService currentUserService,
        ILogger<GalleryManagementService> logger)
    {
        _userRepository = userRepository;
        _galleryRepository = galleryRepository;
        _fileStorageService = fileStorageService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<IEnumerable<UserGalleryItemVM>> GetUserGalleryAsync(Guid userId)
    {
        try
        {
            var currentUserId = Guid.TryParse(_currentUserService.UserId, out var id) ? id : (Guid?)null;
            var isOwner = currentUserId == userId;
            
            var galleryItems = await _galleryRepository.GetUserGalleryAsync(userId, !isOwner);
            
            return galleryItems.Select(item => new UserGalleryItemVM
            {
                Id = item.Id,
                UserId = item.UserId,
                ImageUrl = item.MediaUrl,
                ThumbnailUrl = item.ThumbnailUrl ?? item.MediaUrl,
                Caption = item.Description ?? item.Title,
                UploadedAt = item.UploadedAt,
                FileSize = 0, // TODO: Store file size in entity
                IsProfilePicture = false, // TODO: Check if this is current profile picture
                IsCoverImage = false, // TODO: Check if this is current cover image
                ViewCount = item.ViewCount,
                LikeCount = item.LikeCount,
                IsPublic = item.IsPublic,
                IsFeatured = item.IsFeatured,
                Tags = string.Join(",", ParseTags(item.Tags))
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting gallery for user {UserId}", userId);
            return Enumerable.Empty<UserGalleryItemVM>();
        }
    }

    public async Task<UserGalleryItemVM?> GetGalleryItemAsync(Guid userId, Guid imageId)
    {
        try
        {
            var currentUserId = Guid.TryParse(_currentUserService.UserId, out var id) ? id : (Guid?)null;
            var galleryItem = await _galleryRepository.GetGalleryItemAsync(imageId, currentUserId);
            
            if (galleryItem == null || galleryItem.UserId != userId)
                return null;

            return new UserGalleryItemVM
            {
                Id = galleryItem.Id,
                UserId = galleryItem.UserId,
                ImageUrl = galleryItem.MediaUrl,
                ThumbnailUrl = galleryItem.ThumbnailUrl ?? galleryItem.MediaUrl,
                Caption = galleryItem.Description ?? galleryItem.Title,
                UploadedAt = galleryItem.UploadedAt,
                FileSize = 0, // TODO: Store file size in entity
                IsProfilePicture = false, // TODO: Check if this is current profile picture
                IsCoverImage = false, // TODO: Check if this is current cover image
                ViewCount = galleryItem.ViewCount,
                LikeCount = galleryItem.LikeCount,
                IsPublic = galleryItem.IsPublic,
                IsFeatured = galleryItem.IsFeatured,
                Tags = string.Join(",", ParseTags(galleryItem.Tags))
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting gallery item {ImageId} for user {UserId}", imageId, userId);
            return null;
        }
    }

    public async Task<UserGalleryItemVM?> UploadImageAsync(UploadImageRequest request)
    {
        try
        {
            // Convert base64 to byte array and then to stream
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

            // Upload file to storage
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.FileName)}";
            var filePath = $"gallery/{request.UserId}/{fileName}";
            
            using var imageStream = new MemoryStream(imageBytes);
            var uploadResult = await _fileStorageService.UploadFileAsync(
                imageStream, 
                filePath, 
                request.ContentType);

            if (string.IsNullOrEmpty(uploadResult))
            {
                _logger.LogWarning("Failed to upload image for user {UserId}", request.UserId);
                return null;
            }

            // Create gallery entity
            var galleryItem = new UserGallery(
                request.UserId,
                request.Caption ?? "Untitled",
                uploadResult,
                MediaType.Image,
                request.IsPublic);

            // Save to database
            await _galleryRepository.AddAsync(galleryItem);

            _logger.LogInformation("Image uploaded successfully for user {UserId}, item {ItemId}", 
                request.UserId, galleryItem.Id);

            return new UserGalleryItemVM
            {
                Id = galleryItem.Id,
                UserId = galleryItem.UserId,
                ImageUrl = galleryItem.MediaUrl,
                ThumbnailUrl = galleryItem.ThumbnailUrl ?? galleryItem.MediaUrl,
                Caption = galleryItem.Description ?? galleryItem.Title,
                UploadedAt = galleryItem.UploadedAt,
                FileSize = request.ImageData.Length,
                IsProfilePicture = false,
                IsCoverImage = false,
                ViewCount = galleryItem.ViewCount,
                LikeCount = galleryItem.LikeCount,
                IsPublic = galleryItem.IsPublic,
                IsFeatured = galleryItem.IsFeatured,
                Tags = string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image for user {UserId}", request.UserId);
            return null;
        }
    }

    public async Task<bool> DeleteImageAsync(Guid userId, Guid imageId)
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

            // Delete from storage
            await _fileStorageService.DeleteFileAsync(galleryItem.MediaUrl);

            // Delete from database
            await _galleryRepository.DeleteAsync(galleryItem);

            _logger.LogInformation("Gallery item {ImageId} deleted for user {UserId}", imageId, userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting gallery item {ImageId} for user {UserId}", imageId, userId);
            return false;
        }
    }

    private List<string> ParseTags(string? tagsJson)
    {
        if (string.IsNullOrEmpty(tagsJson))
            return new List<string>();

        try
        {
            var tags = JsonSerializer.Deserialize<string[]>(tagsJson);
            return tags?.ToList() ?? new List<string>();
        }
        catch (JsonException)
        {
            return new List<string>();
        }
    }
}


