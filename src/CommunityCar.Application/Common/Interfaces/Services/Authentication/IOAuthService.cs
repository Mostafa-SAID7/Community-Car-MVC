using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;

namespace CommunityCar.Application.Common.Interfaces.Services.Authentication;

public interface IOAuthService
{
    Task<Result> GoogleSignInAsync(GoogleSignInRequest request);
    Task<Result> FacebookSignInAsync(FacebookSignInRequest request);
    Task<Result> LinkGoogleAccountAsync(LinkExternalAccountRequest request);
    Task<Result> LinkFacebookAccountAsync(LinkExternalAccountRequest request);
    Task<Result> UnlinkExternalAccountAsync(UnlinkExternalAccountRequest request);
    Task<IEnumerable<ExternalLoginInfo>> GetExternalLoginsAsync(string userId);
    
    // Account Management additions
    Task<bool> IsAccountLinkedAsync(string userId, string provider);
    Task<string?> GetLinkedAccountIdAsync(string userId, string provider);
    Task<bool> CanLinkAccountAsync(string userId, string provider);
    Task<bool> CanUnlinkAccountAsync(string userId, string provider);
    Task<bool> HasPasswordSetAsync(string userId);
    Task<IEnumerable<string>> GetAvailableProvidersAsync();
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


