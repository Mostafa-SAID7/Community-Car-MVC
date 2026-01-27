using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;
using CommunityCar.Domain.Entities.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Services.Authentication.Login;

/// <summary>
/// Service for user login operations
/// </summary>
public class LoginService : ILoginService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<LoginService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoginService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ILogger<LoginService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure("Invalid email or password.");
            }

            if (!user.IsActive)
            {
                return Result.Failure("Your account has been deactivated.");
            }

            var result = await _signInManager.PasswordSignInAsync(
                user, request.Password, request.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {Email} logged in successfully", request.Email);
                return Result.Success("Login successful.");
            }

            if (result.RequiresTwoFactor)
            {
                return Result.Failure("Two-factor authentication required.");
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User {Email} account locked out", request.Email);
                return Result.Failure("Account locked due to multiple failed login attempts.");
            }

            if (result.IsNotAllowed)
            {
                return Result.Failure("Email confirmation required. Please check your email.");
            }

            return Result.Failure("Invalid email or password.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CRITICAL ERROR: Exception during login for {Email}. Message: {ErrorMessage}", request.Email, ex.Message);
            return Result.Failure("An error occurred during login. Please try again later or contact support if the issue persists.");
        }
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        return await _userManager.FindByIdAsync(userId);
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out");
    }
}
