using CommunityCar.Application.Common.Interfaces.Services.Authentication;
using CommunityCar.Application.Common.Models.Authentication;
using CommunityCar.Domain.Entities.Auth;
using CommunityCar.Web.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Account;

public class AccountController : Controller
{
    private readonly IAuthenticationService _authService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IAuthenticationService authService, ILogger<AccountController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    [Route("Register")]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Feed");
        }
        return View();
    }

    [HttpPost]
    [Route("Register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVM model)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Feed");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var request = new RegisterRequest
        {
            Email = model.Email,
            Password = model.Password,
            FullName = model.FullName,
            FirstName = model.FirstName,
            LastName = model.LastName
        };

        var result = await _authService.RegisterAsync(request);

        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(RegisterConfirmation));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        if (!string.IsNullOrEmpty(result.Message))
        {
            ModelState.AddModelError(string.Empty, result.Message);
        }

        return View(model);
    }

    [HttpGet]
    [Route("RegisterConfirmation")]
    public IActionResult RegisterConfirmation()
    {
        return View();
    }

    [HttpGet]
    [Route("Login")]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Feed");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [Route("Login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVM model, string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Feed");
        }

        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var request = new LoginRequest
        {
            Email = model.Email,
            Password = model.Password,
            RememberMe = model.RememberMe
        };

        var result = await _authService.LoginAsync(request);

        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Feed");
        }

        ModelState.AddModelError(string.Empty, result.Message);
        return View(model);
    }

    [HttpPost]
    [Route("Logout")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return RedirectToAction("Index", "Feed");
    }

    [HttpGet]
    [Route("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        {
            return BadRequest("Invalid email confirmation link.");
        }

        var result = await _authService.ConfirmEmailAsync(userId, token);
        
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = result.Message;
        }
        else
        {
            TempData["ErrorMessage"] = result.Message;
        }

        return View();
    }

    [HttpGet]
    [Route("ForgotPassword")]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [Route("ForgotPassword")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authService.ForgotPasswordAsync(model.Email);
        TempData["SuccessMessage"] = result.Message;
        
        return RedirectToAction(nameof(ForgotPasswordConfirmation));
    }

    [HttpGet]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    [Route("ResetPassword")]
    public IActionResult ResetPassword(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        {
            return BadRequest("Invalid password reset link.");
        }

        var model = new ResetPasswordVM
        {
            Token = token,
            Email = "" // Will be populated from the token validation
        };

        return View(model);
    }

    [HttpPost]
    [Route("ResetPassword")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var request = new ResetPasswordRequest
        {
            Email = model.Email,
            Token = model.Token,
            NewPassword = model.Password
        };

        var result = await _authService.ResetPasswordAsync(request);

        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Login));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        ModelState.AddModelError(string.Empty, result.Message);
        return View(model);
    }

    [HttpGet]
    [Authorize]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var request = new ChangePasswordRequest
        {
            CurrentPassword = model.CurrentPassword,
            NewPassword = model.NewPassword
        };

        var result = await _authService.ChangePasswordAsync(request);

        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index", "Profile");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        ModelState.AddModelError(string.Empty, result.Message);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendEmailConfirmation(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return BadRequest("Email is required.");
        }

        var result = await _authService.ResendEmailConfirmationAsync(email);
        TempData["SuccessMessage"] = result.Message;
        
        return RedirectToAction(nameof(RegisterConfirmation));
    }

    [HttpGet]
    [Route("AccessDenied")]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
