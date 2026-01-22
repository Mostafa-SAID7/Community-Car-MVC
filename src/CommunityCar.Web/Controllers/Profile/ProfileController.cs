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
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(
        IProfileService profileService,
        ICurrentUserService currentUserService,
        ILogger<ProfileController> logger)
    {
        _profileService = profileService;
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
}