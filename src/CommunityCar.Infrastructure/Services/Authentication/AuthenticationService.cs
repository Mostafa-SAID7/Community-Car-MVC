using CommunityCar.Application.Common.Interfaces.Services.Authentication;
using CommunityCar.Application.Common.Interfaces.Services.Communication;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;
using CommunityCar.Domain.Entities.Auth;
using CommunityCar.Domain.Entities.Profile;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IEmailService emailService,
        ILogger<AuthenticationService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return AuthResult.Failure("User with this email already exists.");
            }

            // Create new user
            var user = new User(request.Email, request.Email)
            {
                FullName = request.FullName,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return AuthResult.Failure("Registration failed.", errors);
            }

            // Generate email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = GenerateEmailConfirmationLink(user.Id.ToString(), token);

            // Send confirmation email
            await _emailService.SendEmailConfirmationAsync(user.Email!, confirmationLink);

            _logger.LogInformation("User {Email} registered successfully", request.Email);

            return AuthResult.Success("Registration successful. Please check your email to confirm your account.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration for {Email}", request.Email);
            return AuthResult.Failure("An error occurred during registration.");
        }
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return AuthResult.Failure("Invalid email or password.");
            }

            if (!user.IsActive)
            {
                return AuthResult.Failure("Your account has been deactivated.");
            }

            var result = await _signInManager.PasswordSignInAsync(
                user, request.Password, request.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {Email} logged in successfully", request.Email);
                return AuthResult.Success("Login successful.");
            }

            if (result.RequiresTwoFactor)
            {
                return AuthResult.Failure("Two-factor authentication required.");
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User {Email} account locked out", request.Email);
                return AuthResult.Failure("Account locked due to multiple failed login attempts.");
            }

            if (result.IsNotAllowed)
            {
                return AuthResult.Failure("Email confirmation required. Please check your email.");
            }

            return AuthResult.Failure("Invalid email or password.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", request.Email);
            return AuthResult.Failure("An error occurred during login.");
        }
    }

    public async Task<AuthResult> ConfirmEmailAsync(string userId, string token)
    {
        try
        {
            if (!Guid.TryParse(userId, out var userGuid))
            {
                return AuthResult.Failure("Invalid user ID.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return AuthResult.Failure("User not found.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                // Send welcome email
                await _emailService.SendWelcomeEmailAsync(user.Email!, user.FullName);
                
                _logger.LogInformation("Email confirmed for user {Email}", user.Email);
                return AuthResult.Success("Email confirmed successfully. You can now log in.");
            }

            var errors = result.Errors.Select(e => e.Description).ToList();
            return AuthResult.Failure("Email confirmation failed.", errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during email confirmation for user {UserId}", userId);
            return AuthResult.Failure("An error occurred during email confirmation.");
        }
    }

    public async Task<AuthResult> ForgotPasswordAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Don't reveal that the user doesn't exist
                return AuthResult.Success("If an account with that email exists, a password reset link has been sent.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = GeneratePasswordResetLink(user.Id.ToString(), token);

            await _emailService.SendPasswordResetAsync(user.Email!, resetLink);

            _logger.LogInformation("Password reset requested for user {Email}", email);
            return AuthResult.Success("If an account with that email exists, a password reset link has been sent.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset request for {Email}", email);
            return AuthResult.Failure("An error occurred while processing your request.");
        }
    }

    public async Task<AuthResult> ResetPasswordAsync(ResetPasswordRequest request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return AuthResult.Failure("Invalid reset token.");
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            if (result.Succeeded)
            {
                _logger.LogInformation("Password reset successfully for user {Email}", request.Email);
                return AuthResult.Success("Password reset successfully. You can now log in with your new password.");
            }

            var errors = result.Errors.Select(e => e.Description).ToList();
            return AuthResult.Failure("Password reset failed.", errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset for {Email}", request.Email);
            return AuthResult.Failure("An error occurred during password reset.");
        }
    }

    public async Task<AuthResult> ChangePasswordAsync(ChangePasswordRequest request)
    {
        try
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return AuthResult.Failure("User not found.");
            }

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                _logger.LogInformation("Password changed for user {Email}", user.Email);
                return AuthResult.Success("Password changed successfully.");
            }

            var errors = result.Errors.Select(e => e.Description).ToList();
            return AuthResult.Failure("Password change failed.", errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password change");
            return AuthResult.Failure("An error occurred during password change.");
        }
    }

    public async Task<AuthResult> ResendEmailConfirmationAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return AuthResult.Success("If an account with that email exists, a confirmation email has been sent.");
            }

            if (user.EmailConfirmed)
            {
                return AuthResult.Failure("Email is already confirmed.");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = GenerateEmailConfirmationLink(user.Id.ToString(), token);

            await _emailService.SendEmailConfirmationAsync(user.Email!, confirmationLink);

            return AuthResult.Success("Confirmation email sent.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resending email confirmation for {Email}", email);
            return AuthResult.Failure("An error occurred while sending confirmation email.");
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

    private string GenerateEmailConfirmationLink(string userId, string token)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        var baseUrl = $"{request?.Scheme}://{request?.Host}";
        return $"{baseUrl}/Account/ConfirmEmail?userId={userId}&token={Uri.EscapeDataString(token)}";
    }

    private string GeneratePasswordResetLink(string userId, string token)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        var baseUrl = $"{request?.Scheme}://{request?.Host}";
        return $"{baseUrl}/Account/ResetPassword?userId={userId}&token={Uri.EscapeDataString(token)}";
    }
}