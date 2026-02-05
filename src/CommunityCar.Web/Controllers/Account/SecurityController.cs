using CommunityCar.Web.Areas.Identity.Interfaces.Services.Security;
using CommunityCar.Web.Areas.Identity.Interfaces.Services.Core;
using CommunityCar.Application.Features.Account.ViewModels.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Web.Controllers.Account;

[Route("account/security")]
[Authorize]
public class SecurityController : Controller
{
    private readonly IAccountSecurityService _securityService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<SecurityController> _logger;

    public SecurityController(
        IAccountSecurityService securityService,
        ICurrentUserService currentUserService,
        ILogger<SecurityController> logger)
    {
        _securityService = securityService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            return RedirectToAction("Login", "Account");

        var lastPasswordChange = await _securityService.GetLastPasswordChangeAsync(userId);
        var twoFactorSetup = await _securityService.SetupTwoFactorAsync(userId);

        var model = new SecurityVM
        {
            TwoFactorEnabled = twoFactorSetup.IsEnabled,
            ActiveSessions = 1, // Mock data since we don't have session tracking
            LastPasswordChange = lastPasswordChange ?? DateTime.UtcNow.AddDays(-30),
            RecentActivity = new List<string> { "Login from Chrome", "Password changed", "Two-factor enabled" }
        };

        return View("Auth/SecurityIndex", model);
    }

    #region Password

    [HttpGet("change-password")]
    public IActionResult ChangePassword()
    {
        return View("Auth/ChangePassword");
    }

    [HttpPost("change-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(CommunityCar.Application.Features.Account.ViewModels.Authentication.ChangePasswordVM model)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
            return View("Auth/ChangePassword", model);

        var request = new ChangePasswordRequest
        {
            UserId = userId,
            OldPassword = model.OldPassword,
            NewPassword = model.NewPassword,
            ConfirmPassword = model.ConfirmPassword
        };

        var result = await _securityService.ChangePasswordAsync(userId, request);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Password changed successfully.";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return View("Auth/ChangePassword", model);
    }

    #endregion

    #region Two-Factor Authentication

    [HttpGet("two-factor")]
    public async Task<IActionResult> TwoFactor()
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            return RedirectToAction("Login", "Account");

        var twoFactorSetup = await _securityService.SetupTwoFactorAsync(userId);
        
        var model = new TwoFactorVM
        {
            IsEnabled = twoFactorSetup.IsEnabled
        };

        if (!twoFactorSetup.IsEnabled)
        {
            model.AuthenticatorKey = twoFactorSetup.SecretKey;
            model.AuthenticatorUri = twoFactorSetup.QrCodeUri;
        }

        return View("Auth/TwoFactor", model);
    }

    [HttpPost("two-factor/enable")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnableTwoFactor(EnableTwoFactorVM model)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            return RedirectToAction("Login", "Authentication");

        if (!ModelState.IsValid)
            return RedirectToAction(nameof(TwoFactor));

        var request = new TwoFactorSetupRequest 
        { 
            UserId = userId,
            Code = model.VerificationCode,
            SecretKey = model.ManualEntryKey ?? string.Empty
        };

        var success = await _securityService.EnableTwoFactorAsync(userId, request);
        if (success)
        {
            TempData["SuccessMessage"] = "Two-factor authentication has been enabled.";
        }
        else
        {
            TempData["ErrorMessage"] = "Could not enable two-factor authentication. Invalid code.";
        }

        return RedirectToAction(nameof(TwoFactor));
    }

    [HttpPost("two-factor/disable")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DisableTwoFactor(DisableTwoFactorVM model)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            return RedirectToAction("Login", "Authentication");

        if (!ModelState.IsValid)
            return RedirectToAction(nameof(TwoFactor));

        var success = await _securityService.DisableTwoFactorAsync(userId, model.Password);
        if (success)
        {
            TempData["SuccessMessage"] = "Two-factor authentication has been disabled.";
        }
        else
        {
            TempData["ErrorMessage"] = "Could not disable two-factor authentication. Incorrect password.";
        }

        return RedirectToAction(nameof(TwoFactor));
    }

    #endregion
}

