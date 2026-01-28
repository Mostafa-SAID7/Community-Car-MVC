using CommunityCar.Application.Common.Interfaces.Services.Authentication;
using CommunityCar.Application.Common.Interfaces.Services.Communication;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;
using CommunityCar.Domain.Entities.Account.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IEmailService _emailService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IEmailService emailService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthenticationService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    #region Registration

    public async Task<Result> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null) return Result.Failure("User with this email already exists.");

        var user = new User(request.Email, request.Email, request.FullName) { EmailConfirmed = false };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded) return Result.Failure("Registration failed.", result.Errors.Select(e => e.Description).ToList());

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var link = GenerateLink("Account/ConfirmEmail", user.Id.ToString(), token);
        await _emailService.SendEmailConfirmationAsync(user.Email!, link);

        return Result.Success("Registration successful. Please check your email to confirm your account.");
    }

    public async Task<Result> ConfirmEmailAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Result.Failure("User not found.");

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded) return Result.Failure("Email confirmation failed.", result.Errors.Select(e => e.Description).ToList());

        await _emailService.SendWelcomeEmailAsync(user.Email!, user.Profile.FullName);
        return Result.Success("Email confirmed successfully. You can now log in.");
    }

    public async Task<Result> ResendEmailConfirmationAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return Result.Success("If an account exists, a confirmation email has been sent.");
        if (user.EmailConfirmed) return Result.Failure("Email is already confirmed.");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var link = GenerateLink("Account/ConfirmEmail", user.Id.ToString(), token);
        await _emailService.SendEmailConfirmationAsync(user.Email!, link);

        return Result.Success("Confirmation email sent.");
    }

    #endregion

    #region Login & Logout

    public async Task<Result> LoginAsync(LoginRequest request)
    {
        var user = await FindUserByLoginIdentifierAsync(request.LoginIdentifier);
        if (user == null) return Result.Failure("Invalid login credentials.");
        if (!user.IsActive) return Result.Failure("Your account has been deactivated.");

        var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, true);
        if (result.Succeeded) 
        {
            user.UpdateLastLogin();
            await _userManager.UpdateAsync(user);
            return Result.Success("Login successful.");
        }
        if (result.RequiresTwoFactor) return Result.Failure("Two-factor authentication required.");
        if (result.IsLockedOut) return Result.Failure("Account locked due to multiple failed login attempts.");
        if (result.IsNotAllowed) return Result.Failure("Email confirmation required.");

        return Result.Failure("Invalid login credentials.");
    }

    public async Task LogoutAsync() => await _signInManager.SignOutAsync();

    public async Task<User?> GetCurrentUserAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return string.IsNullOrEmpty(userId) ? null : await _userManager.FindByIdAsync(userId);
    }

    #endregion

    #region Password Management

    public async Task<Result> ForgotPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return Result.Success("If an account exists, a reset link has been sent.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var link = GenerateLink("Account/ResetPassword", user.Id.ToString(), token);
        await _emailService.SendPasswordResetAsync(user.Email!, link);

        return Result.Success("If an account exists, a reset link has been sent.");
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return Result.Failure("Invalid reset token.");

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        return result.Succeeded ? Result.Success("Password reset successfully.") : Result.Failure("Password reset failed.", result.Errors.Select(e => e.Description).ToList());
    }

    #endregion

    #region Helper Methods

    private async Task<User?> FindUserByLoginIdentifierAsync(string loginIdentifier)
    {
        // Try to determine the type of identifier and find the user accordingly
        
        // Check if it's an email (contains @ symbol)
        if (loginIdentifier.Contains('@'))
        {
            return await _userManager.FindByEmailAsync(loginIdentifier);
        }
        
        // Check if it's a phone number (starts with + or contains only digits and common phone chars)
        if (IsPhoneNumber(loginIdentifier))
        {
            var users = _userManager.Users.Where(u => u.PhoneNumber == loginIdentifier).ToList();
            return users.FirstOrDefault();
        }
        
        // Otherwise, treat it as a username
        return await _userManager.FindByNameAsync(loginIdentifier);
    }

    private static bool IsPhoneNumber(string input)
    {
        // Simple phone number detection - starts with + or contains only digits, spaces, dashes, parentheses
        if (string.IsNullOrWhiteSpace(input)) return false;
        
        // Remove common phone number formatting characters
        var cleaned = input.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "").Replace("+", "");
        
        // Check if what remains is all digits and has reasonable length for a phone number
        return cleaned.All(char.IsDigit) && cleaned.Length >= 7 && cleaned.Length <= 15;
    }

    #endregion

    private string GenerateLink(string path, string userId, string token)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        var baseUrl = $"{request?.Scheme}://{request?.Host}";
        return $"{baseUrl}/{path}?userId={userId}&token={Uri.EscapeDataString(token)}";
    }
}
