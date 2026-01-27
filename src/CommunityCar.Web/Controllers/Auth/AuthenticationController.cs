using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Authentication;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Web.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Auth;

[Route("auth")]
public class AuthenticationController : Controller
{
    private readonly IIdentityOrchestrator _identityOrchestrator;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(
        IIdentityOrchestrator identityOrchestrator,
        ILogger<AuthenticationController> logger)
    {
        _identityOrchestrator = identityOrchestrator;
        _logger = logger;
    }

    #region Registration

    [HttpGet("register")]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Feed");
        
        return View();
    }

    [HttpPost("register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Feed");

        if (!ModelState.IsValid)
            return View(request);

        var result = await _identityOrchestrator.RegisterAsync(request);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Registration successful! Please check your email to confirm your account.";
            return RedirectToAction("Login");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return View(request);
    }

    #endregion

    #region Login

    [HttpGet("login")]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Feed");

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequest request, string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Feed");

        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(request);

        var result = await _identityOrchestrator.LoginAsync(request);
        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            
            return RedirectToAction("Index", "Feed");
        }

        ModelState.AddModelError(string.Empty, result.Message);
        return View(request);
    }

    #endregion

    #region Logout

    [HttpPost("logout")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _identityOrchestrator.LogoutAsync();
        return RedirectToAction("Login");
    }

    #endregion

    #region Email Confirmation

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Invalid email confirmation link.";
            return RedirectToAction("Login");
        }

        var result = await _identityOrchestrator.ConfirmEmailAsync(userId, token);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Email confirmed successfully! You can now log in.";
        }
        else
        {
            TempData["ErrorMessage"] = result.Message;
        }

        return RedirectToAction("Login");
    }

    #endregion

    #region Password Reset

    [HttpGet("forgot-password")]
    public IActionResult ForgotPassword()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Feed");
        
        return View();
    }

    [HttpPost("forgot-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Feed");

        if (!ModelState.IsValid)
            return View(request);

        var result = await _identityOrchestrator.ForgotPasswordAsync(request.Email);

        // Always show success message for security reasons
        TempData["SuccessMessage"] = "If an account with that email exists, we've sent password reset instructions.";
        return RedirectToAction("Login");
    }

    [HttpGet("reset-password")]
    public IActionResult ResetPassword(string userId, string token)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Feed");

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Invalid password reset link.";
            return RedirectToAction("Login");
        }

        var request = new ResetPasswordRequest
        {
            Token = token
        };

        return View(request);
    }

    [HttpPost("reset-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Feed");

        if (!ModelState.IsValid)
            return View(request);

        var result = await _identityOrchestrator.ResetPasswordAsync(request);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Password reset successfully! You can now log in with your new password.";
            return RedirectToAction("Login");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return View(request);
    }

    #endregion

    #region OAuth

    [HttpPost("oauth/google")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GoogleSignIn(GoogleSignInVM model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var request = new GoogleSignInRequest { IdToken = model.IdToken };
        var result = await _identityOrchestrator.GoogleSignInAsync(request);

        if (result.Succeeded)
        {
            return Ok(new { success = true, message = result.Message });
        }

        return BadRequest(new { success = false, message = result.Message, errors = result.Errors });
    }

    [HttpPost("oauth/facebook")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FacebookSignIn(FacebookSignInVM model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var request = new FacebookSignInRequest { AccessToken = model.AccessToken };
        var result = await _identityOrchestrator.FacebookSignInAsync(request);

        if (result.Succeeded)
        {
            return Ok(new { success = true, message = result.Message });
        }

        return BadRequest(new { success = false, message = result.Message, errors = result.Errors });
    }

    #endregion
}


