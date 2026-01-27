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
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ProfileSettingsController> _logger;

    public ProfileSettingsController(
        IProfileOrchestrator profileOrchestrator,
        ICurrentUserService currentUserService,
        ILogger<ProfileSettingsController> logger)
    {
        _profileOrchestrator = profileOrchestrator;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
             return RedirectToAction("Login", "Authentication", new { area = "" });

        var profile = await _profileOrchestrator.GetProfileAsync(userId);
        if (profile == null) return NotFound();

        var model = new ProfileSettingsVM
        {
            Id = profile.Id,
            FullName = profile.FullName,
            Email = profile.Email,
            PhoneNumber = profile.PhoneNumber,
            Bio = profile.Bio,
            City = profile.City,
            Country = profile.Country,
            BioAr = profile.BioAr,
            CityAr = profile.CityAr,
            CountryAr = profile.CountryAr,
            ProfilePictureUrl = profile.ProfilePictureUrl,
            IsEmailConfirmed = profile.IsEmailConfirmed,
            IsPhoneNumberConfirmed = profile.IsPhoneNumberConfirmed,
            IsTwoFactorEnabled = profile.IsTwoFactorEnabled,
            HasGoogleAccount = profile.HasGoogleAccount,
            HasFacebookAccount = profile.HasFacebookAccount,
            LastPasswordChange = profile.LastPasswordChange,
            ActiveSessions = 1 // Placeholder
        };

        return View("~/Views/Profile/Settings.cshtml", model);
    }

    [HttpPost("privacy")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePrivacy(PrivacySettingsVM model)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
             return BadRequest("User not authenticated");

        if (!ModelState.IsValid)
             return BadRequest(ModelState);

        var request = new UpdatePrivacySettingsRequest
        {
            UserId = userId,
            ProfileVisible = model.ProfileVisible,
            EmailVisible = model.EmailVisible,
            PhoneVisible = model.PhoneVisible,
            AllowMessages = model.AllowMessages,
            AllowFriendRequests = model.AllowFriendRequests
        };

        var result = await _profileOrchestrator.UpdatePrivacySettingsAsync(userId, request);
        if (result)
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

        var request = new UpdateNotificationSettingsRequest
        {
            UserId = userId,
            EmailNotifications = model.EmailNotifications,
            PushNotifications = model.PushNotifications,
            SmsNotifications = model.SmsNotifications,
            MarketingEmails = model.MarketingEmails
        };

        var result = await _profileOrchestrator.UpdateNotificationSettingsAsync(userId, request);
        if (result)
             return Ok(new { success = true, message = "Notification settings updated." });

        return BadRequest(new { success = false, message = "Failed to update notification settings." });
    }
}



