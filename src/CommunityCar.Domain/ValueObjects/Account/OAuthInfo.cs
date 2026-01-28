using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.ValueObjects.Account;

public class OAuthInfo : ValueObject
{
    public string? GoogleId { get; private set; }
    public string? FacebookId { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public string? LastLoginProvider { get; private set; }

    // Parameterless constructor for EF Core
    private OAuthInfo()
    {
        GoogleId = null;
        FacebookId = null;
        LastLoginAt = null;
        LastLoginProvider = null;
    }

    public OAuthInfo(
        string? googleId = null,
        string? facebookId = null,
        DateTime? lastLoginAt = null,
        string? lastLoginProvider = null)
    {
        GoogleId = googleId;
        FacebookId = facebookId;
        LastLoginAt = lastLoginAt;
        LastLoginProvider = lastLoginProvider;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return GoogleId ?? string.Empty;
        yield return FacebookId ?? string.Empty;
        yield return LastLoginAt ?? DateTime.MinValue;
        yield return LastLoginProvider ?? string.Empty;
    }

    public static OAuthInfo Empty => new();

    public bool HasGoogleAccount => !string.IsNullOrEmpty(GoogleId);
    public bool HasFacebookAccount => !string.IsNullOrEmpty(FacebookId);
    public bool HasAnyOAuthAccount => HasGoogleAccount || HasFacebookAccount;

    public OAuthInfo LinkGoogle(string googleId)
    {
        return new OAuthInfo(googleId, FacebookId, DateTime.UtcNow, "Google");
    }

    public OAuthInfo LinkFacebook(string facebookId)
    {
        return new OAuthInfo(GoogleId, facebookId, DateTime.UtcNow, "Facebook");
    }

    public OAuthInfo UnlinkGoogle()
    {
        return new OAuthInfo(null, FacebookId, LastLoginAt, LastLoginProvider);
    }

    public OAuthInfo UnlinkFacebook()
    {
        return new OAuthInfo(GoogleId, null, LastLoginAt, LastLoginProvider);
    }

    public OAuthInfo UpdateLastLogin(string provider)
    {
        return new OAuthInfo(GoogleId, FacebookId, DateTime.UtcNow, provider);
    }
}