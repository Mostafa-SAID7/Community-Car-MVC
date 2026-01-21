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
}
