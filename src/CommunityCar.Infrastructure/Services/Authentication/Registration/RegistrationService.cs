using CommunityCar.Application.Common.Interfaces.Services.Communication;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;
using CommunityCar.Domain.Entities.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Services.Authentication.Registration;

/// <summary>
/// Service for user registration operations
/// </summary>
public class RegistrationService : IRegistrationService
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<RegistrationService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RegistrationService(
        UserManager<User> userManager,
        IEmailService emailService,
        ILogger<RegistrationService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _emailService = emailService;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> RegisterAsync(RegisterRequest request)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return Result.Failure("User with this email already exists.");
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
                return Result.Failure("Registration failed.", errors);
            }

            // Generate email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = GenerateEmailConfirmationLink(user.Id.ToString(), token);

            // Send confirmation email
            await _emailService.SendEmailConfirmationAsync(user.Email!, confirmationLink);

            _logger.LogInformation("User {Email} registered successfully", request.Email);

            return Result.Success("Registration successful. Please check your email to confirm your account.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration for {Email}", request.Email);
            return Result.Failure("An error occurred during registration.");
        }
    }

    public async Task<Result> ConfirmEmailAsync(string userId, string token)
    {
        try
        {
            if (!Guid.TryParse(userId, out var userGuid))
            {
                return Result.Failure("Invalid user ID.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure("User not found.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                // Send welcome email
                await _emailService.SendWelcomeEmailAsync(user.Email!, user.FullName);
                
                _logger.LogInformation("Email confirmed for user {Email}", user.Email);
                return Result.Success("Email confirmed successfully. You can now log in.");
            }

            var errors = result.Errors.Select(e => e.Description).ToList();
            return Result.Failure("Email confirmation failed.", errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during email confirmation for user {UserId}", userId);
            return Result.Failure("An error occurred during email confirmation.");
        }
    }

    public async Task<Result> ResendEmailConfirmationAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Result.Success("If an account with that email exists, a confirmation email has been sent.");
            }

            if (user.EmailConfirmed)
            {
                return Result.Failure("Email is already confirmed.");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = GenerateEmailConfirmationLink(user.Id.ToString(), token);

            await _emailService.SendEmailConfirmationAsync(user.Email!, confirmationLink);

            return Result.Success("Confirmation email sent.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resending email confirmation for {Email}", email);
            return Result.Failure("An error occurred while sending confirmation email.");
        }
    }

    private string GenerateEmailConfirmationLink(string userId, string token)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        var baseUrl = $"{request?.Scheme}://{request?.Host}";
        return $"{baseUrl}/Account/ConfirmEmail?userId={userId}&token={Uri.EscapeDataString(token)}";
    }
}
