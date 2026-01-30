using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Features.Account.ViewModels.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Web.Controllers.Account;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly IOAuthService _oAuthService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        IAuthService authService,
        IOAuthService oAuthService,
        ILogger<AccountController> logger)
    {
        _authService = authService;
        _oAuthService = oAuthService;
        _logger = logger;
    }

    #region Registration

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Feed");
        
        return View("~/Views/Account/Register.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVM model)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Feed");

        if (!ModelState.IsValid)
            return View("~/Views/Account/Register.cshtml", model);

        var request = new RegisterRequest
        {
            Email = model.Email,
            Password = model.Password,
            FullName = model.FullName,
            FirstName = model.FullName.Split(' ').FirstOrDefault() ?? model.FullName,
            LastName = model.FullName.Split(' ').Skip(1).FirstOrDefault() ?? ""
        };

        var result = await _authService.RegisterAsync(request);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = result.Message ?? "Registration successful! Please check your email to confirm your account.";
            return RedirectToAction("Login", "Account");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return View("~/Views/Account/Register.cshtml", model);
    }

    #endregion

    #region Login

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Feed");

        ViewData["ReturnUrl"] = returnUrl;
        return View("~/Views/Account/Login.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVM model, string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Feed");

        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View("~/Views/Account/Login.cshtml", model);

        var request = new LoginRequest
        {
            LoginIdentifier = model.LoginIdentifier,
            Password = model.Password,
            RememberMe = model.RememberMe
        };

        var result = await _authService.LoginAsync(request);
        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            
            return RedirectToAction("Index", "Feed");
        }

        ModelState.AddModelError(string.Empty, result.Message ?? "Invalid login attempt.");
        return View(model);
    }

    #endregion

    #region Logout

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return RedirectToAction("Login", "Account");
    }

    #endregion

    #region Email Confirmation

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Invalid email confirmation link.";
            return RedirectToAction("Login", "Account");
        }

        var result = await _authService.ConfirmEmailAsync(userId, token);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = result.Message ?? "Email confirmed successfully! You can now log in.";
        }
        else
        {
            TempData["ErrorMessage"] = result.Message ?? "Email confirmation failed.";
        }

        return RedirectToAction("Login", "Account");
    }

    #endregion

    #region Password Reset

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Feed");
        
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Feed");

        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.ForgotPasswordAsync(model.Email);

        // Always show success message for security reasons
        TempData["SuccessMessage"] = "If an account with that email exists, we've sent password reset instructions.";
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    public IActionResult ResetPassword(string userId, string token)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Feed");

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Invalid password reset link.";
            return RedirectToAction("Login", "Account");
        }

        var model = new ResetPasswordVM
        {
            Token = token,
            UserId = userId
        };

        return View(model);
    }

    [HttpPost]
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
            NewPassword = model.NewPassword
        };

        var result = await _authService.ResetPasswordAsync(request);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = result.Message ?? "Password reset successfully! You can now log in with your new password.";
            return RedirectToAction("Login", "Account");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return View(model);
    }

    #endregion

    #region OAuth

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GoogleSignIn(GoogleSignInVM model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var request = new GoogleSignInRequest { IdToken = model.IdToken };
        var result = await _oAuthService.GoogleSignInAsync(request);

        if (result.Succeeded)
        {
            return Ok(new { success = true, message = result.Message });
        }

        return BadRequest(new { success = false, message = result.Message, errors = result.Errors });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FacebookSignIn(FacebookSignInVM model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var request = new FacebookSignInRequest { AccessToken = model.AccessToken };
        var result = await _oAuthService.FacebookSignInAsync(request);

        if (result.Succeeded)
        {
            return Ok(new { success = true, message = result.Message });
        }

        return BadRequest(new { success = false, message = result.Message, errors = result.Errors });
    }

    #endregion
}
