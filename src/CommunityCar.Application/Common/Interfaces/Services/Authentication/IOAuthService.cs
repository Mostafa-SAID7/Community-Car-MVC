using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;

namespace CommunityCar.Application.Common.Interfaces.Services.Authentication;

public interface IOAuthService
{
    Task<AuthResult> GoogleSignInAsync(GoogleSignInRequest request);
    Task<AuthResult> FacebookSignInAsync(FacebookSignInRequest request);
    Task<AuthResult> LinkGoogleAccountAsync(LinkExternalAccountRequest request);
    Task<AuthResult> LinkFacebookAccountAsync(LinkExternalAccountRequest request);
    Task<AuthResult> UnlinkExternalAccountAsync(UnlinkExternalAccountRequest request);
    Task<IEnumerable<ExternalLoginInfo>> GetExternalLoginsAsync(string userId);
}

public class ExternalLoginInfo
{
    public string LoginProvider { get; set; } = string.Empty;
    public string ProviderKey { get; set; } = string.Empty;
    public string ProviderDisplayName { get; set; } = string.Empty;
    public DateTime LinkedAt { get; set; }
}

public class GoogleUserInfo
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public string Picture { get; set; } = string.Empty;
    public bool EmailVerified { get; set; }
}

public class FacebookUserInfo
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public FacebookPicture? Picture { get; set; }
}

public class FacebookPicture
{
    public FacebookPictureData? Data { get; set; }
}

public class FacebookPictureData
{
    public string Url { get; set; } = string.Empty;
}