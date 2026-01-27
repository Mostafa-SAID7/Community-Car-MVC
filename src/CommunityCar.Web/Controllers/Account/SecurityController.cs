using CommunityCar.Application.Common.Interfaces.Orchestrators;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Common.Models.Authentication;
using CommunityCar.Web.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Account;

[Route("account/security")]
[Authorize]
public class SecurityController : Controller
{
    private readonly IAccountSecurityOrchestrator _securityOrchestrator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<SecurityController> _logger;

    public SecurityController(
        IAccountSecurityOrchestrator securityOrchestrator,
        ICurrentUserService currentUserService,
        ILogger<SecurityController> logger)
    {
        _securityOrchestrator = securityOrchestrator;
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

        var result = await _securityOrchestrator.ChangePasswordAsync(request);
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

        var info = await _securityOrchestrator.GetTwoFactorInfoAsync(userId.ToString());
        
        var model = new TwoFactorVM
        {
            IsEnabled = info.IsEnabled,
            IsMachineRemembered = info.IsMachineRemembered,
            RecoveryCodesLeft = info.RecoveryCodesLeft
        };

        if (info.IsEnabled)
        {
            model.RecoveryCodes = (await _securityOrchestrator.GenerateRecoveryCodesAsync(userId.ToString())).ToList();
        }
        else
        {
            model.AuthenticatorKey = await _securityOrchestrator.GetAuthenticatorKeyAsync(userId.ToString());
            model.AuthenticatorUri = info.AuthenticatorUri;
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

        var result = await _securityOrchestrator.EnableTwoFactorAsync(userId.ToString(), model.Code);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Two-factor authentication has been enabled.";
            // Show recovery codes
            var codes = await _securityOrchestrator.GenerateRecoveryCodesAsync(userId.ToString());
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

        var result = await _securityOrchestrator.DisableTwoFactorAsync(userId.ToString());
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

        var codes = await _securityOrchestrator.GenerateRecoveryCodesAsync(userId.ToString());
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
            UserId = userId,
            Token = model.IdToken,
            Provider = "Google"
        };

        var result = await _securityOrchestrator.LinkGoogleAccountAsync(request);
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
            UserId = userId,
            Token = model.AccessToken,
            Provider = "Facebook"
        };

        var result = await _securityOrchestrator.LinkFacebookAccountAsync(request);
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

        var request = new UnlinkExternalAccountRequest
        {
            UserId = userId,
            Provider = provider
        };

        var result = await _securityOrchestrator.UnlinkExternalAccountAsync(request);
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


