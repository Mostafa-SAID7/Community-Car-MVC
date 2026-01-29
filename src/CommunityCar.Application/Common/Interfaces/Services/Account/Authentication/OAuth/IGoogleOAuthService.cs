using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Features.Account.ViewModels.Authentication;

namespace CommunityCar.Application.Common.Interfaces.Services.Account.Authentication.OAuth;

/// <summary>
/// Service responsible for Google OAuth operations
/// </summary>
public interface IGoogleOAuthService
{
    /// <summary>
    /// Signs in user with Google ID token
    /// </summary>
    Task<Result> GoogleSignInAsync(GoogleSignInRequest request);

    /// <summary>
    /// Links Google account to existing user
    /// </summary>
    Task<Result> LinkGoogleAccountAsync(LinkExternalAccountRequest request);

    /// <summary>
    /// Verifies Google ID token and returns user info
    /// </summary>
    Task<GoogleUserInfo?> VerifyGoogleTokenAsync(string idToken);
}