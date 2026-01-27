using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;
using CommunityCar.Domain.Entities.Auth;
using CommunityCar.Infrastructure.Models.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CommunityCar.Infrastructure.Services.Authentication.OAuth;

/// <summary>
/// Service responsible for Facebook OAuth operations
/// </summary>
public class FacebookOAuthService : IFacebookOAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<FacebookOAuthService> _logger;
    private readonly HttpClient _httpClient;

    public FacebookOAuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ILogger<FacebookOAuthService> logger,
        HttpClient httpClient)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<Result> FacebookSignInAsync(FacebookSignInRequest request)
    {
        try
        {
            // Verify Facebook access token
            var facebookUserInfo = await VerifyFacebookTokenAsync(request.AccessToken);
            if (facebookUserInfo == null)
            {
                return Result.Failure("Invalid Facebook token.");
            }

            // Check if user exists
            var existingUser = await _userManager.FindByEmailAsync(facebookUserInfo.Email);
            if (existingUser != null)
            {
                // Link Facebook account if not already linked
                if (string.IsNullOrEmpty(existingUser.FacebookId))
                {
                    existingUser.FacebookId = facebookUserInfo.Id;
                    existingUser.ProfilePictureUrl = facebookUserInfo.Picture?.Data?.Url;
                    existingUser.LastLoginProvider = "Facebook";
                    existingUser.LastLoginAt = DateTime.UtcNow;
                    await _userManager.UpdateAsync(existingUser);
                }

                // Sign in user
                await _signInManager.SignInAsync(existingUser, isPersistent: false);
                
                _logger.LogInformation("User {Email} signed in with Facebook", facebookUserInfo.Email);
                return Result.Success("Successfully signed in with Facebook.");
            }
            else
            {
                // Create new user
                var newUser = new User(facebookUserInfo.Email, facebookUserInfo.Email)
                {
                    FullName = facebookUserInfo.Name,
                    FacebookId = facebookUserInfo.Id,
                    ProfilePictureUrl = facebookUserInfo.Picture?.Data?.Url,
                    EmailConfirmed = true, // Facebook emails are considered verified
                    LastLoginProvider = "Facebook",
                    LastLoginAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(newUser);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(newUser, isPersistent: false);
                    
                    _logger.LogInformation("New user {Email} created and signed in with Facebook", facebookUserInfo.Email);
                    return Result.Success("Successfully signed in with Facebook.");
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
            _logger.LogError(ex, "Error during Facebook sign-in");
            return Result.Failure("An error occurred during Facebook sign-in.");
        }
    }

    public async Task<Result> LinkFacebookAccountAsync(LinkExternalAccountRequest request)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return Result.Failure("User not found.");
            }

            // Verify Facebook token
            var facebookUserInfo = await VerifyFacebookTokenAsync(request.ExternalToken);
            if (facebookUserInfo == null)
            {
                return Result.Failure("Invalid Facebook token.");
            }

            // Check if Facebook account is already linked to another user
            var existingUser = await _userManager.Users
                .FirstOrDefaultAsync(u => u.FacebookId == facebookUserInfo.Id && u.Id != user.Id);
            if (existingUser != null)
            {
                return Result.Failure("This Facebook account is already linked to another user.");
            }

            // Link Facebook account
            user.FacebookId = facebookUserInfo.Id;
            if (string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                user.ProfilePictureUrl = facebookUserInfo.Picture?.Data?.Url;
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("Facebook account linked for user {UserId}", request.UserId);
                return Result.Success("Facebook account linked successfully.");
            }
            else
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return Result.Failure("Failed to link Facebook account.", errors);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error linking Facebook account for user {UserId}", request.UserId);
            return Result.Failure("An error occurred while linking Facebook account.");
        }
    }

    public async Task<FacebookUserInfo?> VerifyFacebookTokenAsync(string accessToken)
    {
        try
        {
            // Verify Facebook access token and get user info
            var response = await _httpClient.GetAsync($"https://graph.facebook.com/me?fields=id,name,email,first_name,last_name,picture&access_token={accessToken}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var userInfo = JsonSerializer.Deserialize<FacebookUserInfo>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });

            return userInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying Facebook token");
            return null;
        }
    }
}
