using CommunityCar.Application.Common.Interfaces.Orchestrators;
using CommunityCar.Application.Common.Interfaces.Services;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Web.Models.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Profile;

[Route("profile/settings")]
[Authorize]
public class ProfileSettingsController : Controller
{
    private readonly IProfileOrchestrator _profileOrchestrator;
    private readonly IAccountOrchestrator _accountOrchestrator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ProfileSettingsController> _logger;

    public ProfileSettingsController(
        IProfileOrchestrator profileOrchestrator,
        IAccountOrchestrator accountOrchestrator,
        ICurrentUserService currentUserService,
        ILogger<ProfileSettingsController> logger)
    {
        _profileOrchestrator = profileOrchestrator;
        _accountOrchestrator = accountOrchestrator;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
             return RedirectToAction("Login", "Authentication", new { area = "" });

        var profile = await _profileOrchestrator.GetProfileSettingsAsync(userId);
        if (profile == null) return NotFound();

        return View("~/Views/Profile/Settings.cshtml", profile);
    }

    [HttpPost("privacy")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePrivacy(PrivacySettingsVM model)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
             return BadRequest("User not authenticated");

        if (!ModelState.IsValid)
             return BadRequest(ModelState);

        var request = new CommunityCar.Application.Common.Models.Account.UpdatePrivacySettingsRequest
        {
            UserId = userId,
            ProfileVisible = model.ProfileVisible,
            EmailVisible = model.EmailVisible,
            PhoneVisible = model.PhoneVisible,
            // AllowMessages = model.AllowMessages, // Not in Request
            // AllowFriendRequests = model.AllowFriendRequests // Not in Request
        };

        var result = await _accountOrchestrator.UpdatePrivacySettingsAsync(request);
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

        var request = new CommunityCar.Application.Common.Models.Account.UpdateNotificationSettingsRequest
        {
            UserId = userId,
            EmailNotifications = model.EmailNotifications,
            PushNotifications = model.PushNotifications,
            SmsNotifications = model.SmsNotifications,
            MarketingEmails = model.MarketingEmails
        };

        var result = await _accountOrchestrator.UpdateNotificationSettingsAsync(request);
        if (result.Succeeded)
             return Ok(new { success = true, message = "Notification settings updated." });

        return BadRequest(new { success = false, message = "Failed to update notification settings." });
    }
}



