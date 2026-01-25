using CommunityCar.Application.Common.Interfaces.Services.Profile;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Profile.DTOs;
using CommunityCar.Web.Models.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Profile;

[Route("profile")]
[Authorize]
public class ProfileController : Controller
{
    private readonly IProfileService _profileService;
    private readonly IUserGalleryService _galleryService;
    private readonly IGamificationService _gamificationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(
        IProfileService profileService,
        IUserGalleryService galleryService,
        IGamificationService gamificationService,
        ICurrentUserService currentUserService,
        ILogger<ProfileController> logger)
    {
        _profileService = profileService;
        _galleryService = galleryService;
        _gamificationService = gamificationService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    private async Task SetProfileHeaderDataAsync(Guid userId)
    {
        try
        {
            var profile = await _profileService.GetProfileAsync(userId);
            if (profile != null)
            {
                ViewBag.FullName = profile.FullName;
                ViewBag.Email = profile.Email;
                ViewBag.ProfilePictureUrl = profile.ProfilePictureUrl;
                ViewBag.CreatedAt = profile.CreatedAt;
                ViewBag.PostsCount = profile.PostsCount;
                ViewBag.CommentsCount = profile.CommentsCount;
                ViewBag.LikesReceived = profile.LikesReceived;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load profile header data for user {UserId}", userId);
            // Set default values
            ViewBag.FullName = "User Profile";
            ViewBag.Email = "user@example.com";
            ViewBag.ProfilePictureUrl = null;
            ViewBag.CreatedAt = DateTime.Now;
            ViewBag.PostsCount = 0;
            ViewBag.CommentsCount = 0;
            ViewBag.LikesReceived = 0;
        }
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var profile = await _profileService.GetProfileAsync(userId);
        if (profile == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var viewModel = new ProfileIndexVM
        {
            Id = profile.Id,
            FullName = profile.FullName,
            Email = profile.Email,
            PhoneNumber = profile.PhoneNumber,
            Bio = profile.Bio,
            City = profile.City,
            Country = profile.Country,
            ProfilePictureUrl = profile.ProfilePictureUrl,
            CreatedAt = profile.CreatedAt,
            LastLoginAt = profile.LastLoginAt,
            IsEmailConfirmed = profile.IsEmailConfirmed,
            IsPhoneNumberConfirmed = profile.IsPhoneNumberConfirmed,
            IsTwoFactorEnabled = profile.IsTwoFactorEnabled,
            IsActive = profile.IsActive,
            HasGoogleAccount = profile.HasGoogleAccount,
            HasFacebookAccount = profile.HasFacebookAccount,
            PostsCount = profile.PostsCount,
            CommentsCount = profile.CommentsCount,
            LikesReceived = profile.LikesReceived
        };

        return View(viewModel);
    }

    [HttpGet("settings")]
    public async Task<IActionResult> Settings()
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var settings = await _profileService.GetProfileSettingsAsync(userId);
        if (settings == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Set ViewBag data for the profile header
        await SetProfileHeaderDataAsync(userId);

        var viewModel = new ProfileSettingsVM
        {
            Id = settings.Id,
            FullName = settings.FullName,
            Email = settings.Email,
            PhoneNumber = settings.PhoneNumber,
            Bio = settings.Bio,
            City = settings.City,
            Country = settings.Country,
            ProfilePictureUrl = settings.ProfilePictureUrl,
            IsEmailConfirmed = settings.IsEmailConfirmed,
            IsPhoneNumberConfirmed = settings.IsPhoneNumberConfirmed,
            IsTwoFactorEnabled = settings.IsTwoFactorEnabled,
            HasGoogleAccount = settings.HasGoogleAccount,
            HasFacebookAccount = settings.HasFacebookAccount,
            EmailNotifications = settings.EmailNotifications,
            PushNotifications = settings.PushNotifications,
            SmsNotifications = settings.SmsNotifications,
            MarketingEmails = settings.MarketingEmails,
            ActiveSessions = settings.ActiveSessions
        };

        return View(viewModel);
    }

    [HttpPost("update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(UpdateProfileVM model)
    {
        if (!ModelState.IsValid)
        {
            return View("Settings", model);
        }

        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var request = new UpdateProfileRequest
        {
            FullName = model.FullName,
            PhoneNumber = model.PhoneNumber,
            Bio = model.Bio,
            City = model.City,
            Country = model.Country
        };

        var success = await _profileService.UpdateProfileAsync(userId, request);
        if (success)
        {
            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", "Failed to update profile. Please try again.");
        return View("Settings", model);
    }

    [HttpPost("upload-picture")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadProfilePicture(IFormFile profilePicture)
    {
        if (profilePicture == null || profilePicture.Length == 0)
        {
            TempData["ErrorMessage"] = "Please select a valid image file.";
            return RedirectToAction("Settings");
        }

        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        using var stream = profilePicture.OpenReadStream();
        var success = await _profileService.UpdateProfilePictureAsync(userId, stream, profilePicture.FileName);
        
        if (success)
        {
            TempData["SuccessMessage"] = "Profile picture updated successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to update profile picture. Please try again.";
        }

        return RedirectToAction("Settings");
    }

    [HttpPost("delete-picture")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProfilePicture()
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var success = await _profileService.DeleteProfilePictureAsync(userId);
        
        if (success)
        {
            TempData["SuccessMessage"] = "Profile picture deleted successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to delete profile picture. Please try again.";
        }

        return RedirectToAction("Settings");
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return Json(new { success = false, message = "User not found" });
        }

        var stats = await _profileService.GetProfileStatsAsync(userId);
        return Json(new { success = true, data = stats });
    }

    [HttpGet("interests")]
    public async Task<IActionResult> Interests()
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        await SetProfileHeaderDataAsync(userId);
        ViewBag.CurrentUserId = userId;
        return View();
    }

    [HttpGet("gallery")]
    public async Task<IActionResult> Gallery()
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var galleryItems = await _galleryService.GetUserGalleryAsync(userId);
        var gamificationStats = await _gamificationService.GetUserStatsAsync(userId);

        await SetProfileHeaderDataAsync(userId);
        ViewBag.CurrentUserId = userId;
        ViewBag.GalleryItems = galleryItems;
        ViewBag.GamificationStats = gamificationStats;

        return View();
    }

    [HttpPost("gallery/upload")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadGalleryItem(CreateGalleryItemRequest request, IFormFile mediaFile)
    {
        if (mediaFile == null || mediaFile.Length == 0)
        {
            TempData["ErrorMessage"] = "Please select a valid media file.";
            return RedirectToAction("Gallery");
        }

        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            using var stream = mediaFile.OpenReadStream();
            var itemId = await _galleryService.CreateGalleryItemAsync(userId, request, stream, mediaFile.FileName);
            
            TempData["SuccessMessage"] = "Media uploaded successfully!";
            
            // Process gamification
            await _gamificationService.ProcessUserActionAsync(userId, "gallery_upload");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload gallery item for user {UserId}", userId);
            TempData["ErrorMessage"] = "Failed to upload media. Please try again.";
        }

        return RedirectToAction("Gallery");
    }

    [HttpGet("badges")]
    public async Task<IActionResult> Badges()
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var badges = await _gamificationService.GetUserBadgesAsync(userId);
        var achievements = await _gamificationService.GetUserAchievementsAsync(userId);
        var stats = await _gamificationService.GetUserStatsAsync(userId);

        await SetProfileHeaderDataAsync(userId);
        ViewBag.CurrentUserId = userId;
        ViewBag.Badges = badges;
        ViewBag.Achievements = achievements;
        ViewBag.GamificationStats = stats;

        return View();
    }

    [HttpPost("update-privacy")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePrivacySettings(PrivacySettingsVM model)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var request = new UpdatePrivacySettingsRequest
        {
            ProfileVisible = model.ProfileVisible,
            EmailVisible = model.EmailVisible,
            PhoneVisible = model.PhoneVisible,
            AllowMessages = model.AllowMessages,
            AllowFriendRequests = model.AllowFriendRequests
        };

        var success = await _profileService.UpdatePrivacySettingsAsync(userId, request);
        if (success)
        {
            TempData["SuccessMessage"] = "Privacy settings updated successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to update privacy settings. Please try again.";
        }

        return RedirectToAction("Settings");
    }

    [HttpPost("update-notifications")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateNotificationSettings(ProfileSettingsVM model)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var request = new UpdateNotificationSettingsRequest
        {
            EmailNotifications = model.EmailNotifications,
            PushNotifications = model.PushNotifications,
            SmsNotifications = model.SmsNotifications,
            MarketingEmails = model.MarketingEmails
        };

        var success = await _profileService.UpdateNotificationSettingsAsync(userId, request);
        if (success)
        {
            TempData["SuccessMessage"] = "Notification settings updated successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to update notification settings. Please try again.";
        }

        return RedirectToAction("Settings");
    }

    [HttpPost("gallery/toggle-visibility/{itemId}")]
    public async Task<IActionResult> ToggleGalleryItemVisibility(Guid itemId)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return Json(new { success = false, message = "User not found" });
        }

        var success = await _galleryService.ToggleItemVisibilityAsync(itemId, userId);
        return Json(new { success });
    }

    [HttpPost("gallery/delete/{itemId}")]
    public async Task<IActionResult> DeleteGalleryItem(Guid itemId)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return Json(new { success = false, message = "User not found" });
        }

        var success = await _galleryService.DeleteGalleryItemAsync(itemId, userId);
        return Json(new { success });
    }

    [HttpGet("gallery/item/{itemId}")]
    public async Task<IActionResult> GetGalleryItem(Guid itemId)
    {
        var item = await _galleryService.GetGalleryItemAsync(itemId);
        if (item == null)
        {
            return NotFound();
        }

        return Json(new { success = true, data = item });
    }
}