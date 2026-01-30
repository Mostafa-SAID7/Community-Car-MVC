using CommunityCar.Application.Common.Interfaces.Services.Account.Authentication.OAuth;
using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Features.Account.ViewModels.Authentication;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Services.Account.Authentication.OAuth;

public class FacebookOAuthService : IFacebookOAuthService
{
    private readonly ILogger<FacebookOAuthService> _logger;

    public FacebookOAuthService(ILogger<FacebookOAuthService> logger)
    {
        _logger = logger;
    }

    public Task<Result> FacebookSignInAsync(FacebookSignInRequest request)
    {
        return Task.FromResult(Result.Failure("Facebook Sign-In not implemented yet."));
    }

    public Task<Result> LinkFacebookAccountAsync(LinkExternalAccountRequest request)
    {
        return Task.FromResult(Result.Failure("Facebook account linking not implemented yet."));
    }

    public Task<CommunityCar.Application.Features.Account.ViewModels.Authentication.FacebookUserInfo?> VerifyFacebookTokenAsync(string accessToken)
    {
        _logger.LogWarning("Facebook Token Verification stub called.");
        return Task.FromResult<CommunityCar.Application.Features.Account.ViewModels.Authentication.FacebookUserInfo?>(null);
    }
}
