using CommunityCar.Application.Common.Interfaces.Services.Account.Authentication.OAuth;
using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Features.Account.ViewModels.Authentication;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Services.Account.Authentication.OAuth;

public class GoogleOAuthService : IGoogleOAuthService
{
    private readonly ILogger<GoogleOAuthService> _logger;

    public GoogleOAuthService(ILogger<GoogleOAuthService> logger)
    {
        _logger = logger;
    }

    public Task<Result> GoogleSignInAsync(GoogleSignInRequest request)
    {
        return Task.FromResult(Result.Failure("Google Sign-In not implemented yet."));
    }

    public Task<Result> LinkGoogleAccountAsync(LinkExternalAccountRequest request)
    {
         return Task.FromResult(Result.Failure("Google account linking not implemented yet."));
    }

    public Task<CommunityCar.Application.Features.Account.ViewModels.Authentication.GoogleUserInfo?> VerifyGoogleTokenAsync(string idToken)
    {
        _logger.LogWarning("Google Token Verification stub called.");
        return Task.FromResult<CommunityCar.Application.Features.Account.ViewModels.Authentication.GoogleUserInfo?>(null);
    }
}
