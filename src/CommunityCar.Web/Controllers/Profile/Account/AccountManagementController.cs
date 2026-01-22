using CommunityCar.Application.Common.Interfaces.Services.Profile;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Profile.DTOs;
using CommunityCar.Web.Models.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Profile.Account;

[Route("profile/account")]
[Authorize]
public class AccountManagementController : Controller
{
    private readonly IAccountManagementService _accountManagementService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AccountManagementController> _logger;

    public AccountManagementController(
        IAccountManagementService accountManagementService,
        ICurrentUserService currentUserService,
        ILogger<AccountManagementController> logger)
    {
        _accountManagementService = accountManagementService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [HttpGet("delete")]
    public IActionResult DeleteAccount()
    {
        return View(new DeleteAccountVM());
    }

    [HttpPost("delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAccount(DeleteAccountVM model)
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
            var request = new DeleteAccountRequest
            {
                Password = model.Password,
                ConfirmDeletion = model.ConfirmDeletion
            };

            var success = await _accountManagementService.DeleteAccountAsync(userId, request);
            if (success)
            {
                TempData["SuccessMessage"] = "Your account has been deleted successfully.";
                return RedirectToAction("Logout", "Account");
            }
            else
            {
                ModelState.AddModelError("", "Failed to delete account. Please check your password and try again.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting account for user {UserId}", userId);
            ModelState.AddModelError("", "An error occurred while deleting your account.");
        }

        return View(model);
    }

    [HttpPost("deactivate")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeactivateAccount([FromForm] string password)
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
            var success = await _accountManagementService.DeactivateAccountAsync(userId, password);
            if (success)
            {
                return Json(new { success = true, message = "Account deactivated successfully", redirectUrl = Url.Action("Logout", "Account") });
            }
            else
            {
                return Json(new { success = false, message = "Invalid password" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating account for user {UserId}", userId);
            return Json(new { success = false, message = "An error occurred while deactivating your account" });
        }
    }
}