using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Account;

[Route("profile/settings")]
[Authorize]
public class ProfileSettingsController : Controller
{
    private readonly IProfileService _profileService;
    private readonly IAccountManagementService _accountManagementService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ProfileSettingsController> _logger;

    public ProfileSettingsController(
        IProfileService profileService,
        IAccountManagementService accountManagementService,
        ICurrentUserService currentUserService,
        ILogger<ProfileSettingsController> logger)
    {
        _profileService = profileService;
        _accountManagementService = accountManagementService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
             return RedirectToAction("Login", "Account", new { area = "" });

        var profile = await _profileService.GetProfileAsync(userId);
        if (profile == null) return NotFound();

        var privacy = await _accountManagementService.GetPrivacySettingsAsync(userId);
        var notifications = await _accountManagementService.GetNotificationSettingsAsync(userId);

        var viewModel = new ProfileSettingsVM
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
            CoverImageUrl = profile.CoverImageUrl,
            IsEmailConfirmed = profile.IsEmailConfirmed,
            IsPhoneNumberConfirmed = profile.IsPhoneNumberConfirmed,
            IsTwoFactorEnabled = profile.IsTwoFactorEnabled,
            HasGoogleAccount = profile.HasGoogleAccount,
            HasFacebookAccount = profile.HasFacebookAccount,
            
            // Privacy
            PublicProfile = privacy.ProfileVisible,
            // EmailVisible = privacy.EmailVisible,
            // PhoneVisible = privacy.PhoneVisible,
            
            // Notifications
            EmailNotifications = notifications.EmailNotifications,
            PushNotifications = notifications.PushNotifications,
            SmsNotifications = notifications.SmsNotifications,
            MarketingEmails = notifications.MarketingEmails
        };

        return View("~/Views/Account/Profile/Settings.cshtml", viewModel);
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
            IsPublic = model.IsPublic,
            AllowMessages = model.AllowMessages,
            AllowFriendRequests = model.AllowFriendRequests,
            DefaultGalleryPrivacy = model.DefaultGalleryPrivacy,
            ShowActivityStatus = model.ShowActivityStatus,
            ShowOnlineStatus = model.ShowOnlineStatus
        };

        var result = await _accountManagementService.UpdatePrivacySettingsAsync(request);
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

        var request = new UpdateNotificationSettingsRequest
        {
            UserId = userId,
            EmailNotifications = model.EmailNotifications,
            PushNotifications = model.PushNotifications,
            SmsNotifications = model.SmsNotifications,
            MarketingEmails = model.MarketingEmails,
            WeeklyDigest = model.WeeklyDigest,
            SecurityAlerts = model.SecurityAlerts
        };

        var result = await _accountManagementService.UpdateNotificationSettingsAsync(request);
        if (result.Succeeded)
             return Ok(new { success = true, message = "Notification settings updated." });

        return BadRequest(new { success = false, message = "Failed to update notification settings." });
    }
}



