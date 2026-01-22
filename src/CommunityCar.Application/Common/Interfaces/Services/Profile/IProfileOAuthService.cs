namespace CommunityCar.Application.Common.Interfaces.Services.Profile;

public interface IProfileOAuthService
{
    // OAuth connections
    Task<bool> LinkGoogleAccountAsync(Guid userId, string googleId, string? profilePictureUrl = null);
    Task<bool> LinkFacebookAccountAsync(Guid userId, string facebookId, string? profilePictureUrl = null);
    Task<bool> UnlinkGoogleAccountAsync(Guid userId);
    Task<bool> UnlinkFacebookAccountAsync(Guid userId);
}