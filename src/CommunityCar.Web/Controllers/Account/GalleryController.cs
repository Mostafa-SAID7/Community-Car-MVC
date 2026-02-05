using CommunityCar.Web.Areas.Identity.Interfaces.Services.Media;
using CommunityCar.Web.Areas.Identity.Interfaces.Services.Core;
using CommunityCar.Application.Features.Account.ViewModels.Media;
using CommunityCar.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Account;

[Route("profile/gallery")]
[Authorize]
public class GalleryController : Controller
{
    private readonly IUserGalleryService _galleryService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<GalleryController> _logger;

    public GalleryController(
        IUserGalleryService galleryService,
        ICurrentUserService currentUserService,
        ILogger<GalleryController> logger)
    {
        _galleryService = galleryService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            return RedirectToAction("Login", "Account", new { area = "" });

        var galleryItems = await _galleryService.GetUserGalleryAsync(userId) ?? new List<UserGalleryItemVM>();
        var imageCount = await _galleryService.GetImageCountAsync(userId);

        // Use ViewBagHelper to set consistent data
        ViewBagHelper.SetPageMetadata(ViewBag, "Gallery", "Manage your photo gallery");
        ViewBagHelper.SetUserStats(ViewBag, new { ImageCount = imageCount });
        ViewBag.UserId = userId.ToString();

        return View("~/Views/Account/Profile/Gallery.cshtml", galleryItems);
    }

    [HttpPost("upload")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(IFormFile file, string? caption, bool isPublic = true)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                _logger.LogWarning("Upload attempt with invalid user ID: {UserId}", _currentUserService.UserId);
                return JsonResponseHelper.AuthenticationRequired();
            }

            // Use FileHelper for comprehensive file validation
            var imageValidation = FileHelper.ValidateImageFile(file);
            if (!imageValidation.IsValid)
            {
                _logger.LogWarning("Upload attempt with invalid image file by user {UserId}: {Error}", userId, imageValidation.ErrorMessage);
                return JsonResponseHelper.Error(imageValidation.ErrorMessage);
            }

            // Additional file size validation using FileHelper
            var maxSize = FileHelper.MaxFileSizes["image"]; // 5MB
            var sizeValidation = ValidationHelper.ValidateFileUpload(file, FileHelper.ImageExtensions, maxSize);
            if (!sizeValidation.IsValid)
            {
                _logger.LogWarning("Upload attempt failed validation by user {UserId}: {Errors}", userId, string.Join(", ", sizeValidation.Errors));
                return JsonResponseHelper.Error(string.Join(", ", sizeValidation.Errors));
            }

            // Generate secure filename using SecurityHelper
            var secureFileName = SecurityHelper.GenerateSecureFileName(file.FileName);
            
            _logger.LogInformation("Starting image upload for user {UserId}, file: {FileName}, size: {FileSize}, type: {ContentType}", 
                userId, secureFileName, FileHelper.FormatFileSize(file.Length), file.ContentType);

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            var imageData = Convert.ToBase64String(stream.ToArray());

            var request = new UploadImageRequest
            {
                UserId = userId,
                ImageData = imageData,
                FileName = secureFileName,
                ContentType = file.ContentType,
                Caption = caption,
                IsPublic = isPublic
            };

            var result = await _galleryService.UploadImageAsync(request);
            if (result != null)
            {
                _logger.LogInformation("Image uploaded successfully for user {UserId}, imageId: {ImageId}", userId, result.Id);
                
                // Get the complete gallery item data for frontend update
                var galleryItem = await _galleryService.GetGalleryItemAsync(userId, result.Id);
                if (galleryItem != null)
                {
                    return JsonResponseHelper.Success("Image uploaded successfully", new {
                        imageId = result.Id,
                        galleryItem = new {
                            id = galleryItem.Id,
                            imageUrl = galleryItem.ImageUrl,
                            thumbnailUrl = galleryItem.ThumbnailUrl,
                            caption = galleryItem.Caption,
                            isPublic = galleryItem.IsPublic,
                            timeAgo = galleryItem.TimeAgo,
                            viewCount = galleryItem.ViewCount
                        }
                    });
                }
                
                return JsonResponseHelper.Success("Image uploaded successfully", new { imageId = result.Id });
            }

            _logger.LogError("Gallery service returned null result for user {UserId}", userId);
            return BadRequest("Failed to upload image - service returned null");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image for user {UserId}, file: {FileName}", 
                _currentUserService.UserId, file?.FileName ?? "unknown");
            return BadRequest($"An error occurred while uploading the image: {ex.Message}");
        }
    }

    [HttpPost("delete/{imageId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid imageId)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            return BadRequest("User not authenticated");

        var result = await _galleryService.DeleteImageAsync(userId, imageId);
        if (result)
            return Json(new { success = true, message = "Image deleted successfully" });

        return BadRequest("Failed to delete image");
    }

    [HttpPost("set-profile-picture/{imageId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetAsProfilePicture(Guid imageId)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            return BadRequest("User not authenticated");

        var result = await _galleryService.SetAsProfilePictureAsync(userId, imageId);
        if (result)
            return Json(new { success = true, message = "Profile picture updated successfully" });

        return BadRequest("Failed to set profile picture");
    }

    [HttpPost("set-cover-image/{imageId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetAsCoverImage(Guid imageId)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            return BadRequest("User not authenticated");

        var result = await _galleryService.SetAsCoverImageAsync(userId, imageId);
        if (result)
            return Json(new { success = true, message = "Cover image updated successfully" });

        return BadRequest("Failed to set cover image");
    }

    [HttpPost("toggle-visibility/{imageId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleVisibility(Guid imageId)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            return BadRequest("User not authenticated");

        var result = await _galleryService.ToggleItemVisibilityAsync(userId, imageId);
        if (result)
            return Json(new { success = true, message = "Visibility updated successfully" });

        return BadRequest("Failed to update visibility");
    }

    [HttpPost("update-caption/{imageId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCaption(Guid imageId, string caption)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            return BadRequest("User not authenticated");

        var result = await _galleryService.UpdateImageCaptionAsync(userId, imageId, caption);
        if (result)
            return Json(new { success = true, message = "Caption updated successfully" });

        return BadRequest("Failed to update caption");
    }

    [HttpGet("view/{imageId}")]
    public async Task<IActionResult> ViewImage(Guid imageId)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            return RedirectToAction("Login", "Account", new { area = "" });

        var image = await _galleryService.GetGalleryItemAsync(userId, imageId);
        if (image == null)
            return NotFound();

        return View("~/Views/Account/Profile/ImageView.cshtml", image);
    }
}
