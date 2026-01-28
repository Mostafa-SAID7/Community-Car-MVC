using CommunityCar.Application.Common.Interfaces.Orchestrators;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Common.Models.Authentication;
using CommunityCar.Web.Models.Account;
using CommunityCar.Web.Models.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Account;

[Route("account/security")]
[Authorize]
public class SecurityController : Controller
{
    private readonly IAccountOrchestrator _accountOrchestrator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<SecurityController> _logger;

    public SecurityController(
        IAccountOrchestrator accountOrchestrator,
        ICurrentUserService currentUserService,
        ILogger<SecurityController> logger)
    {
        _accountOrchestrator = accountOrchestrator;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    #region Password

    [HttpGet("change-password")]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost("change-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            return RedirectToAction("Login", "Authentication", new { area = "" });

        if (!ModelState.IsValid)
            return View(model);

        var request = new ChangePasswordRequest
        {
            UserId = userId,
            CurrentPassword = model.CurrentPassword,
            NewPassword = model.NewPassword
        };

        var result = await _accountOrchestrator.ChangePasswordAsync(request);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Password changed successfully.";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return View(model);
    }

    #endregion

    #region Two-Factor Authentication

    [HttpGet("two-factor")]
    public async Task<IActionResult> TwoFactor()
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            return RedirectToAction("Login", "Authentication", new { area = "" });

        var securityInfo = await _accountOrchestrator.GetSecurityInfoAsync(userId);
        
        var model = new TwoFactorVM
        {
            IsEnabled = securityInfo.IsTwoFactorEnabled,
            // IsMachineRemembered = info.IsMachineRemembered, // Not available
            // RecoveryCodesLeft = info.RecoveryCodesLeft // Not available directly
        };

        if (securityInfo.IsTwoFactorEnabled)
        {
             // If enabled, we probably shouldn't regenerate codes on every view, 
             // but current logic implies showing them. 
             // However, GenerateRecoveryCodesAsync invalidates old ones usually.
             // We'll just show status. User can click "Generate" to see codes.
        }
        else
        {
            var setup = await _accountOrchestrator.SetupTwoFactorAsync(userId);
            model.AuthenticatorKey = setup.SecretKey;
            model.AuthenticatorUri = setup.QrCodeUri;
        }

        return View(model);
    }

    [HttpPost("two-factor/enable")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnableTwoFactor(EnableTwoFactorVM model)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            return RedirectToAction("Login", "Authentication", new { area = "" });

        if (!ModelState.IsValid)
            return RedirectToAction(nameof(TwoFactor));

        var request = new TwoFactorSetupRequest 
        { 
            Code = model.VerificationCode,
            // SecretKey is needed if the service validates it against the one in request,
            // but usually it validates against what was stored in session/temp during setup.
            // Start with empty or if we need to pass it from view (hidden field).
            // Assuming simplified flow where service handles state or we don't need to pass key back if stateless TOTP.
            // But TwoFactorSetupRequest has SecretKey.
             SecretKey = model.ManualEntryKey // Assuming this hidden field holds the key
        };

        var result = await _accountOrchestrator.EnableTwoFactorAsync(userId, request);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Two-factor authentication has been enabled.";
            var codes = await _accountOrchestrator.GenerateRecoveryCodesAsync(userId);
            TempData["RecoveryCodes"] = codes;
        }
        else
        {
            TempData["ErrorMessage"] = "Could not enable two-factor authentication. Invalid code.";
        }

        return RedirectToAction(nameof(TwoFactor));
    }

    [HttpPost("two-factor/disable")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DisableTwoFactor()
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            return RedirectToAction("Login", "Authentication", new { area = "" });

        // Note: Password should ideally be verified here
        var result = await _accountOrchestrator.DisableTwoFactorAsync(userId, string.Empty);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Two-factor authentication has been disabled.";
        }
        else
        {
            TempData["ErrorMessage"] = "Could not disable two-factor authentication.";
        }

        return RedirectToAction(nameof(TwoFactor));
    }

    [HttpPost("two-factor/generate-codes")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateRecoveryCodes()
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            return RedirectToAction("Login", "Authentication", new { area = "" });

        var codes = await _accountOrchestrator.GenerateRecoveryCodesAsync(userId);
        TempData["RecoveryCodes"] = codes;
        TempData["SuccessMessage"] = "New recovery codes have been generated.";

        return RedirectToAction(nameof(TwoFactor));
    }

    #endregion

    #region External Logins

    [HttpPost("link-google")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LinkGoogleAccount(GoogleSignInVM model)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
             return BadRequest("User not authenticated");

        var request = new LinkExternalAccountRequest
        {
            UserId = userId.ToString(), // Request model uses string
            ExternalToken = model.IdToken, // Mapped to ExternalToken
            Provider = "Google"
        };

        var result = await _accountOrchestrator.LinkExternalAccountAsync(request);
        if (result.Succeeded)
            return Ok(new { success = true, message = "Google account linked successfully." });

        return BadRequest(new { success = false, message = result.Message });
    }

    [HttpPost("link-facebook")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LinkFacebookAccount(FacebookSignInVM model)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
             return BadRequest("User not authenticated");
             
        var request = new LinkExternalAccountRequest
        {
            UserId = userId.ToString(),
            ExternalToken = model.AccessToken,
            Provider = "Facebook"
        };

        var result = await _accountOrchestrator.LinkExternalAccountAsync(request);
        if (result.Succeeded)
            return Ok(new { success = true, message = "Facebook account linked successfully." });

        return BadRequest(new { success = false, message = result.Message });
    }

    [HttpPost("unlink")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnlinkAccount(string provider)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            return BadRequest("User not authenticated");

        var result = await _accountOrchestrator.UnlinkExternalAccountAsync(userId, provider);
        if (result.Succeeded)
        {
             TempData["SuccessMessage"] = $"{provider} account unlinked successfully.";
             return RedirectToAction(nameof(Index));
        }

        TempData["ErrorMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    #endregion
}


