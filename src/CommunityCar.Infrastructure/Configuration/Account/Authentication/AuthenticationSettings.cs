namespace CommunityCar.Infrastructure.Configuration.Account.Authentication;

/// <summary>
/// Authentication configuration settings
/// </summary>
public class AuthenticationSettings
{
    public const string SectionName = "Authentication";

    public CookieSettings Cookie { get; set; } = new();
    public JwtSettings Jwt { get; set; } = new();
    public OAuthSettings OAuth { get; set; } = new();
    public TwoFactorSettings TwoFactor { get; set; } = new();
    public SessionSettings Session { get; set; } = new();
}

/// <summary>
/// Cookie authentication settings
/// </summary>
public class CookieSettings
{
    public string CookieName { get; set; } = "CommunityCar.Auth";
    public TimeSpan ExpireTimeSpan { get; set; } = TimeSpan.FromDays(30);
    public bool SlidingExpiration { get; set; } = true;
    public string LoginPath { get; set; } = "/Login";
    public string LogoutPath { get; set; } = "/Logout";
    public string AccessDeniedPath { get; set; } = "/AccessDenied";
    public bool HttpOnly { get; set; } = true;
    public bool SecurePolicy { get; set; } = true; // Always secure in production
    public string SameSiteMode { get; set; } = "Strict"; // Strict, Lax, None
}

/// <summary>
/// JWT authentication settings
/// </summary>
public class JwtSettings
{
    public bool EnableJwtAuthentication { get; set; } = false;
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = "CommunityCar";
    public string Audience { get; set; } = "CommunityCar.Users";
    public TimeSpan AccessTokenExpiry { get; set; } = TimeSpan.FromMinutes(15);
    public TimeSpan RefreshTokenExpiry { get; set; } = TimeSpan.FromDays(7);
    public bool ValidateIssuer { get; set; } = true;
    public bool ValidateAudience { get; set; } = true;
    public bool ValidateLifetime { get; set; } = true;
    public bool ValidateIssuerSigningKey { get; set; } = true;
}

/// <summary>
/// OAuth authentication settings
/// </summary>
public class OAuthSettings
{
    public bool EnableOAuth { get; set; } = true;
    public GoogleOAuthSettings Google { get; set; } = new();
    public FacebookOAuthSettings Facebook { get; set; } = new();
    public MicrosoftOAuthSettings Microsoft { get; set; } = new();
    public TwitterOAuthSettings Twitter { get; set; } = new();
}

/// <summary>
/// Google OAuth settings
/// </summary>
public class GoogleOAuthSettings
{
    public bool Enabled { get; set; } = false;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string[] Scopes { get; set; } = { "openid", "profile", "email" };
    public bool SaveTokens { get; set; } = true;
}

/// <summary>
/// Facebook OAuth settings
/// </summary>
public class FacebookOAuthSettings
{
    public bool Enabled { get; set; } = false;
    public string AppId { get; set; } = string.Empty;
    public string AppSecret { get; set; } = string.Empty;
    public string[] Scopes { get; set; } = { "email", "public_profile" };
    public bool SaveTokens { get; set; } = true;
}

/// <summary>
/// Microsoft OAuth settings
/// </summary>
public class MicrosoftOAuthSettings
{
    public bool Enabled { get; set; } = false;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string TenantId { get; set; } = "common";
    public string[] Scopes { get; set; } = { "openid", "profile", "email" };
    public bool SaveTokens { get; set; } = true;
}

/// <summary>
/// Twitter OAuth settings
/// </summary>
public class TwitterOAuthSettings
{
    public bool Enabled { get; set; } = false;
    public string ConsumerKey { get; set; } = string.Empty;
    public string ConsumerSecret { get; set; } = string.Empty;
    public bool SaveTokens { get; set; } = true;
}

/// <summary>
/// Two-factor authentication settings
/// </summary>
public class TwoFactorSettings
{
    public bool EnableTwoFactor { get; set; } = true;
    public bool RequireForAdmins { get; set; } = true;
    public string ApplicationName { get; set; } = "CommunityCar";
    public TimeSpan TokenLifetime { get; set; } = TimeSpan.FromMinutes(5);
    public int TokenLength { get; set; } = 6;
    public int MaxAttempts { get; set; } = 3;
    public TimeSpan LockoutDuration { get; set; } = TimeSpan.FromMinutes(15);
    public bool EnableRecoveryCodes { get; set; } = true;
    public int RecoveryCodesCount { get; set; } = 10;
}

/// <summary>
/// Session management settings
/// </summary>
public class SessionSettings
{
    public bool EnableSessionTracking { get; set; } = true;
    public TimeSpan SessionTimeout { get; set; } = TimeSpan.FromHours(24);
    public TimeSpan IdleTimeout { get; set; } = TimeSpan.FromMinutes(30);
    public int MaxConcurrentSessions { get; set; } = 5;
    public bool TerminateOtherSessionsOnLogin { get; set; } = false;
    public bool TrackLocation { get; set; } = true;
    public bool TrackDevice { get; set; } = true;
    public TimeSpan SessionCleanupInterval { get; set; } = TimeSpan.FromHours(6);
}