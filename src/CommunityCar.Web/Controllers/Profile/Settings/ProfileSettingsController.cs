using CommunityCar.Application.Common.Interfaces.Services.Profile;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Profile.DTOs;
using CommunityCar.Web.Models.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Profile.Settings;

[Route("profile/settings")]
[Authorize]
public class ProfileSettingsController : Controller
{
    private readonly IProfileManagementService _profileManagementService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ProfileSettingsController> _logger;

    public ProfileSettingsController(
        IProfileManagementService profileManagementService,
        ICurrentUserService currentUserService,
        ILogger<ProfileSettingsController> logger)
    {
        _profileManagementService = profileManagementService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var profileSettings = await _profileManagementService.GetProfileSettingsAsync(userId);
        if (profileSettings == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var viewModel = new ProfileSettingsVM
        {
            Id = profileSettings.Id,
            FullName = profileSettings.FullName,
            Email = profileSettings.Email,
            PhoneNumber = profileSettings.PhoneNumber,
            Bio = profileSettings.Bio,
            City = profileSettings.City,
            Country = profileSettings.Country,
            ProfilePictureUrl = profileSettings.ProfilePictureUrl,
            IsEmailConfirmed = profileSettings.IsEmailConfirmed,
            IsPhoneNumberConfirmed = profileSettings.IsPhoneNumberConfirmed,
            IsTwoFactorEnabled = profileSettings.IsTwoFactorEnabled,
            EmailNotifications = profileSettings.EmailNotifications,
            PushNotifications = profileSettings.PushNotifications,
            SmsNotifications = profileSettings.SmsNotifications,
            MarketingEmails = profileSettings.MarketingEmails,
            HasGoogleAccount = profileSettings.HasGoogleAccount,
            HasFacebookAccount = profileSettings.HasFacebookAccount,
            LastPasswordChange = profileSettings.LastPasswordChange,
            ActiveSessions = profileSettings.ActiveSessions
        };

        return View(viewModel);
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(ProfileSettingsVM model)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        try
        {
            var updateRequest = new UpdateProfileRequest
            {
                FullName = model.FullName,
                Bio = model.Bio,
                City = model.City,
                Country = model.Country,
                PhoneNumber = model.PhoneNumber
            };

            var success = await _profileManagementService.UpdateProfileAsync(userId, updateRequest);

            // Handle profile picture upload separately if provided
            if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
            {
                using var stream = model.ProfilePicture.OpenReadStream();
                await _profileManagementService.UpdateProfilePictureAsync(userId, stream, model.ProfilePicture.FileName);
            }

            // Update notification settings
            var notificationRequest = new UpdateNotificationSettingsRequest
            {
                EmailNotifications = model.EmailNotifications,
                PushNotifications = model.PushNotifications,
                SmsNotifications = model.SmsNotifications,
                MarketingEmails = model.MarketingEmails
            };
            await _profileManagementService.UpdateNotificationSettingsAsync(userId, notificationRequest);

            if (success)
            {
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Failed to update profile. Please try again.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile for user {UserId}", userId);
            ModelState.AddModelError("", "An error occurred while updating your profile.");
        }

        return View("Index", model);
    }

    [HttpPost("delete-profile-picture")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProfilePicture()
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return Json(new { success = false, message = "User not authenticated" });
        }

        try
        {
            var success = await _profileManagementService.DeleteProfilePictureAsync(userId);
            if (success)
            {
                return Json(new { success = true, message = "Profile picture deleted successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to delete profile picture" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting profile picture for user {UserId}", userId);
            return Json(new { success = false, message = "An error occurred while deleting the profile picture" });
        }
    }

    [HttpGet("privacy")]
    public async Task<IActionResult> Privacy()
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var privacySettings = await _profileManagementService.GetPrivacySettingsAsync(userId);
        
        var viewModel = new PrivacySettingsVM
        {
            ProfileVisible = privacySettings.GetValueOrDefault("ProfileVisible", true),
            EmailVisible = privacySettings.GetValueOrDefault("EmailVisible", false),
            PhoneVisible = privacySettings.GetValueOrDefault("PhoneVisible", false),
            AllowMessages = privacySettings.GetValueOrDefault("AllowMessages", true),
            AllowFriendRequests = privacySettings.GetValueOrDefault("AllowFriendRequests", true)
        };

        return View(viewModel);
    }

    [HttpPost("privacy")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePrivacySettings(PrivacySettingsVM model)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var settings = new Dictionary<string, bool>
            {
                ["ProfileVisible"] = model.ProfileVisible,
                ["EmailVisible"] = model.EmailVisible,
                ["PhoneVisible"] = model.PhoneVisible,
                ["AllowMessages"] = model.AllowMessages,
                ["AllowFriendRequests"] = model.AllowFriendRequests
            };

            var success = await _profileManagementService.UpdatePrivacySettingsAsync(userId, settings);
            if (success)
            {
                TempData["SuccessMessage"] = "Privacy settings updated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update privacy settings.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating privacy settings for user {UserId}", userId);
            TempData["ErrorMessage"] = "An error occurred while updating privacy settings.";
        }

        return RedirectToAction(nameof(Privacy));
    }
}