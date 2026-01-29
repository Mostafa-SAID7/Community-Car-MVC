using CommunityCar.Application.Common.Extensions;
using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Interfaces.Services.Communication;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Features.Account.ViewModels.Authentication;
using CommunityCar.Domain.Entities.Account.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Services.Account;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IEmailService _emailService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IEmailService emailService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<Result> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null) return Result.Failure("User with this email already exists.");

        var user = new User(request.Email, request.Email, request.FullName) { EmailConfirmed = false };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded) return result.ToApplicationResult();

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var link = GenerateLink("auth/confirm-email", user.Id.ToString(), token);
        await _emailService.SendEmailConfirmationAsync(user.Email!, link);

        return Result.Success("Registration successful. Please check your email to confirm your account.");
    }

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

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<Result> ConfirmEmailAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Result.Failure("User not found.");

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded) return result.ToApplicationResult();

        await _emailService.SendWelcomeEmailAsync(user.Email!, user.Profile.FullName);
        return Result.Success("Email confirmed successfully. You can now log in.");
    }

    public async Task<Result> ResendEmailConfirmationAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return Result.Success("If an account exists, a confirmation email has been sent.");
        if (user.EmailConfirmed) return Result.Failure("Email is already confirmed.");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var link = GenerateLink("auth/confirm-email", user.Id.ToString(), token);
        await _emailService.SendEmailConfirmationAsync(user.Email!, link);

        return Result.Success("Confirmation email sent.");
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return string.IsNullOrEmpty(userId) ? null : await _userManager.FindByIdAsync(userId);
    }

    public async Task<Result> ForgotPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return Result.Success("If an account exists, a reset link has been sent.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var link = GenerateLink("auth/reset-password", user.Id.ToString(), token);
        await _emailService.SendPasswordResetAsync(user.Email!, link);

        return Result.Success("If an account exists, a reset link has been sent.");
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return Result.Failure("Invalid reset token.");

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        return result.Succeeded ? Result.Success("Password reset successfully.") : result.ToApplicationResult();
    }

    #region Helpers

    private async Task<User?> FindUserByLoginIdentifierAsync(string loginIdentifier)
    {
        if (loginIdentifier.Contains('@'))
        {
            return await _userManager.FindByEmailAsync(loginIdentifier);
        }
        
        if (IsPhoneNumber(loginIdentifier))
        {
            var user = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == loginIdentifier);
            return user;
        }
        
        return await _userManager.FindByNameAsync(loginIdentifier);
    }

    private static bool IsPhoneNumber(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        var cleaned = input.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "").Replace("+", "");
        return cleaned.All(char.IsDigit) && cleaned.Length >= 7 && cleaned.Length <= 15;
    }

    private string GenerateLink(string path, string userId, string token)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        var baseUrl = $"{request?.Scheme}://{request?.Host}";
        return $"{baseUrl}/{path}?userId={userId}&token={Uri.EscapeDataString(token)}";
    }

    #endregion
}
