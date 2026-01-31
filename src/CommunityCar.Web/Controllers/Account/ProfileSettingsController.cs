using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Interfaces.Services.Storage;
using CommunityCar.Application.Features.Account.ViewModels.Core;
using CommunityCar.Application.Features.Account.ViewModels.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Account;

[Route("profile/settings")]
[Authorize]
public class ProfileSettingsController : Controller
{
    private readonly IProfileService _profileService;
    private readonly IAccountManagementService _accountManagementService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<ProfileSettingsController> _logger;

    private readonly IGamificationService _gamificationService;

    public ProfileSettingsController(
        IProfileService profileService,
        IAccountManagementService accountManagementService,
        IGamificationService gamificationService,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService,
        ILogger<ProfileSettingsController> logger)
    {
        _profileService = profileService;
        _accountManagementService = accountManagementService;
        _gamificationService = gamificationService;
        _fileStorageService = fileStorageService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
             return RedirectToAction("Login", "Account", new { area = "" });

        var profile = await _profileService.GetProfileAsync(userId);
        if (profile == null) return NotFound();

        var privacy = await _accountManagementService.GetPrivacySettingsAsync(userId);
        var notifications = await _accountManagementService.GetNotificationSettingsAsync(userId);

        // Populate Sidebar ViewBag Data
        ViewBag.UserId = userId.ToString();
        ViewBag.FullName = profile.FullName;
        ViewBag.Email = profile.Email;
        ViewBag.UserName = profile.UserName;
        ViewBag.Bio = profile.Bio;
        ViewBag.BioAr = profile.BioAr;
        ViewBag.City = profile.City;
        ViewBag.Country = profile.Country;
        ViewBag.ProfilePictureUrl = profile.ProfilePictureUrl;
        ViewBag.CreatedAt = profile.CreatedAt;
        ViewBag.PostsCount = profile.PostsCount;
        ViewBag.CommentsCount = profile.CommentsCount;
        ViewBag.LikesReceived = profile.LikesReceived;

        var stats = await _gamificationService.GetUserStatsAsync(userId);
        if (stats != null)
        {
            ViewBag.Level = stats.Level;
            ViewBag.TotalPoints = stats.TotalPoints;
            ViewBag.Rank = stats.Rank;
            ViewBag.BadgesCount = stats.BadgesCount;
        }

        var viewModel = new ProfileSettingsVM
        {
            Id = profile.Id,
            FullName = profile.FullName,
            Email = profile.Email,
            PhoneNumber = profile.PhoneNumber,
            Bio = profile.Bio,
            City = profile.City,
            Country = profile.Country,
            BioAr = profile.BioAr,
            CityAr = profile.CityAr,
            CountryAr = profile.CountryAr,
            ProfilePictureUrl = profile.ProfilePictureUrl,
            CoverImageUrl = profile.CoverImageUrl,
            IsEmailConfirmed = profile.IsEmailConfirmed,
            IsPhoneNumberConfirmed = profile.IsPhoneNumberConfirmed,
            IsTwoFactorEnabled = profile.IsTwoFactorEnabled,
            HasGoogleAccount = profile.HasGoogleAccount,
            HasFacebookAccount = profile.HasFacebookAccount,
            
            // Privacy
            PublicProfile = privacy.ProfileVisible,
            // EmailVisible = privacy.EmailVisible,
            // PhoneVisible = privacy.PhoneVisible,
            
            // Notifications
            EmailNotifications = notifications.EmailNotifications,
            PushNotifications = notifications.PushNotifications,
            SmsNotifications = notifications.SmsNotifications,
            MarketingEmails = notifications.MarketingEmails
        };

        return View("~/Views/Account/Profile/Settings.cshtml", viewModel);
    }

    [HttpPost("privacy")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePrivacy(PrivacySettingsVM model)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
             return BadRequest("User not authenticated");

        if (!ModelState.IsValid)
             return BadRequest(ModelState);

        var request = new UpdatePrivacySettingsRequest
        {
            UserId = userId,
            ProfileVisible = model.ProfileVisible,
            EmailVisible = model.EmailVisible,
            PhoneVisible = model.PhoneVisible,
            IsPublic = model.IsPublic,
            AllowMessages = model.AllowMessages,
            AllowFriendRequests = model.AllowFriendRequests,
            DefaultGalleryPrivacy = model.DefaultGalleryPrivacy,
            ShowActivityStatus = model.ShowActivityStatus,
            ShowOnlineStatus = model.ShowOnlineStatus
        };

        var result = await _accountManagementService.UpdatePrivacySettingsAsync(request);
        if (result.Succeeded)
             return Ok(new { success = true, message = "Privacy settings updated." });

        return BadRequest(new { success = false, message = "Failed to update privacy settings." });
    }

    [HttpPost("notifications")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateNotifications(NotificationSettingsVM model)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
             return BadRequest("User not authenticated");

        if (!ModelState.IsValid)
             return BadRequest(ModelState);

        var request = new UpdateNotificationSettingsRequest
        {
            UserId = userId,
            EmailNotifications = model.EmailNotifications,
            PushNotifications = model.PushNotifications,
            SmsNotifications = model.SmsNotifications,
            MarketingEmails = model.MarketingEmails,
            WeeklyDigest = model.WeeklyDigest,
            SecurityAlerts = model.SecurityAlerts
        };

        var result = await _accountManagementService.UpdateNotificationSettingsAsync(request);
        if (result.Succeeded)
             return Ok(new { success = true, message = "Notification settings updated." });

        return BadRequest(new { success = false, message = "Failed to update notification settings." });
    }

    [HttpPost("profile")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(ProfileSettingsVM model)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return BadRequest(new { success = false, message = "User not authenticated" });
                
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            if (!ModelState.IsValid)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(new { success = false, message = "Validation failed", errors });
                }
                
                // For regular form submission, return to the settings page with validation errors
                TempData["ErrorMessage"] = "Please correct the validation errors and try again.";
                return RedirectToAction("Index");
            }

            var updateRequest = new UpdateProfileRequest
            {
                UserId = userId,
                FullName = model.FullName ?? string.Empty,
                Bio = model.Bio,
                City = model.City,
                Country = model.Country,
                PhoneNumber = model.PhoneNumber,
                Website = null // ProfileSettingsVM doesn't have Website, so set to null
            };

            var result = await _profileService.UpdateProfileAsync(updateRequest);
            if (result)
            {
                // Handle profile picture upload if provided
                if (model.ProfilePicture != null)
                {
                    // First upload the file to get the URL
                    using var stream = model.ProfilePicture.OpenReadStream();
                    var fileName = $"profile_{userId}_{Guid.NewGuid()}{Path.GetExtension(model.ProfilePicture.FileName)}";
                    var imageUrl = await _fileStorageService.UploadFileAsync(stream, fileName, model.ProfilePicture.ContentType);
                    
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        var uploadResult = await _profileService.UpdateProfilePictureAsync(userId, imageUrl);
                        if (!uploadResult)
                        {
                            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                                return BadRequest(new { success = false, message = "Profile updated but failed to upload picture." });
                            
                            TempData["ErrorMessage"] = "Profile updated but failed to upload picture.";
                            return RedirectToAction("Index");
                        }
                    }
                }

                // Check if this is an AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Ok(new { success = true, message = "Profile updated successfully." });
                }

                // For regular form submission, redirect with success message
                TempData["SuccessMessage"] = "Profile updated successfully.";
                return RedirectToAction("Index");
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return BadRequest(new { success = false, message = "Failed to update profile." });

            TempData["ErrorMessage"] = "Failed to update profile.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile for user {UserId}", _currentUserService.UserId);
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return BadRequest(new { success = false, message = "An unexpected error occurred while updating profile." });

            TempData["ErrorMessage"] = "An unexpected error occurred while updating profile.";
            return RedirectToAction("Index");
        }
    }
}



