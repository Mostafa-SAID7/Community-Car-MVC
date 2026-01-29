using CommunityCar.Web.Models.Account.Authentication.Login;
using CommunityCar.Web.Models.Account.Authentication.Registration;
using CommunityCar.Web.Models.Account.Authentication.PasswordReset;
using CommunityCar.Web.Models.Account.Authentication.Login.External;
using CommunityCar.Web.Models.Account.Authentication.OAuth;
using CommunityCar.Application.Common.Interfaces.Orchestrators;
using CommunityCar.Application.Common.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Auth;

[Route("auth")]
public class AuthenticationController : Controller
{
    private readonly IAuthOrchestrator _authOrchestrator;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(
        IAuthOrchestrator authOrchestrator,
        ILogger<AuthenticationController> logger)
    {
        _authOrchestrator = authOrchestrator;
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
    public async Task<IActionResult> Register(RegisterVM model)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Feed");

        if (!ModelState.IsValid)
            return View(model);

        var request = new RegisterRequest
        {
            Email = model.Email,
            Password = model.Password,
            FullName = model.FullName,
            FirstName = model.FullName.Split(' ').FirstOrDefault() ?? model.FullName,
            LastName = model.FullName.Split(' ').Skip(1).FirstOrDefault() ?? ""
        };

        var result = await _authOrchestrator.RegisterAsync(request);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Registration successful! Please check your email to confirm your account.";
            return RedirectToAction("Login");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return View(model);
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
    public async Task<IActionResult> Login(LoginVM model, string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Feed");

        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        var request = new LoginRequest
        {
            LoginIdentifier = model.LoginIdentifier,
            Password = model.Password,
            RememberMe = model.RememberMe
        };

        var result = await _authOrchestrator.LoginAsync(request);
        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            
            return RedirectToAction("Index", "Feed");
        }

        ModelState.AddModelError(string.Empty, result.Message);
        return View(model);
    }

    #endregion

    #region Logout

    [HttpPost("logout")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _authOrchestrator.LogoutAsync();
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

        var result = await _authOrchestrator.ConfirmEmailAsync(userId, token);
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
    public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Feed");

        if (!ModelState.IsValid)
            return View(model);

        var result = await _authOrchestrator.ForgotPasswordAsync(model.Email);

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

        var model = new ResetPasswordVM
        {
            Token = token,
            UserId = userId
        };

        return View(model);
    }

    [HttpPost("reset-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Feed");

        if (!ModelState.IsValid)
            return View(model);

        var request = new ResetPasswordRequest
        {
            Email = model.Email,
            Token = model.Token,
            NewPassword = model.Password
        };

        var result = await _authOrchestrator.ResetPasswordAsync(request);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Password reset successfully! You can now log in with your new password.";
            return RedirectToAction("Login");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return View(model);
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
        var result = await _authOrchestrator.GoogleSignInAsync(request);

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
        var result = await _authOrchestrator.FacebookSignInAsync(request);

        if (result.Succeeded)
        {
            return Ok(new { success = true, message = result.Message });
        }

        return BadRequest(new { success = false, message = result.Message, errors = result.Errors });
    }

    #endregion
}


