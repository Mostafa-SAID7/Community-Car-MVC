using CommunityCar.Application.Common.Interfaces.Services.Account.Management;
using CommunityCar.Application.Common.Interfaces.Services.Account.Core;
using CommunityCar.Application.Features.Account.ViewModels.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Web.Controllers.Account;

[Route("account/settings")]
[Authorize]
public class AccountSettingsController : Controller
{
    private readonly IAccountManagementService _managementService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AccountSettingsController> _logger;

    public AccountSettingsController(
        IAccountManagementService managementService,
        ICurrentUserService currentUserService,
        ILogger<AccountSettingsController> logger)
    {
        _managementService = managementService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    #region Account Deactivation

    [HttpGet("deactivate")]
    public IActionResult DeactivateAccount()
    {
        return View();
    }

    [HttpPost("deactivate")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeactivateAccount(DeactivateAccountVM model)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
            return View(model);

        var request = new DeactivateAccountRequest
        {
            UserId = userId,
            Password = model.Password,
            Reason = model.Reason
        };

        var result = await _managementService.DeactivateAccountAsync(request);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Account deactivated successfully. You can reactivate it by logging in again within 30 days.";
            return RedirectToAction("Logout", "Authentication");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return View(model);
    }

    #endregion

    #region Account Deletion

    [HttpGet("delete")]
    public IActionResult DeleteAccount()
    {
        return View();
    }

    [HttpPost("delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAccount(DeleteAccountVM model)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
            return View(model);

        if (!model.ConfirmDeletion)
        {
            ModelState.AddModelError(nameof(model.ConfirmDeletion), "You must confirm account deletion.");
            return View(model);
        }

        var request = new DeleteAccountRequest
        {
            UserId = userId,
            Password = model.Password,
            Reason = model.Reason
        };

        var result = await _managementService.DeleteAccountAsync(request);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Account deleted successfully. We're sorry to see you go!";
            return RedirectToAction("Login", "Account");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return View(model);
    }

    #endregion

    #region Data Export

    [HttpGet("export-data")]
    public IActionResult ExportData()
    {
        return View();
    }

    [HttpPost("export-data")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExportData(ExportDataVM model)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            return BadRequest("User not authenticated");

        if (!ModelState.IsValid)
            return View(model);

        var request = new ExportUserDataRequest
        {
            UserId = userId,
            DataCategories = new List<string>()
        };

        if (model.IncludeProfile) request.DataCategories.Add("Profile");
        if (model.IncludeActivity) request.DataCategories.Add("Activity");
        if (model.IncludeMedia) request.DataCategories.Add("Media");

        var result = await _managementService.ExportUserDataAsync(request);
        if (result.Succeeded && result.Data is byte[] data)
        {
            var fileName = $"user-data-export-{DateTime.UtcNow:yyyyMMdd-HHmmss}.zip";
            return File(data, "application/zip", fileName);
        }

        TempData["ErrorMessage"] = result.Message;
        return View(model);
    }

    #endregion

    #region Navigation Methods

    [HttpGet("privacy")]
    public IActionResult Privacy()
    {
        return RedirectToAction("Index", "PrivacySettings");
    }

    #endregion
}