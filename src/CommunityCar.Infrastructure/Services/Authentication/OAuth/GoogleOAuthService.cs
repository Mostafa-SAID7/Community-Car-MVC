using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;
using CommunityCar.Domain.Entities.Account;
using CommunityCar.Infrastructure.Models.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CommunityCar.Infrastructure.Services.Authentication.OAuth;

/// <summary>
/// Service responsible for Google OAuth operations
/// </summary>
public class GoogleOAuthService : IGoogleOAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<GoogleOAuthService> _logger;
    private readonly HttpClient _httpClient;

    public GoogleOAuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ILogger<GoogleOAuthService> logger,
        HttpClient httpClient)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<Result> GoogleSignInAsync(GoogleSignInRequest request)
    {
        try
        {
            // Verify Google ID token
            var googleUserInfo = await VerifyGoogleTokenAsync(request.IdToken);
            if (googleUserInfo == null)
            {
                return Result.Failure("Invalid Google token.");
            }

            // Check if user exists
            var existingUser = await _userManager.FindByEmailAsync(googleUserInfo.Email);
            if (existingUser != null)
            {
                // Link Google account if not already linked
                if (!existingUser.OAuthInfo.HasGoogleAccount)
                {
                    existingUser.LinkGoogleAccount(googleUserInfo.Id);
                    var updatedProfile = existingUser.Profile.UpdateProfilePicture(googleUserInfo.Picture);
                    existingUser.UpdateProfile(updatedProfile);
                    existingUser.UpdateLastLogin("Google");
                    await _userManager.UpdateAsync(existingUser);
                }

                // Sign in user
                await _signInManager.SignInAsync(existingUser, isPersistent: false);
                
                _logger.LogInformation("User {Email} signed in with Google", googleUserInfo.Email);
                return Result.Success("Successfully signed in with Google.");
            }
            else
            {
                // Create new user
                var newUser = new User(googleUserInfo.Email, googleUserInfo.Email, googleUserInfo.Name)
                {
                    EmailConfirmed = googleUserInfo.EmailVerified
                };
                
                newUser.LinkGoogleAccount(googleUserInfo.Id);
                var profile = newUser.Profile.UpdateProfilePicture(googleUserInfo.Picture);
                newUser.UpdateProfile(profile);
                newUser.UpdateLastLogin("Google");

                var result = await _userManager.CreateAsync(newUser);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(newUser, isPersistent: false);
                    
                    _logger.LogInformation("New user {Email} created and signed in with Google", googleUserInfo.Email);
                    return Result.Success("Successfully signed in with Google.");
                }
                else
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return Result.Failure("Failed to create user account.", errors);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Google sign-in");
            return Result.Failure("An error occurred during Google sign-in.");
        }
    }

    public async Task<Result> LinkGoogleAccountAsync(LinkExternalAccountRequest request)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return Result.Failure("User not found.");
            }

            // Verify Google token
            var googleUserInfo = await VerifyGoogleTokenAsync(request.ExternalToken);
            if (googleUserInfo == null)
            {
                return Result.Failure("Invalid Google token.");
            }

            // Check if Google account is already linked to another user
            var existingUser = await _userManager.Users
                .FirstOrDefaultAsync(u => u.OAuthInfo.GoogleId == googleUserInfo.Id && u.Id != user.Id);
            if (existingUser != null)
            {
                return Result.Failure("This Google account is already linked to another user.");
            }

            // Link Google account
            user.LinkGoogleAccount(googleUserInfo.Id);
            if (string.IsNullOrEmpty(user.Profile.ProfilePictureUrl))
            {
                var updatedProfile = user.Profile.UpdateProfilePicture(googleUserInfo.Picture);
                user.UpdateProfile(updatedProfile);
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("Google account linked for user {UserId}", request.UserId);
                return Result.Success("Google account linked successfully.");
            }
            else
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return Result.Failure("Failed to link Google account.", errors);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error linking Google account for user {UserId}", request.UserId);
            return Result.Failure("An error occurred while linking Google account.");
        }
    }

    public async Task<GoogleUserInfo?> VerifyGoogleTokenAsync(string idToken)
    {
        try
        {
            // In a real implementation, verify the JWT token with Google's public keys
            // For now, this is a placeholder that would need proper JWT verification
            
            // This is a simplified version - in production, use Google.Apis.Auth library
            var response = await _httpClient.GetAsync($"https://oauth2.googleapis.com/tokeninfo?id_token={idToken}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var tokenInfo = JsonSerializer.Deserialize<GoogleUserInfo>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });

            return tokenInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying Google token");
            return null;
        }
    }
}
