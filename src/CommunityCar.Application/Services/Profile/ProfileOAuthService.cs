using CommunityCar.Application.Common.Interfaces.Services.Profile;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using Microsoft.AspNetCore.Identity;
using CommunityCar.Domain.Entities.Auth;

namespace CommunityCar.Application.Services.Profile;

public class ProfileOAuthService : IProfileOAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly ICurrentUserService _currentUserService;

    public ProfileOAuthService(
        UserManager<User> userManager,
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    public async Task<bool> LinkGoogleAccountAsync(Guid userId, string googleId, string? profilePictureUrl = null)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        user.LinkGoogleAccount(googleId, profilePictureUrl);
        user.Audit(_currentUserService.UserId);
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> LinkFacebookAccountAsync(Guid userId, string facebookId, string? profilePictureUrl = null)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        user.LinkFacebookAccount(facebookId, profilePictureUrl);
        user.Audit(_currentUserService.UserId);
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> UnlinkGoogleAccountAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        user.UnlinkGoogleAccount();
        user.Audit(_currentUserService.UserId);
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> UnlinkFacebookAccountAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        user.UnlinkFacebookAccount();
        user.Audit(_currentUserService.UserId);
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }
}