using CommunityCar.Application.Common.Interfaces.Services.Communication;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;
using CommunityCar.Domain.Entities.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Services.Authentication.PasswordReset;

/// <summary>
/// Service for password reset operations
/// </summary>
public class PasswordResetService : IPasswordResetService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<PasswordResetService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PasswordResetService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IEmailService emailService,
        ILogger<PasswordResetService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> ForgotPasswordAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Don't reveal that the user doesn't exist
                return Result.Success("If an account with that email exists, a password reset link has been sent.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = GeneratePasswordResetLink(user.Id.ToString(), token);

            await _emailService.SendPasswordResetAsync(user.Email!, resetLink);

            _logger.LogInformation("Password reset requested for user {Email}", email);
            return Result.Success("If an account with that email exists, a password reset link has been sent.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset request for {Email}", email);
            return Result.Failure("An error occurred while processing your request.");
        }
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure("Invalid reset token.");
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            if (result.Succeeded)
            {
                _logger.LogInformation("Password reset successfully for user {Email}", request.Email);
                return Result.Success("Password reset successfully. You can now log in with your new password.");
            }

            var errors = result.Errors.Select(e => e.Description).ToList();
            return Result.Failure("Password reset failed.", errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset for {Email}", request.Email);
            return Result.Failure("An error occurred during password reset.");
        }
    }

    public async Task<Result> ChangePasswordAsync(ChangePasswordRequest request)
    {
        try
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Result.Failure("User not found.");
            }

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                _logger.LogInformation("Password changed for user {Email}", user.Email);
                return Result.Success("Password changed successfully.");
            }

            var errors = result.Errors.Select(e => e.Description).ToList();
            return Result.Failure("Password change failed.", errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password change");
            return Result.Failure("An error occurred during password change.");
        }
    }

    private async Task<User?> GetCurrentUserAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        return await _userManager.FindByIdAsync(userId);
    }

    private string GeneratePasswordResetLink(string userId, string token)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        var baseUrl = $"{request?.Scheme}://{request?.Host}";
        return $"{baseUrl}/Account/ResetPassword?userId={userId}&token={Uri.EscapeDataString(token)}";
    }
}
