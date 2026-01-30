using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Features.Account.ViewModels.Authentication;

namespace CommunityCar.Application.Common.Interfaces.Services.Account.Authentication.OAuth;

/// <summary>
/// Service responsible for Facebook OAuth operations
/// </summary>
public interface IFacebookOAuthService
{
    /// <summary>
    /// Signs in user with Facebook access token
    /// </summary>
    Task<Result> FacebookSignInAsync(FacebookSignInRequest request);

    /// <summary>
    /// Links Facebook account to existing user
    /// </summary>
    Task<Result> LinkFacebookAccountAsync(LinkExternalAccountRequest request);

    /// <summary>
    /// Verifies Facebook access token and returns user info
    /// </summary>
    Task<CommunityCar.Application.Features.Account.ViewModels.Authentication.FacebookUserInfo?> VerifyFacebookTokenAsync(string accessToken);
}