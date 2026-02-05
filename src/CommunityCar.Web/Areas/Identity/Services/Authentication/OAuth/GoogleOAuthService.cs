using CommunityCar.Web.Areas.Identity.Interfaces.Services.Authentication.OAuth;
using CommunityCar.Web.Areas.Identity.Interfaces.Services;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Features.Account.ViewModels.Authentication;
using Microsoft.Extensions.Logging;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Application.Configuration.Authentication;
using Microsoft.Extensions.Options;
using CommunityCar.Application.Common.Extensions;

namespace CommunityCar.Web.Areas.Identity.Services.Authentication.OAuth;

public class GoogleOAuthService : IGoogleOAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly OAuthSettings _settings;
    private readonly ILogger<GoogleOAuthService> _logger;

    public GoogleOAuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IOptions<AuthenticationSettings> options,
        ILogger<GoogleOAuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _settings = options.Value.OAuth;
        _logger = logger;
    }

    public async Task<Result> GoogleSignInAsync(GoogleSignInRequest request)
    {
        try
        {
            var payload = await VerifyGoogleTokenAsync(request.IdToken);
            if (payload == null)
                return Result.Failure("Invalid Google token.");

            var info = new UserLoginInfo("Google", payload.ExternalId, "Google");
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    user = new User(payload.Email, payload.Email, payload.Name)
                    {
                        EmailConfirmed = true // Google emails are verified
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

            return Result.Success("Successfully signed in with Google.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Google sign-in");
            return Result.Failure("An error occurred during Google sign-in.");
        }
    }

    public Task<Result> LinkGoogleAccountAsync(LinkExternalAccountRequest request)
    {
         return Task.FromResult(Result.Failure("Google account linking not implemented yet."));
    }

    public async Task<CommunityCar.Application.Features.Account.ViewModels.Authentication.GoogleUserInfo?> VerifyGoogleTokenAsync(string idToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _settings.Google.ClientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            
            return new CommunityCar.Application.Features.Account.ViewModels.Authentication.GoogleUserInfo
            {
                Provider = "Google",
                ExternalId = payload.Subject,
                Email = payload.Email,
                Name = payload.Name
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Google token verification failed");
            return null;
        }
    }
}

