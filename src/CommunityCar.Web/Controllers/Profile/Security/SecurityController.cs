using CommunityCar.Application.Common.Interfaces.Services.Profile;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Profile.DTOs;
using CommunityCar.Web.Models.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Profile.Security;

[Route("profile/security")]
[Authorize]
public class SecurityController : Controller
{
    private readonly IProfileSecurityService _profileSecurityService;
    private readonly IProfileOAuthService _profileOAuthService;
    private readonly IProfileEmailService _profileEmailService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<SecurityController> _logger;

    public SecurityController(
        IProfileSecurityService profileSecurityService,
        IProfileOAuthService profileOAuthService,
        IProfileEmailService profileEmailService,
        ICurrentUserService currentUserService,
        ILogger<SecurityController> logger)
    {
        _profileSecurityService = profileSecurityService;
        _profileOAuthService = profileOAuthService;
        _profileEmailService = profileEmailService;
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

        var securityLog = await _profileSecurityService.GetSecurityLogAsync(userId, 1, 10);

        var viewModel = new SecurityVM
        {
            IsTwoFactorEnabled = false, // Will be populated from user data
            LastPasswordChange = null, // Will be populated from user data
            ActiveSessions = 1, // Will be populated from user data
            RecentActivity = securityLog.Select(log => new SecurityLogItemVM
            {
                Timestamp = log.Timestamp,
                Action = log.Action,
                IpAddress = log.IpAddress,
                UserAgent = log.UserAgent,
                Location = log.Location,
                IsSuccessful = log.IsSuccessful
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpGet("change-password")]
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordVM());
    }

    [HttpPost("change-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var request = new ChangePasswordRequest
            {
                CurrentPassword = model.CurrentPassword,
                NewPassword = model.NewPassword,
                ConfirmPassword = model.ConfirmPassword
            };

            var success = await _profileSecurityService.ChangePasswordAsync(userId, request);
            if (success)
            {
                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Failed to change password. Please check your current password.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user {UserId}", userId);
            ModelState.AddModelError("", "An error occurred while changing your password.");
        }

        return View(model);
    }

    [HttpGet("setup-2fa")]
    public async Task<IActionResult> SetupTwoFactor()
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var setup = await _profileSecurityService.SetupTwoFactorAsync(userId);
            var viewModel = new TwoFactorSetupVM
            {
                SecretKey = setup.SecretKey,
                QrCodeUrl = setup.QrCodeUrl,
                ManualEntryKey = setup.ManualEntryKey,
                BackupCodes = setup.BackupCodes
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting up 2FA for user {UserId}", userId);
            TempData["ErrorMessage"] = "An error occurred while setting up two-factor authentication.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost("enable-2fa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnableTwoFactor(TwoFactorSetupVM model)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            return View("SetupTwoFactor", model);
        }

        try
        {
            var request = new TwoFactorSetupRequest
            {
                Code = model.Code
            };

            var success = await _profileSecurityService.EnableTwoFactorAsync(userId, request);
            if (success)
            {
                TempData["SuccessMessage"] = "Two-factor authentication enabled successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("Code", "Invalid verification code. Please try again.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enabling 2FA for user {UserId}", userId);
            ModelState.AddModelError("", "An error occurred while enabling two-factor authentication.");
        }

        return View("SetupTwoFactor", model);
    }

    [HttpPost("disable-2fa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DisableTwoFactor([FromForm] string password)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return Json(new { success = false, message = "User not authenticated" });
        }

        if (string.IsNullOrEmpty(password))
        {
            return Json(new { success = false, message = "Password is required" });
        }

        try
        {
            var success = await _profileSecurityService.DisableTwoFactorAsync(userId, password);
            if (success)
            {
                return Json(new { success = true, message = "Two-factor authentication disabled successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Invalid password" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disabling 2FA for user {UserId}", userId);
            return Json(new { success = false, message = "An error occurred while disabling two-factor authentication" });
        }
    }

    [HttpPost("link-google")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LinkGoogleAccount([FromForm] string googleId, [FromForm] string? profilePictureUrl)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return Json(new { success = false, message = "User not authenticated" });
        }

        try
        {
            var success = await _profileOAuthService.LinkGoogleAccountAsync(userId, googleId, profilePictureUrl);
            if (success)
            {
                return Json(new { success = true, message = "Google account linked successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to link Google account" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error linking Google account for user {UserId}", userId);
            return Json(new { success = false, message = "An error occurred while linking Google account" });
        }
    }

    [HttpPost("unlink-google")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnlinkGoogleAccount()
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return Json(new { success = false, message = "User not authenticated" });
        }

        try
        {
            var success = await _profileOAuthService.UnlinkGoogleAccountAsync(userId);
            if (success)
            {
                return Json(new { success = true, message = "Google account unlinked successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to unlink Google account" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlinking Google account for user {UserId}", userId);
            return Json(new { success = false, message = "An error occurred while unlinking Google account" });
        }
    }

    [HttpPost("link-facebook")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LinkFacebookAccount([FromForm] string facebookId, [FromForm] string? profilePictureUrl)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return Json(new { success = false, message = "User not authenticated" });
        }

        try
        {
            var success = await _profileOAuthService.LinkFacebookAccountAsync(userId, facebookId, profilePictureUrl);
            if (success)
            {
                return Json(new { success = true, message = "Facebook account linked successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to link Facebook account" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error linking Facebook account for user {UserId}", userId);
            return Json(new { success = false, message = "An error occurred while linking Facebook account" });
        }
    }

    [HttpPost("unlink-facebook")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnlinkFacebookAccount()
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return Json(new { success = false, message = "User not authenticated" });
        }

        try
        {
            var success = await _profileOAuthService.UnlinkFacebookAccountAsync(userId);
            if (success)
            {
                return Json(new { success = true, message = "Facebook account unlinked successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to unlink Facebook account" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlinking Facebook account for user {UserId}", userId);
            return Json(new { success = false, message = "An error occurred while unlinking Facebook account" });
        }
    }
}