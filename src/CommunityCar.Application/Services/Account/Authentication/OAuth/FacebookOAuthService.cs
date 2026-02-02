using CommunityCar.Application.Common.Interfaces.Services.Account.Authentication.OAuth;
using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Features.Account.ViewModels.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Application.Configuration.Authentication;
using Microsoft.Extensions.Options;
using CommunityCar.Application.Common.Extensions;
using System.Net.Http.Json;
using System.Text.Json;

namespace CommunityCar.Application.Services.Account.Authentication.OAuth;

public class FacebookOAuthService : IFacebookOAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly OAuthSettings _settings;
    private readonly HttpClient _httpClient;
    private readonly ILogger<FacebookOAuthService> _logger;

    public FacebookOAuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IOptions<AuthenticationSettings> options,
        HttpClient httpClient,
        ILogger<FacebookOAuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _settings = options.Value.OAuth;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Result> FacebookSignInAsync(FacebookSignInRequest request)
    {
        try
        {
            var userInfo = await VerifyFacebookTokenAsync(request.AccessToken);
            if (userInfo == null)
                return Result.Failure("Invalid Facebook token.");

            var info = new UserLoginInfo("Facebook", userInfo.ExternalId, "Facebook");
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(userInfo.Email);
                if (user == null)
                {
                    user = new User(userInfo.Email, userInfo.Email, userInfo.Name)
                    {
                        EmailConfirmed = true // Facebook emails are usually verified
                    };
                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                        return createResult.ToApplicationResult();
                }

                var addLoginResult = await _userManager.AddLoginAsync(user, info);
                if (!addLoginResult.Succeeded)
                    return addLoginResult.ToApplicationResult();
            }

            if (!user.IsActive)
                return Result.Failure("Account deactivated.");

            await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);
            user.UpdateLastLogin();
            await _userManager.UpdateAsync(user);

            return Result.Success("Successfully signed in with Facebook.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Facebook sign-in");
            return Result.Failure("An error occurred during Facebook sign-in.");
        }
    }

    public Task<Result> LinkFacebookAccountAsync(LinkExternalAccountRequest request)
    {
        return Task.FromResult(Result.Failure("Facebook account linking not implemented yet."));
    }

    public async Task<CommunityCar.Application.Features.Account.ViewModels.Authentication.FacebookUserInfo?> VerifyFacebookTokenAsync(string accessToken)
    {
        try
        {
            // Verify token and get user info from Graph API
            var response = await _httpClient.GetAsync($"https://graph.facebook.com/me?fields=id,name,email,first_name,last_name,picture&access_token={accessToken}");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Facebook token verification failed with status {StatusCode}", response.StatusCode);
                return null;
            }

            var fbResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

            return new CommunityCar.Application.Features.Account.ViewModels.Authentication.FacebookUserInfo
            {
                Provider = "Facebook",
                ExternalId = fbResponse.GetProperty("id").GetString() ?? "",
                Email = fbResponse.TryGetProperty("email", out var emailProp) ? emailProp.GetString() ?? "" : "",
                Name = fbResponse.GetProperty("name").GetString() ?? ""
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Facebook token verification failed");
            return null;
        }
    }
}