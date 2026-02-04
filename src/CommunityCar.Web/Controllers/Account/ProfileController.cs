using CommunityCar.Application.Common.Interfaces.Services.Account.Profile;
using CommunityCar.Application.Common.Interfaces.Services.Account.Media;
using CommunityCar.Application.Common.Interfaces.Services.Account.Gamification;
using CommunityCar.Application.Common.Interfaces.Services.Account.Core;
using CommunityCar.Application.Features.Account.ViewModels.Core;
using CommunityCar.Application.Features.Account.ViewModels.Media;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Account;

[Route("{culture}/profile")]
[Authorize]
public class ProfileController : Controller
{
    private readonly IProfileService _profileService;
    private readonly IUserGalleryService _userGalleryService;
    private readonly IGamificationService _gamificationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(
        IProfileService profileService,
        IUserGalleryService userGalleryService,
        IGamificationService gamificationService,
        ICurrentUserService currentUserService,
        ILogger<ProfileController> logger)
    {
        _profileService = profileService;
        _userGalleryService = userGalleryService;
        _gamificationService = gamificationService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    private async Task SetProfileHeaderDataAsync(Guid userId)
    {
        try
        {
            var profile = await _profileService.GetProfileAsync(userId);
            if (profile != null)
            {
                ViewBag.FullName = profile.FullName;
                ViewBag.Email = profile.Email;
                ViewBag.UserName = profile.UserName;
                ViewBag.Bio = profile.Bio;
                ViewBag.BioAr = profile.BioAr;
                ViewBag.City = profile.City;
                ViewBag.Country = profile.Country;
                ViewBag.ProfilePictureUrl = profile.ProfilePictureUrl;
                ViewBag.CoverImageUrl = profile.CoverImageUrl;
                ViewBag.CreatedAt = profile.CreatedAt;
                ViewBag.PostsCount = profile.PostsCount;
                ViewBag.CommentsCount = profile.CommentsCount;
                ViewBag.LikesReceived = profile.LikesReceived;
                
                // Gamification Stats
                var stats = await _gamificationService.GetUserStatsAsync(userId);
                if (stats != null)
                {
                    ViewBag.Level = stats.Level;
                    ViewBag.TotalPoints = stats.TotalPoints;
                    ViewBag.Rank = stats.Rank;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load profile header data for user {UserId}", userId);
            // Set default values
            ViewBag.FullName = "User Profile";
            ViewBag.Email = "user@example.com";
            ViewBag.ProfilePictureUrl = null;
            ViewBag.CreatedAt = DateTime.Now;
            ViewBag.PostsCount = 0;
            ViewBag.CommentsCount = 0;
            ViewBag.LikesReceived = 0;
        }
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string culture)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return RedirectToAction("Login", "Account", new { culture });
        }

        var profile = await _profileService.GetProfileAsync(userId);
        if (profile != null && !string.IsNullOrEmpty(profile.Slug))
        {
            return RedirectToAction(nameof(ViewProfileBySlug), new { culture, slug = profile.Slug });
        }

        return RedirectToAction(nameof(ViewProfile), new { culture, id = userId });
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> ViewProfileBySlug(string culture, string slug)
    {
        var profile = await _profileService.GetProfileBySlugAsync(slug);
        if (profile == null)
        {
            return NotFound();
        }

        var currentUserId = Guid.TryParse(_currentUserService.UserId, out var currentId) ? currentId : Guid.Empty;
        
        await SetProfileHeaderDataAsync(profile.Id);
        ViewBag.UserId = profile.Id;
        ViewBag.IsOwner = currentUserId == profile.Id;

        var viewModel = new ProfileIndexVM
        {
            Id = profile.Id,
            Slug = profile.Slug,
            FullName = profile.FullName,
            Email = profile.Email,
            PhoneNumber = profile.PhoneNumber,
            Bio = profile.Bio,
            City = profile.City,
            Country = profile.Country,
            ProfilePictureUrl = profile.ProfilePictureUrl,
            CreatedAt = profile.CreatedAt,
            IsEmailConfirmed = profile.IsEmailConfirmed,
            IsActive = profile.IsActive,
            PostsCount = profile.PostsCount,
        };

        return View("~/Views/Account/Profile/Index.cshtml", viewModel);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ViewProfile(string culture, Guid id)
    {
        var profile = await _profileService.GetProfileAsync(id);
        if (profile == null)
        {
            var fallbackVM = new ProfileIndexVM
            {
                Id = id,
                FullName = "Unknown User",
                Email = "hidden",
                Bio = "This user profile could not be found or is no longer active.",
                City = "Unknown",
                Country = "Unknown",
                CreatedAt = DateTime.UtcNow,
                PostsCount = 0,
                CommentsCount = 0,
                LikesReceived = 0
            };
            return View("~/Views/Account/Profile/Index.cshtml", fallbackVM);
        }

        var currentUserId = Guid.TryParse(_currentUserService.UserId, out var currentId) ? currentId : Guid.Empty;
        
        await SetProfileHeaderDataAsync(id);
        ViewBag.UserId = id;
        ViewBag.IsOwner = currentUserId == id;

        var viewModel = new ProfileIndexVM
        {
            Id = profile.Id,
            FullName = profile.FullName,
            Email = profile.Email,
            PhoneNumber = profile.PhoneNumber,
            Bio = profile.Bio,
            City = profile.City,
            Country = profile.Country,
            ProfilePictureUrl = profile.ProfilePictureUrl,
            CreatedAt = profile.CreatedAt,
            IsEmailConfirmed = profile.IsEmailConfirmed,
            IsActive = profile.IsActive,
            PostsCount = profile.PostsCount,
        };

        return View("~/Views/Account/Profile/Index.cshtml", viewModel);
    }

    // Settings actions moved to ProfileSettingsController

    [HttpPost("update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(string culture, UpdateProfileVM model)
    {
        if (!ModelState.IsValid)
        {
            // If invalid, we need to redirect to settings with errors or re-render
            // Since settings are now in a separate controller, we might need to rely on TempData or return to that controller
            // For simplicity, let's redirect to settings
            TempData["ErrorMessage"] = "Invalid profile data.";
            return RedirectToAction("Index", "ProfileSettings");
        }

        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
             return RedirectToAction("Login", "Account", new { area = "" });
        }

        var nameParts = model.FullName.Split(' ', 2);
        var firstName = nameParts.Length > 0 ? nameParts[0] : "";
        var lastName = nameParts.Length > 1 ? nameParts[1] : "";

        var request = new UpdateProfileRequest
        {
            UserId = userId,
            FullName = model.FullName,
            PhoneNumber = model.PhoneNumber,
            Bio = model.Bio,
            City = model.City,
            Country = model.Country
        };

        var success = await _profileService.UpdateProfileAsync(request);
        if (success)
        {
            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Index), new { culture = System.Globalization.CultureInfo.CurrentCulture.Name });
        }

        TempData["ErrorMessage"] = "Failed to update profile. Please try again.";
        return RedirectToAction("Index", "ProfileSettings", new { culture = System.Globalization.CultureInfo.CurrentCulture.Name });
    }

    [HttpPost("upload-picture")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadProfilePicture(IFormFile profilePicture)
    {
        if (profilePicture == null || profilePicture.Length == 0)
        {
            TempData["ErrorMessage"] = "Please select a valid image file.";
            return RedirectToAction("Index", "ProfileSettings");
        }

        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
             return RedirectToAction("Login", "Account", new { area = "" });
        }

        // Placeholder URL logic - ideally this should be handled by the orchestrator/service fully including upload
        var imageUrl = $"/uploads/profiles/{userId}_{profilePicture.FileName}";
        var success = await _profileService.UpdateProfilePictureAsync(userId, imageUrl);
        
        if (success)
        {
            TempData["SuccessMessage"] = "Profile picture updated successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to update profile picture. Please try again.";
        }

        return RedirectToAction("Index", "ProfileSettings");
    }

    [HttpPost("delete-picture")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProfilePicture()
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
             return RedirectToAction("Login", "Account", new { area = "" });
        }

        var success = await _profileService.DeleteProfilePictureAsync(userId);
        
        if (success)
        {
            TempData["SuccessMessage"] = "Profile picture deleted successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to delete profile picture. Please try again.";
        }

        return RedirectToAction("Index", "ProfileSettings");
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(string culture)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return Json(new { success = false, message = "User not found" });
        }

        var stats = await _profileService.GetProfileStatsAsync(userId);
        return Json(new { success = true, data = stats });
    }

    [HttpGet("{id:guid}/interests")]
    public async Task<IActionResult> Interests(string culture, Guid id)
    {
        var currentUserId = Guid.TryParse(_currentUserService.UserId, out var currentId) ? currentId : Guid.Empty;
        
        await SetProfileHeaderDataAsync(id);
        ViewBag.UserId = id;
        ViewBag.IsOwner = currentUserId == id;
        
        return View("~/Views/Account/Profile/Interests.cshtml");
    }

    [HttpGet("{id:guid}/badges")]
    public async Task<IActionResult> Badges(string culture, Guid id)
    {
        var currentUserId = Guid.TryParse(_currentUserService.UserId, out var currentId) ? currentId : Guid.Empty;
        
        var badges = await _gamificationService.GetUserBadgesAsync(id);
        var achievements = await _gamificationService.GetUserAchievementsAsync(id);
        var stats = await _gamificationService.GetUserStatsAsync(id);

        await SetProfileHeaderDataAsync(id);
        ViewBag.UserId = id;
        ViewBag.IsOwner = currentUserId == id;
        ViewBag.Badges = badges;
        ViewBag.Achievements = achievements;
        ViewBag.GamificationStats = stats;

        return View("~/Views/Account/Profile/Badges.cshtml");
    }

    [HttpGet("gallery")]
    public async Task<IActionResult> Gallery(string culture)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
             return RedirectToAction("Login", "Account", new { culture });
        }

        return RedirectToAction("Index", "Gallery", new { culture, userId });
    }

    [HttpGet("{id:guid}/gallery")]
    public async Task<IActionResult> ViewUserGallery(string culture, Guid id)
    {
        var currentUserId = Guid.TryParse(_currentUserService.UserId, out var currentId) ? currentId : Guid.Empty;
        
        // Check if user exists
        var profile = await _profileService.GetProfileAsync(id);
        if (profile == null)
        {
            return NotFound("User not found");
        }

        // Get gallery items for the specified user
        var galleryItems = await _userGalleryService.GetUserGalleryAsync(id) ?? new List<UserGalleryItemVM>();
        var imageCount = await _userGalleryService.GetImageCountAsync(id);

        await SetProfileHeaderDataAsync(id);
        ViewBag.UserId = id;
        ViewBag.IsOwner = currentUserId == id;
        ViewBag.ImageCount = imageCount;

        return View("~/Views/Account/Profile/Gallery.cshtml", galleryItems);
    }

    [HttpPost("gallery/upload")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadGalleryItem(string culture, CommunityCar.Application.Features.Account.ViewModels.Media.UploadImageRequest request, IFormFile mediaFile)
    {
        if (mediaFile == null || mediaFile.Length == 0)
        {
            TempData["ErrorMessage"] = "Please select a valid media file.";
            return RedirectToAction("Gallery");
        }

        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
             return RedirectToAction("Login", "Authentication", new { area = "" });
        }

        try
        {
            using var stream = mediaFile.OpenReadStream();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var imageData = Convert.ToBase64String(memoryStream.ToArray());
            
            request.UserId = userId;
            request.ImageData = imageData;
            request.FileName = mediaFile.FileName;
            request.ContentType = mediaFile.ContentType;
            
            var success = await _userGalleryService.UploadImageAsync(request) != null;
            
            if (success)
            {
                TempData["SuccessMessage"] = "Media uploaded successfully!";
                // await _profileOrchestrator.ProcessUserActionAsync(userId, "gallery_upload");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to upload media. Please try again.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload gallery item for user {UserId}", userId);
            TempData["ErrorMessage"] = "Failed to upload media. Please try again.";
        }

        return RedirectToAction("Gallery", new { culture });
    }



    [HttpPost("gallery/toggle-visibility/{itemId}")]
    public async Task<IActionResult> ToggleGalleryItemVisibility(Guid itemId)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return Json(new { success = false, message = "User not found" });
        }

        var success = await _userGalleryService.ToggleItemVisibilityAsync(userId, itemId);
        return Json(new { success });
    }

    [HttpPost("gallery/delete/{itemId}")]
    public async Task<IActionResult> DeleteGalleryItem(Guid itemId)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return Json(new { success = false, message = "User not found" });
        }

        var success = await _userGalleryService.DeleteGalleryItemAsync(userId, itemId);
        return Json(new { success });
    }

    [HttpGet("gallery/item/{itemId}")]
    public IActionResult GetGalleryItem(Guid itemId)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return Unauthorized();
        }

        // var item = await _profileOrchestrator.GetGalleryItemAsync(userId, itemId);
        // if (item == null)
        // {
        //     return NotFound();
        // }

        return Json(new { success = false, message = "Not implemented" });
    }
}


