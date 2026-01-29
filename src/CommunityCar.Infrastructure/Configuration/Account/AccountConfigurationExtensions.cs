using CommunityCar.Infrastructure.Configuration.Account.Core;
using CommunityCar.Infrastructure.Configuration.Account.Authentication;
using CommunityCar.Infrastructure.Configuration.Account.Security;
using CommunityCar.Infrastructure.Configuration.Account.Gamification;
using CommunityCar.Infrastructure.Configuration.Account.Social;
using CommunityCar.Infrastructure.Configuration.Account.Media;
using CommunityCar.Infrastructure.Configuration.Account.Management;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommunityCar.Infrastructure.Configuration.Account;

/// <summary>
/// Extension methods for configuring Account-related settings
/// </summary>
public static class AccountConfigurationExtensions
{
    /// <summary>
    /// Adds all Account-related configuration settings to the service collection
    /// </summary>
    public static IServiceCollection AddAccountConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        // Core Account Settings
        services.Configure<AccountSettings>(configuration.GetSection(AccountSettings.SectionName));
        
        // Authentication Settings
        services.Configure<AuthenticationSettings>(configuration.GetSection(AuthenticationSettings.SectionName));
        
        // Security Settings
        services.Configure<SecuritySettings>(configuration.GetSection(SecuritySettings.SectionName));
        
        // Gamification Settings
        services.Configure<GamificationSettings>(configuration.GetSection(GamificationSettings.SectionName));
        
        // Social Settings
        services.Configure<SocialSettings>(configuration.GetSection(SocialSettings.SectionName));
        
        // Media Settings
        services.Configure<MediaSettings>(configuration.GetSection(MediaSettings.SectionName));
        
        // Management Settings
        services.Configure<ManagementSettings>(configuration.GetSection(ManagementSettings.SectionName));

        return services;
    }

    /// <summary>
    /// Validates Account configuration settings
    /// </summary>
    public static void ValidateAccountConfiguration(IConfiguration configuration)
    {
        var accountSettings = configuration.GetSection(AccountSettings.SectionName).Get<AccountSettings>();
        var authSettings = configuration.GetSection(AuthenticationSettings.SectionName).Get<AuthenticationSettings>();
        var securitySettings = configuration.GetSection(SecuritySettings.SectionName).Get<SecuritySettings>();

        // Validate core account settings
        if (accountSettings != null)
        {
            if (accountSettings.MaxLoginAttempts <= 0)
                throw new InvalidOperationException("MaxLoginAttempts must be greater than 0");
                
            if (accountSettings.LoginLockoutDuration <= TimeSpan.Zero)
                throw new InvalidOperationException("LoginLockoutDuration must be greater than 0");
        }

        // Validate authentication settings
        if (authSettings?.Jwt?.EnableJwtAuthentication == true)
        {
            if (string.IsNullOrEmpty(authSettings.Jwt.SecretKey))
                throw new InvalidOperationException("JWT SecretKey is required when JWT authentication is enabled");
                
            if (authSettings.Jwt.AccessTokenExpiry <= TimeSpan.Zero)
                throw new InvalidOperationException("JWT AccessTokenExpiry must be greater than 0");
        }

        // Validate OAuth settings
        if (authSettings?.OAuth?.EnableOAuth == true)
        {
            if (authSettings.OAuth.Google.Enabled && string.IsNullOrEmpty(authSettings.OAuth.Google.ClientId))
                throw new InvalidOperationException("Google OAuth ClientId is required when Google OAuth is enabled");
                
            if (authSettings.OAuth.Facebook.Enabled && string.IsNullOrEmpty(authSettings.OAuth.Facebook.AppId))
                throw new InvalidOperationException("Facebook OAuth AppId is required when Facebook OAuth is enabled");
        }

        // Validate security settings
        if (securitySettings != null)
        {
            if (securitySettings.PasswordPolicy.MinLength < 4)
                throw new InvalidOperationException("Password minimum length must be at least 4 characters");
                
            if (securitySettings.PasswordPolicy.MaxLength < securitySettings.PasswordPolicy.MinLength)
                throw new InvalidOperationException("Password maximum length must be greater than minimum length");
                
            if (securitySettings.LockoutPolicy.MaxFailedAttempts <= 0)
                throw new InvalidOperationException("MaxFailedAttempts must be greater than 0");
        }
    }

    /// <summary>
    /// Gets Account settings with fallback to defaults
    /// </summary>
    public static AccountSettings GetAccountSettings(this IConfiguration configuration)
    {
        return configuration.GetSection(AccountSettings.SectionName).Get<AccountSettings>() ?? new AccountSettings();
    }

    /// <summary>
    /// Gets Authentication settings with fallback to defaults
    /// </summary>
    public static AuthenticationSettings GetAuthenticationSettings(this IConfiguration configuration)
    {
        return configuration.GetSection(AuthenticationSettings.SectionName).Get<AuthenticationSettings>() ?? new AuthenticationSettings();
    }

    /// <summary>
    /// Gets Security settings with fallback to defaults
    /// </summary>
    public static SecuritySettings GetSecuritySettings(this IConfiguration configuration)
    {
        return configuration.GetSection(SecuritySettings.SectionName).Get<SecuritySettings>() ?? new SecuritySettings();
    }

    /// <summary>
    /// Gets Gamification settings with fallback to defaults
    /// </summary>
    public static GamificationSettings GetGamificationSettings(this IConfiguration configuration)
    {
        return configuration.GetSection(GamificationSettings.SectionName).Get<GamificationSettings>() ?? new GamificationSettings();
    }

    /// <summary>
    /// Gets Social settings with fallback to defaults
    /// </summary>
    public static SocialSettings GetSocialSettings(this IConfiguration configuration)
    {
        return configuration.GetSection(SocialSettings.SectionName).Get<SocialSettings>() ?? new SocialSettings();
    }

    /// <summary>
    /// Gets Media settings with fallback to defaults
    /// </summary>
    public static MediaSettings GetMediaSettings(this IConfiguration configuration)
    {
        return configuration.GetSection(MediaSettings.SectionName).Get<MediaSettings>() ?? new MediaSettings();
    }

    /// <summary>
    /// Gets Management settings with fallback to defaults
    /// </summary>
    public static ManagementSettings GetManagementSettings(this IConfiguration configuration)
    {
        return configuration.GetSection(ManagementSettings.SectionName).Get<ManagementSettings>() ?? new ManagementSettings();
    }
}