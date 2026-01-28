using CommunityCar.Application.Common.Interfaces.Orchestrators;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Web.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Account;

[Route("account/management")]
[Authorize]
public class ManagementController : Controller
{
    private readonly IAccountOrchestrator _accountOrchestrator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ManagementController> _logger;

    public ManagementController(
        IAccountOrchestrator accountOrchestrator,
        ICurrentUserService currentUserService,
        ILogger<ManagementController> logger)
    {
        _accountOrchestrator = accountOrchestrator;
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
            return RedirectToAction("Login", "Authentication", new { area = "" });

        if (!ModelState.IsValid)
            return View(model);

        var request = new DeactivateAccountRequest
        {
            UserId = userId,
            Password = model.Password,
            Reason = model.Reason
        };

        var result = await _accountOrchestrator.DeactivateAccountAsync(request);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Account deactivated successfully. You can reactivate it by logging in again within 30 days.";
            return RedirectToAction("Logout", "Authentication", new { area = "" });
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
            return RedirectToAction("Login", "Authentication", new { area = "" });

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

        var result = await _accountOrchestrator.DeleteAccountAsync(request);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Account deleted successfully. We're sorry to see you go!";
            return RedirectToAction("Login", "Authentication", new { area = "" });
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
            return BadRequest(ModelState);

        var request = new ExportUserDataRequest
        {
            UserId = userId,
            // Map other fields as needed
        };

        var result = await _accountOrchestrator.ExportUserDataAsync(request);
        if (result.Succeeded && result.Data is byte[] data)
        {
            var fileName = $"user-data-export-{DateTime.UtcNow:yyyyMMdd-HHmmss}.zip";
            return File(data, "application/zip", fileName);
        }

        return BadRequest(new { success = false, message = result.Message });
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


