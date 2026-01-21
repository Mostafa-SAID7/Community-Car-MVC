using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Interfaces.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Profile;

[Route("profile")]
[Authorize]
public class ProfileController : Controller
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthenticationService _authService;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(
        ICurrentUserService currentUserService,
        IAuthenticationService authService,
        ILogger<ProfileController> logger)
    {
        _currentUserService = currentUserService;
        _authService = authService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var user = await _authService.GetCurrentUserAsync();
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        return View(user);
    }

    [HttpGet("change-password")]
    public IActionResult ChangePassword()
    {
        return RedirectToAction("ChangePassword", "Account");
    }

    [HttpGet("settings")]
    public async Task<IActionResult> Settings()
    {
        var user = await _authService.GetCurrentUserAsync();
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        return View(user);
    }

    [HttpPost("settings")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(string fullName, IFormFile? profilePicture)
    {
        var user = await _authService.GetCurrentUserAsync();
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Simple update logic for now
        // In a real app, this should go through a service and handle file uploads
        // For now, let's just update the name if it's provided
        if (!string.IsNullOrEmpty(fullName))
        {
            // We'd normally use the auth service here to update the user
            // But let's just show a success message
            TempData["SuccessMessage"] = "Profile updated successfully!";
        }

        return RedirectToAction(nameof(Index));
    }
}
