using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Features.Account.ViewModels.Authentication;
using CommunityCar.Web.Extensions;
using CommunityCar.Web.Helpers;
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

    [HttpGet("register")]
    public IActionResult Register()
    {
        if (User.IsAuthenticated())
        {
            var culture = CultureHelper.GetCurrentCulture().Name;
            return RedirectToAction("Index", "Feed", new { culture });
        }
        
        // Set page metadata using ViewBagHelper
        ViewBagHelper.SetPageMetadata(ViewBag, "Register", "Create your Community Car account");
        ViewBagHelper.SetFormData(ViewBag, isEdit: false);
        
        return View("Auth/Register");
    }

    [HttpPost("register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVM model)
    {
        if (User.IsAuthenticated())
        {
            var culture = CultureHelper.GetCurrentCulture().Name;
            return RedirectToAction("Index", "Feed", new { culture });
        }

        if (!ModelState.IsValid)
        {
            if (Request.IsAjaxRequest())
                return JsonResponseHelper.ValidationError(ValidationHelper.GetModelStateErrors(ModelState));
            
            ViewBagHelper.SetPageMetadata(ViewBag, "Register", "Create your Community Car account");
            ViewBagHelper.SetFormData(ViewBag, isEdit: false);
            return View("Auth/Register", model);
        }

        // Additional validation using ValidationHelper
        var emailValidation = ValidationHelper.IsValidEmail(model.Email);
        var passwordValidation = ValidationHelper.ValidatePassword(model.Password);
        
        if (!emailValidation)
        {
            ModelState.AddModelError(nameof(model.Email), "Please enter a valid email address");
        }
        
        if (!passwordValidation.IsValid)
        {
            foreach (var error in passwordValidation.Errors)
            {
                ModelState.AddModelError(nameof(model.Password), error);
            }
        }

        if (!ModelState.IsValid)
        {
            if (Request.IsAjaxRequest())
                return JsonResponseHelper.ValidationError(ValidationHelper.GetModelStateErrors(ModelState));
            
            ViewBagHelper.SetPageMetadata(ViewBag, "Register", "Create your Community Car account");
            ViewBagHelper.SetFormData(ViewBag, isEdit: false);
            return View("Auth/Register", model);
        }

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
            if (Request.IsAjaxRequest())
                return JsonResponseHelper.Success("Registration successful! Please check your email to confirm your account.");
            
            this.SetSuccessMessage(result.Message ?? "Registration successful! Please check your email to confirm your account.");
            return RedirectToAction("Login");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        if (Request.IsAjaxRequest())
            return JsonResponseHelper.ValidationError(ValidationHelper.GetModelStateErrors(ModelState));

        ViewBagHelper.SetPageMetadata(ViewBag, "Register", "Create your Community Car account");
        ViewBagHelper.SetFormData(ViewBag, isEdit: false);
        return View("Auth/Register", model);
    }

    #endregion

    #region Login

    [HttpGet("login")]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.IsAuthenticated())
        {
            var culture = CultureHelper.GetCurrentCulture().Name;
            return RedirectToAction("Index", "Feed", new { culture });
        }

        // Use UrlHelper to generate safe return URL
        var safeReturnUrl = UrlHelper.GenerateReturnUrl(Url, returnUrl, "Index", "Feed");
        ViewData["ReturnUrl"] = safeReturnUrl;
        
        // Set page metadata using ViewBagHelper
        ViewBagHelper.SetPageMetadata(ViewBag, "Login", "Sign in to your Community Car account");
        ViewBagHelper.SetFormData(ViewBag, isEdit: false);
        
        return View("Auth/Login");
    }

    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVM model, string? returnUrl = null)
    {
        if (User.IsAuthenticated())
        {
            var culture = CultureHelper.GetCurrentCulture().Name;
            return RedirectToAction("Index", "Feed", new { culture });
        }

        var safeReturnUrl = UrlHelper.GenerateReturnUrl(Url, returnUrl, "Index", "Feed");
        ViewData["ReturnUrl"] = safeReturnUrl;

        if (!ModelState.IsValid)
        {
            if (Request.IsAjaxRequest())
                return this.JsonError("Please correct the validation errors.", ModelState.GetValidationErrors());
            
            return View("Auth/Login", model);
        }

        var request = new LoginRequest
        {
            LoginIdentifier = model.LoginIdentifier,
            Password = model.Password,
            RememberMe = model.RememberMe
        };

        var result = await _authService.LoginAsync(request);
        if (result.Succeeded)
        {
            var redirectUrl = !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) 
                ? returnUrl 
                : Url.Action("Index", "Feed", new { culture = System.Globalization.CultureInfo.CurrentCulture.Name });

            if (Request.IsAjaxRequest())
                return this.JsonSuccess("Login successful", redirectUrl);
            
            return Redirect(redirectUrl!);
        }

        var errorMessage = result.Message ?? "Invalid login attempt.";
        if (Request.IsAjaxRequest())
            return this.JsonError(errorMessage);

        ModelState.AddModelError(string.Empty, errorMessage);
        return View("Auth/Login", model);
    }

    #endregion

    #region Logout

    [HttpPost("logout")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
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

        var result = await _authService.ConfirmEmailAsync(userId, token);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = result.Message ?? "Email confirmed successfully! You can now log in.";
        }
        else
        {
            TempData["ErrorMessage"] = result.Message ?? "Email confirmation failed.";
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
        
        return View("Auth/ForgotPassword");
    }

    [HttpPost("forgot-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Feed");

        if (!ModelState.IsValid)
            return View("Auth/ForgotPassword", model);

        var result = await _authService.ForgotPasswordAsync(model.Email);

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

        return View("Auth/ResetPassword", model);
    }

    [HttpPost("reset-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Feed");

        if (!ModelState.IsValid)
            return View("Auth/ResetPassword", model);

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
            return RedirectToAction("Login");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return View("Auth/ResetPassword", model);
    }

    #endregion

    #region External Login

    [HttpPost("external-login")]
    [ValidateAntiForgeryToken]
    public IActionResult ExternalLogin(string provider, string? returnUrl = null)
    {
        _logger.LogInformation("External login initiated. Provider: {Provider}, ReturnUrl: {ReturnUrl}", provider, returnUrl);
        
        // Validate provider
        if (string.IsNullOrEmpty(provider))
        {
            _logger.LogError("External login provider is null or empty");
            TempData["ErrorMessage"] = "Invalid login provider.";
            return RedirectToAction("Login");
        }

        // Ensure we have the correct redirect URL
        var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl });
        _logger.LogInformation("External login redirect URL: {RedirectUrl}", redirectUrl);
        
        try
        {
            var properties = _authService.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            _logger.LogInformation("External authentication properties configured for provider: {Provider}", provider);
            return Challenge(properties, provider);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error configuring external authentication properties for provider: {Provider}", provider);
            TempData["ErrorMessage"] = $"Error initiating {provider} login. Please try again.";
            return RedirectToAction("Login");
        }
    }

    [HttpGet("external-login-callback")]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
    {
        _logger.LogInformation("External login callback started. ReturnUrl: {ReturnUrl}, RemoteError: {RemoteError}", returnUrl, remoteError);
        
        returnUrl = returnUrl ?? Url.Action("Index", "Feed", new { culture = System.Globalization.CultureInfo.CurrentCulture.Name }) ?? "/en/feed";

        if (remoteError != null)
        {
            _logger.LogError("External login failed with remote error: {RemoteError}", remoteError);
            TempData["ErrorMessage"] = $"Google login failed: {remoteError}";
            return RedirectToAction("Login");
        }

        try
        {
            // Get external login info first
            var info = await _authService.GetExternalLoginInfoAsync();
            if (info == null)
            {
                _logger.LogError("External login info is null - this usually means the OAuth callback failed or was invalid");
                TempData["ErrorMessage"] = "Error loading Google login information. Please try again.";
                return RedirectToAction("Login");
            }

            _logger.LogInformation("External login info received. Provider: {Provider}, ProviderKey: {ProviderKey}, Email: {Email}", 
                info.LoginProvider, info.ProviderKey, info.Principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value);

            // Try to sign in with existing external login
            var result = await _authService.ExternalLoginSignInAsync();
            _logger.LogInformation("External login sign-in result: Succeeded={Succeeded}, IsLockedOut={IsLockedOut}, IsNotAllowed={IsNotAllowed}", 
                result.Succeeded, result.IsLockedOut, result.IsNotAllowed);

            if (result.Succeeded)
            {
                _logger.LogInformation("External login successful, redirecting to: {ReturnUrl}", returnUrl);
                TempData["SuccessMessage"] = "Successfully signed in with Google!";
                return LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("External login failed: Account is locked out");
                TempData["ErrorMessage"] = "Your account is locked out. Please try again later.";
                return RedirectToAction("Login");
            }

            if (result.IsNotAllowed)
            {
                _logger.LogWarning("External login failed: Account is not allowed to sign in");
                TempData["ErrorMessage"] = "Your account is not allowed to sign in. Please contact support.";
                return RedirectToAction("Login");
            }

            // If we get here, the user doesn't have an account yet, so create one
            _logger.LogInformation("Creating new user with external login. Provider: {Provider}, Email: {Email}", 
                info.LoginProvider, info.Principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value);

            var createResult = await _authService.CreateUserWithExternalLoginAsync(info);
            if (createResult.Succeeded)
            {
                _logger.LogInformation("Successfully created user with external login");
                TempData["SuccessMessage"] = "Account created successfully! Welcome to CommunityCar!";
                return LocalRedirect(returnUrl);
            }

            // Handle specific error cases
            _logger.LogError("Failed to create user with external login. Errors: {Errors}", string.Join(", ", createResult.Errors));
            var errorMessage = "Failed to create account with Google login.";
            if (createResult.Errors.Any())
            {
                var firstError = createResult.Errors.First();
                if (firstError.Contains("already exists") || firstError.Contains("email"))
                {
                    errorMessage = "An account with this email already exists. Please try logging in with your password instead.";
                }
                else if (firstError.Contains("DuplicateUserName"))
                {
                    errorMessage = "An account with this information already exists. Please try logging in with your password instead.";
                }
            }

            TempData["ErrorMessage"] = errorMessage;
            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred during external login callback");
            TempData["ErrorMessage"] = "An unexpected error occurred during Google login. Please try again.";
            return RedirectToAction("Login");
        }
    }

    #endregion

    #region OAuth

    [HttpPost("google-signin")]
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

    [HttpPost("facebook-signin")]
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

    #region OAuth Debug

    [HttpGet("oauth-debug")]
    public IActionResult OAuthDebug()
    {
        try
        {
            var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var googleClientId = configuration.GetSection("SocialAuth:Google")["ClientId"];
            var googleClientSecret = configuration.GetSection("SocialAuth:Google")["ClientSecret"];
            
            var currentUrl = $"{Request.Scheme}://{Request.Host}";
            var expectedRedirectUri = $"{currentUrl}/signin-google";
            
            var debugInfo = new
            {
                GoogleClientId = googleClientId,
                GoogleClientSecretConfigured = !string.IsNullOrEmpty(googleClientSecret),
                CurrentBaseUrl = currentUrl,
                ExpectedRedirectUri = expectedRedirectUri,
                ActualCallbackUrl = Url.Action("ExternalLoginCallback", "Account"),
                IsHttps = Request.IsHttps,
                Host = Request.Host.ToString(),
                Scheme = Request.Scheme,
                UserAgent = Request.Headers["User-Agent"].ToString(),
                RemoteIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };
            
            _logger.LogInformation("OAuth Debug Info: {@DebugInfo}", debugInfo);
            return Json(debugInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OAuth debug endpoint");
            return Json(new { error = ex.Message });
        }
    }

    #endregion

    #region Access Denied

    [HttpGet("access-denied")]
    public IActionResult AccessDenied()
    {
        return View("Auth/AccessDenied");
    }

    #endregion
}
