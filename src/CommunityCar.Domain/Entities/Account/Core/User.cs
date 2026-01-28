using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using CommunityCar.Domain.Events;
using CommunityCar.Domain.Base;
using CommunityCar.Domain.ValueObjects.Account;
using CommunityCar.Domain.Enums.Account;
using CommunityCar.Domain.Entities.Account.Authentication;
using CommunityCar.Domain.Entities.Account.Profile;
using CommunityCar.Domain.Entities.Account.Management;
using CommunityCar.Domain.Entities.Account.Analytics;
using CommunityCar.Domain.Entities.Account.Gamification;
using CommunityCar.Domain.Entities.Account.Media;

namespace CommunityCar.Domain.Entities.Account.Core;

// Inherit from IdentityUser<Guid> for ASP.NET Core Identity compatibility
public class User : IdentityUser<Guid>, IBaseEntity, ISoftDeletable
{
    // Re-implementing BaseEntity properties manually since we can't multiple inherit
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Soft Delete Properties
    public bool IsDeleted { get; private set; } = false;
    public DateTime? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }

    // Profile Information (Value Object)
    private UserProfile _profile = UserProfile.Empty;
    public UserProfile Profile 
    { 
        get => _profile; 
        private set => _profile = value ?? UserProfile.Empty; 
    }

    // Privacy Settings (Value Object)
    private PrivacySettings _privacySettings = PrivacySettings.Default;
    public PrivacySettings PrivacySettings 
    { 
        get => _privacySettings; 
        private set => _privacySettings = value ?? PrivacySettings.Default; 
    }

    // Notification Settings (Value Object)
    private NotificationSettings _notificationSettings = NotificationSettings.Default;
    public NotificationSettings NotificationSettings 
    { 
        get => _notificationSettings; 
        private set => _notificationSettings = value ?? NotificationSettings.Default; 
    }

    // Two Factor Settings (Value Object)
    private TwoFactorSettings _twoFactorSettings = TwoFactorSettings.Disabled;
    public TwoFactorSettings TwoFactorSettings 
    { 
        get => _twoFactorSettings; 
        private set => _twoFactorSettings = value ?? TwoFactorSettings.Disabled; 
    }

    // OAuth Information (Value Object)
    private OAuthInfo _oAuthInfo = OAuthInfo.Empty;
    public OAuthInfo OAuthInfo 
    { 
        get => _oAuthInfo; 
        private set => _oAuthInfo = value ?? OAuthInfo.Empty; 
    }

    public bool IsActive { get; set; }

    // Gamification & Roles
    public int TotalPoints { get; set; } = 0;
    public bool HasPendingAdminRequest { get; set; } = false;
    
    // Security
    public DateTime? LastPasswordChangeAt { get; set; }

    // Backward compatibility properties (marked as obsolete)
    [Obsolete("Use Profile.FullName instead")]
    public string FullName 
    { 
        get => Profile.FullName; 
        set => Profile = Profile.UpdateBasicInfo(value, Profile.FirstName, Profile.LastName); 
    }

    [Obsolete("Use Profile.FirstName instead")]
    public string? FirstName 
    { 
        get => Profile.FirstName; 
        set => Profile = Profile.UpdateBasicInfo(Profile.FullName, value, Profile.LastName); 
    }

    [Obsolete("Use Profile.LastName instead")]
    public string? LastName 
    { 
        get => Profile.LastName; 
        set => Profile = Profile.UpdateBasicInfo(Profile.FullName, Profile.FirstName, value); 
    }

    [Obsolete("Use Profile.Bio instead")]
    public string? Bio 
    { 
        get => Profile.Bio; 
        set => Profile = Profile.UpdateBio(value, Profile.BioAr); 
    }

    [Obsolete("Use Profile.City instead")]
    public string? City 
    { 
        get => Profile.City; 
        set => Profile = Profile.UpdateLocation(value, Profile.Country, Profile.CityAr, Profile.CountryAr); 
    }

    [Obsolete("Use Profile.Country instead")]
    public string? Country 
    { 
        get => Profile.Country; 
        set => Profile = Profile.UpdateLocation(Profile.City, value, Profile.CityAr, Profile.CountryAr); 
    }

    [Obsolete("Use Profile.Website instead")]
    public string? Website 
    { 
        get => Profile.Website; 
        set => Profile = new UserProfile(
            Profile.FullName,
            Profile.FirstName,
            Profile.LastName,
            Profile.Bio,
            Profile.City,
            Profile.Country,
            Profile.BioAr,
            Profile.CityAr,
            Profile.CountryAr,
            value,
            Profile.ProfilePictureUrl,
            Profile.CoverImageUrl); 
    }

    [Obsolete("Use Profile.ProfilePictureUrl instead")]
    public string? ProfilePictureUrl 
    { 
        get => Profile.ProfilePictureUrl; 
        set => Profile = Profile.UpdateProfilePicture(value); 
    }

    [Obsolete("Use Profile.CoverImageUrl instead")]
    public string? CoverImageUrl 
    { 
        get => Profile.CoverImageUrl; 
        set => Profile = Profile.UpdateCoverImage(value); 
    }

    // Privacy Settings backward compatibility
    [Obsolete("Use PrivacySettings.IsPublic instead")]
    public bool IsPublic 
    { 
        get => PrivacySettings.IsPublic; 
        set => PrivacySettings = new PrivacySettings(value, PrivacySettings.ShowEmail, PrivacySettings.ShowLocation, PrivacySettings.ShowOnlineStatus, PrivacySettings.AllowMessagesFromStrangers, PrivacySettings.AllowTagging, PrivacySettings.ShowActivityStatus); 
    }

    [Obsolete("Use PrivacySettings.ShowEmail instead")]
    public bool ShowEmail 
    { 
        get => PrivacySettings.ShowEmail; 
        set => PrivacySettings = new PrivacySettings(PrivacySettings.IsPublic, value, PrivacySettings.ShowLocation, PrivacySettings.ShowOnlineStatus, PrivacySettings.AllowMessagesFromStrangers, PrivacySettings.AllowTagging, PrivacySettings.ShowActivityStatus); 
    }

    [Obsolete("Use PrivacySettings.ShowLocation instead")]
    public bool ShowLocation 
    { 
        get => PrivacySettings.ShowLocation; 
        set => PrivacySettings = new PrivacySettings(PrivacySettings.IsPublic, PrivacySettings.ShowEmail, value, PrivacySettings.ShowOnlineStatus, PrivacySettings.AllowMessagesFromStrangers, PrivacySettings.AllowTagging, PrivacySettings.ShowActivityStatus); 
    }

    [Obsolete("Use PrivacySettings.ShowOnlineStatus instead")]
    public bool ShowOnlineStatus 
    { 
        get => PrivacySettings.ShowOnlineStatus; 
        set => PrivacySettings = new PrivacySettings(PrivacySettings.IsPublic, PrivacySettings.ShowEmail, PrivacySettings.ShowLocation, value, PrivacySettings.AllowMessagesFromStrangers, PrivacySettings.AllowTagging, PrivacySettings.ShowActivityStatus); 
    }

    [Obsolete("Use PrivacySettings.AllowMessagesFromStrangers instead")]
    public bool AllowMessagesFromStrangers 
    { 
        get => PrivacySettings.AllowMessagesFromStrangers; 
        set => PrivacySettings = new PrivacySettings(PrivacySettings.IsPublic, PrivacySettings.ShowEmail, PrivacySettings.ShowLocation, PrivacySettings.ShowOnlineStatus, value, PrivacySettings.AllowTagging, PrivacySettings.ShowActivityStatus); 
    }

    [Obsolete("Use PrivacySettings.AllowTagging instead")]
    public bool AllowTagging 
    { 
        get => PrivacySettings.AllowTagging; 
        set => PrivacySettings = new PrivacySettings(PrivacySettings.IsPublic, PrivacySettings.ShowEmail, PrivacySettings.ShowLocation, PrivacySettings.ShowOnlineStatus, PrivacySettings.AllowMessagesFromStrangers, value, PrivacySettings.ShowActivityStatus); 
    }

    [Obsolete("Use PrivacySettings.ShowActivityStatus instead")]
    public bool ShowActivityStatus 
    { 
        get => PrivacySettings.ShowActivityStatus; 
        set => PrivacySettings = new PrivacySettings(PrivacySettings.IsPublic, PrivacySettings.ShowEmail, PrivacySettings.ShowLocation, PrivacySettings.ShowOnlineStatus, PrivacySettings.AllowMessagesFromStrangers, PrivacySettings.AllowTagging, value); 
    }

    // Notification Settings backward compatibility
    [Obsolete("Use NotificationSettings.EmailNotificationsEnabled instead")]
    public bool EmailNotificationsEnabled 
    { 
        get => NotificationSettings.EmailNotificationsEnabled; 
        set => NotificationSettings = new NotificationSettings(value, NotificationSettings.PushNotificationsEnabled, NotificationSettings.SmsNotificationsEnabled, NotificationSettings.MarketingEmailsEnabled, NotificationSettings.CommentNotificationsEnabled, NotificationSettings.LikeNotificationsEnabled, NotificationSettings.FollowNotificationsEnabled, NotificationSettings.MessageNotificationsEnabled); 
    }

    [Obsolete("Use NotificationSettings.PushNotificationsEnabled instead")]
    public bool PushNotificationsEnabled 
    { 
        get => NotificationSettings.PushNotificationsEnabled; 
        set => NotificationSettings = new NotificationSettings(NotificationSettings.EmailNotificationsEnabled, value, NotificationSettings.SmsNotificationsEnabled, NotificationSettings.MarketingEmailsEnabled, NotificationSettings.CommentNotificationsEnabled, NotificationSettings.LikeNotificationsEnabled, NotificationSettings.FollowNotificationsEnabled, NotificationSettings.MessageNotificationsEnabled); 
    }

    [Obsolete("Use NotificationSettings.SmsNotificationsEnabled instead")]
    public bool SmsNotificationsEnabled 
    { 
        get => NotificationSettings.SmsNotificationsEnabled; 
        set => NotificationSettings = new NotificationSettings(NotificationSettings.EmailNotificationsEnabled, NotificationSettings.PushNotificationsEnabled, value, NotificationSettings.MarketingEmailsEnabled, NotificationSettings.CommentNotificationsEnabled, NotificationSettings.LikeNotificationsEnabled, NotificationSettings.FollowNotificationsEnabled, NotificationSettings.MessageNotificationsEnabled); 
    }

    [Obsolete("Use NotificationSettings.MarketingEmailsEnabled instead")]
    public bool MarketingEmailsEnabled 
    { 
        get => NotificationSettings.MarketingEmailsEnabled; 
        set => NotificationSettings = new NotificationSettings(NotificationSettings.EmailNotificationsEnabled, NotificationSettings.PushNotificationsEnabled, NotificationSettings.SmsNotificationsEnabled, value, NotificationSettings.CommentNotificationsEnabled, NotificationSettings.LikeNotificationsEnabled, NotificationSettings.FollowNotificationsEnabled, NotificationSettings.MessageNotificationsEnabled); 
    }

    [Obsolete("Use NotificationSettings.CommentNotificationsEnabled instead")]
    public bool CommentNotificationsEnabled 
    { 
        get => NotificationSettings.CommentNotificationsEnabled; 
        set => NotificationSettings = new NotificationSettings(NotificationSettings.EmailNotificationsEnabled, NotificationSettings.PushNotificationsEnabled, NotificationSettings.SmsNotificationsEnabled, NotificationSettings.MarketingEmailsEnabled, value, NotificationSettings.LikeNotificationsEnabled, NotificationSettings.FollowNotificationsEnabled, NotificationSettings.MessageNotificationsEnabled); 
    }

    [Obsolete("Use NotificationSettings.LikeNotificationsEnabled instead")]
    public bool LikeNotificationsEnabled 
    { 
        get => NotificationSettings.LikeNotificationsEnabled; 
        set => NotificationSettings = new NotificationSettings(NotificationSettings.EmailNotificationsEnabled, NotificationSettings.PushNotificationsEnabled, NotificationSettings.SmsNotificationsEnabled, NotificationSettings.MarketingEmailsEnabled, NotificationSettings.CommentNotificationsEnabled, value, NotificationSettings.FollowNotificationsEnabled, NotificationSettings.MessageNotificationsEnabled); 
    }

    [Obsolete("Use NotificationSettings.FollowNotificationsEnabled instead")]
    public bool FollowNotificationsEnabled 
    { 
        get => NotificationSettings.FollowNotificationsEnabled; 
        set => NotificationSettings = new NotificationSettings(NotificationSettings.EmailNotificationsEnabled, NotificationSettings.PushNotificationsEnabled, NotificationSettings.SmsNotificationsEnabled, NotificationSettings.MarketingEmailsEnabled, NotificationSettings.CommentNotificationsEnabled, NotificationSettings.LikeNotificationsEnabled, value, NotificationSettings.MessageNotificationsEnabled); 
    }

    [Obsolete("Use NotificationSettings.MessageNotificationsEnabled instead")]
    public bool MessageNotificationsEnabled 
    { 
        get => NotificationSettings.MessageNotificationsEnabled; 
        set => NotificationSettings = new NotificationSettings(NotificationSettings.EmailNotificationsEnabled, NotificationSettings.PushNotificationsEnabled, NotificationSettings.SmsNotificationsEnabled, NotificationSettings.MarketingEmailsEnabled, NotificationSettings.CommentNotificationsEnabled, NotificationSettings.LikeNotificationsEnabled, NotificationSettings.FollowNotificationsEnabled, value); 
    }

    // OAuth Info backward compatibility
    [Obsolete("Use OAuthInfo.GoogleId instead")]
    public string? GoogleId 
    { 
        get => OAuthInfo.GoogleId; 
        set => OAuthInfo = new OAuthInfo(value, OAuthInfo.FacebookId, OAuthInfo.LastLoginAt, OAuthInfo.LastLoginProvider); 
    }

    [Obsolete("Use OAuthInfo.FacebookId instead")]
    public string? FacebookId 
    { 
        get => OAuthInfo.FacebookId; 
        set => OAuthInfo = new OAuthInfo(OAuthInfo.GoogleId, value, OAuthInfo.LastLoginAt, OAuthInfo.LastLoginProvider); 
    }

    [Obsolete("Use OAuthInfo.LastLoginAt instead")]
    public DateTime? LastLoginAt 
    { 
        get => OAuthInfo.LastLoginAt; 
        set => OAuthInfo = new OAuthInfo(OAuthInfo.GoogleId, OAuthInfo.FacebookId, value, OAuthInfo.LastLoginProvider); 
    }

    [Obsolete("Use OAuthInfo.LastLoginProvider instead")]
    public string? LastLoginProvider 
    { 
        get => OAuthInfo.LastLoginProvider; 
        set => OAuthInfo = new OAuthInfo(OAuthInfo.GoogleId, OAuthInfo.FacebookId, OAuthInfo.LastLoginAt, value); 
    }

    // TwoFactor Settings backward compatibility
    [Obsolete("Use TwoFactorSettings.TwoFactorSecretKey instead")]
    public string? TwoFactorSecretKey 
    { 
        get => TwoFactorSettings.TwoFactorSecretKey; 
        set => TwoFactorSettings = new TwoFactorSettings(TwoFactorSettings.TwoFactorEnabled, value, TwoFactorSettings.TwoFactorEnabledAt, TwoFactorSettings.BackupCodes, TwoFactorSettings.FailedTwoFactorAttempts, TwoFactorSettings.TwoFactorLockoutEnd, TwoFactorSettings.SmsEnabled, TwoFactorSettings.EmailTwoFactorEnabled); 
    }

    [Obsolete("Use TwoFactorSettings.TwoFactorEnabledAt instead")]
    public DateTime? TwoFactorEnabledAt 
    { 
        get => TwoFactorSettings.TwoFactorEnabledAt; 
        set => TwoFactorSettings = new TwoFactorSettings(TwoFactorSettings.TwoFactorEnabled, TwoFactorSettings.TwoFactorSecretKey, value, TwoFactorSettings.BackupCodes, TwoFactorSettings.FailedTwoFactorAttempts, TwoFactorSettings.TwoFactorLockoutEnd, TwoFactorSettings.SmsEnabled, TwoFactorSettings.EmailTwoFactorEnabled); 
    }

    [Obsolete("Use TwoFactorSettings.BackupCodes instead")]
    public string? BackupCodes 
    { 
        get => TwoFactorSettings.BackupCodes; 
        set => TwoFactorSettings = new TwoFactorSettings(TwoFactorSettings.TwoFactorEnabled, TwoFactorSettings.TwoFactorSecretKey, TwoFactorSettings.TwoFactorEnabledAt, value, TwoFactorSettings.FailedTwoFactorAttempts, TwoFactorSettings.TwoFactorLockoutEnd, TwoFactorSettings.SmsEnabled, TwoFactorSettings.EmailTwoFactorEnabled); 
    }

    [Obsolete("Use TwoFactorSettings.FailedTwoFactorAttempts instead")]
    public int FailedTwoFactorAttempts 
    { 
        get => TwoFactorSettings.FailedTwoFactorAttempts; 
        set => TwoFactorSettings = new TwoFactorSettings(TwoFactorSettings.TwoFactorEnabled, TwoFactorSettings.TwoFactorSecretKey, TwoFactorSettings.TwoFactorEnabledAt, TwoFactorSettings.BackupCodes, value, TwoFactorSettings.TwoFactorLockoutEnd, TwoFactorSettings.SmsEnabled, TwoFactorSettings.EmailTwoFactorEnabled); 
    }

    [Obsolete("Use TwoFactorSettings.TwoFactorLockoutEnd instead")]
    public DateTime? TwoFactorLockoutEnd 
    { 
        get => TwoFactorSettings.TwoFactorLockoutEnd; 
        set => TwoFactorSettings = new TwoFactorSettings(TwoFactorSettings.TwoFactorEnabled, TwoFactorSettings.TwoFactorSecretKey, TwoFactorSettings.TwoFactorEnabledAt, TwoFactorSettings.BackupCodes, TwoFactorSettings.FailedTwoFactorAttempts, value, TwoFactorSettings.SmsEnabled, TwoFactorSettings.EmailTwoFactorEnabled); 
    }

    [Obsolete("Use TwoFactorSettings.SmsEnabled instead")]
    public bool SmsEnabled 
    { 
        get => TwoFactorSettings.SmsEnabled; 
        set => TwoFactorSettings = new TwoFactorSettings(TwoFactorSettings.TwoFactorEnabled, TwoFactorSettings.TwoFactorSecretKey, TwoFactorSettings.TwoFactorEnabledAt, TwoFactorSettings.BackupCodes, TwoFactorSettings.FailedTwoFactorAttempts, TwoFactorSettings.TwoFactorLockoutEnd, value, TwoFactorSettings.EmailTwoFactorEnabled); 
    }

    [Obsolete("Use TwoFactorSettings.EmailTwoFactorEnabled instead")]
    public bool EmailTwoFactorEnabled 
    { 
        get => TwoFactorSettings.EmailTwoFactorEnabled; 
        set => TwoFactorSettings = new TwoFactorSettings(TwoFactorSettings.TwoFactorEnabled, TwoFactorSettings.TwoFactorSecretKey, TwoFactorSettings.TwoFactorEnabledAt, TwoFactorSettings.BackupCodes, TwoFactorSettings.FailedTwoFactorAttempts, TwoFactorSettings.TwoFactorLockoutEnd, TwoFactorSettings.SmsEnabled, value); 
    }

    // Navigation properties
    public virtual ICollection<UserToken> Tokens { get; private set; } = new List<UserToken>();
    public virtual ICollection<UserSession> Sessions { get; private set; } = new List<UserSession>();
    public virtual ICollection<UserActivity> Activities { get; private set; } = new List<UserActivity>();
    public virtual ICollection<UserAchievement> Achievements { get; private set; } = new List<UserAchievement>();
    public virtual ICollection<UserBadge> Badges { get; private set; } = new List<UserBadge>();
    public virtual ICollection<UserGallery> Gallery { get; private set; } = new List<UserGallery>();
    public virtual ICollection<UserInterest> Interests { get; private set; } = new List<UserInterest>();
    public virtual ICollection<UserManagementAction> ManagementActions { get; private set; } = new List<UserManagementAction>();
    public virtual ICollection<UserContentAnalytics> ContentAnalytics { get; private set; } = new List<UserContentAnalytics>();

    // Consent and Legal
    public bool DataProcessingConsent { get; set; } = false;
    public bool MarketingEmailsConsent { get; set; } = false;
    public string? AcceptedToSVersion { get; set; }
    public DateTime? ToSAcceptedAt { get; set; }
    public string? AcceptedPrivacyPolicyVersion { get; set; }
    public DateTime? PrivacyPolicyAcceptedAt { get; set; }

    // Re-implementing AggregateRoot properties
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public User(string email, string userName, string fullName = "")
    {
        Email = email;
        UserName = userName;
        IsActive = true;
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        Profile = new UserProfile(fullName);
        PrivacySettings = PrivacySettings.Default;
        NotificationSettings = NotificationSettings.Default;
        TwoFactorSettings = TwoFactorSettings.Disabled;
        OAuthInfo = OAuthInfo.Empty;
    }

    // EF Core constructor
    public User() 
    {
        IsActive = true;
        Profile = UserProfile.Empty;
        PrivacySettings = PrivacySettings.Default;
        NotificationSettings = NotificationSettings.Default;
        TwoFactorSettings = TwoFactorSettings.Disabled;
        OAuthInfo = OAuthInfo.Empty;
    }

    public void Deactivate()
    {
        IsActive = false;
        Audit(UpdatedBy);
    }

    public void Delete()
    {
        SoftDelete(UpdatedBy ?? "System");
    }

    public virtual void SoftDelete(string? deletedBy)
    {
        if (IsDeleted) return;
        
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        IsActive = false; // Also deactivate the user
        UpdatedBy = deletedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public virtual void Restore(string? restoredBy)
    {
        if (!IsDeleted) return;
        
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        IsActive = true; // Reactivate the user
        UpdatedBy = restoredBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(UserProfile profile)
    {
        Profile = profile;
        Audit(UpdatedBy);
    }

    public void UpdatePrivacySettings(PrivacySettings privacySettings)
    {
        PrivacySettings = privacySettings;
        Audit(UpdatedBy);
    }

    public void UpdateTwoFactorSettings(TwoFactorSettings twoFactorSettings)
    {
        TwoFactorSettings = twoFactorSettings;
        Audit(UpdatedBy);
    }

    public void UpdateNotificationSettings(NotificationSettings notificationSettings)
    {
        NotificationSettings = notificationSettings;
        Audit(UpdatedBy);
    }

    public void UpdateLastLogin(string? provider = null)
    {
        OAuthInfo = OAuthInfo.UpdateLastLogin(provider ?? "Local");
        Audit(UpdatedBy);
    }

    public void LinkGoogleAccount(string googleId)
    {
        OAuthInfo = OAuthInfo.LinkGoogle(googleId);
        Audit(UpdatedBy);
    }

    public void LinkFacebookAccount(string facebookId)
    {
        OAuthInfo = OAuthInfo.LinkFacebook(facebookId);
        Audit(UpdatedBy);
    }

    public void UnlinkGoogleAccount()
    {
        OAuthInfo = OAuthInfo.UnlinkGoogle();
        Audit(UpdatedBy);
    }

    public void UnlinkFacebookAccount()
    {
        OAuthInfo = OAuthInfo.UnlinkFacebook();
        Audit(UpdatedBy);
    }

    public void EnableTwoFactor(string secretKey)
    {
        TwoFactorSettings = TwoFactorSettings.Enable(secretKey);
        TwoFactorEnabled = true; // Identity property
        Audit(UpdatedBy);
    }

    public void DisableTwoFactor()
    {
        TwoFactorSettings = TwoFactorSettings.Disable();
        TwoFactorEnabled = false; // Identity property
        Audit(UpdatedBy);
    }

    public void IncrementTwoFactorFailures()
    {
        TwoFactorSettings = TwoFactorSettings.IncrementFailures();
        Audit(UpdatedBy);
    }

    public void ResetTwoFactorFailures()
    {
        TwoFactorSettings = TwoFactorSettings.ResetFailures();
        Audit(UpdatedBy);
    }

    public bool IsTwoFactorLockedOut()
    {
        return TwoFactorSettings.IsLockedOut;
    }

    public void SetSmsToken(string token, TimeSpan expiry)
    {
        var userToken = new UserToken(Id, TokenType.SmsTwoFactor, token, expiry, "TwoFactorAuthentication");
        Tokens.Add(userToken);
        Audit(UpdatedBy);
    }

    public void SetEmailTwoFactorToken(string token, TimeSpan expiry)
    {
        var userToken = new UserToken(Id, TokenType.EmailTwoFactor, token, expiry, "TwoFactorAuthentication");
        Tokens.Add(userToken);
        Audit(UpdatedBy);
    }

    public bool IsValidSmsToken(string token)
    {
        var smsToken = Tokens
            .Where(t => t.TokenType == TokenType.SmsTwoFactor && t.IsValid())
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefault();
        
        return smsToken?.ValidateToken(token) ?? false;
    }

    public bool IsValidEmailTwoFactorToken(string token)
    {
        var emailToken = Tokens
            .Where(t => t.TokenType == TokenType.EmailTwoFactor && t.IsValid())
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefault();
        
        return emailToken?.ValidateToken(token) ?? false;
    }

    public void ClearSmsToken()
    {
        var smsTokens = Tokens.Where(t => t.TokenType == TokenType.SmsTwoFactor && !t.IsUsed);
        foreach (var token in smsTokens)
        {
            token.MarkAsUsed();
        }
        Audit(UpdatedBy);
    }

    public void ClearEmailTwoFactorToken()
    {
        var emailTokens = Tokens.Where(t => t.TokenType == TokenType.EmailTwoFactor && !t.IsUsed);
        foreach (var token in emailTokens)
        {
            token.MarkAsUsed();
        }
        Audit(UpdatedBy);
    }

    public void AcceptTermsOfService(string version)
    {
        AcceptedToSVersion = version;
        ToSAcceptedAt = DateTime.UtcNow;
        Audit(UpdatedBy);
    }

    public void AcceptPrivacyPolicy(string version)
    {
        AcceptedPrivacyPolicyVersion = version;
        PrivacyPolicyAcceptedAt = DateTime.UtcNow;
        Audit(UpdatedBy);
    }

    public void UpdatePrivacySettings(Dictionary<string, bool> settings)
    {
        var newPrivacySettings = new PrivacySettings(
            settings.GetValueOrDefault("IsPublic", PrivacySettings.IsPublic),
            settings.GetValueOrDefault("ShowEmail", PrivacySettings.ShowEmail),
            settings.GetValueOrDefault("ShowLocation", PrivacySettings.ShowLocation),
            settings.GetValueOrDefault("ShowOnlineStatus", PrivacySettings.ShowOnlineStatus),
            settings.GetValueOrDefault("AllowMessagesFromStrangers", PrivacySettings.AllowMessagesFromStrangers),
            settings.GetValueOrDefault("AllowTagging", PrivacySettings.AllowTagging),
            settings.GetValueOrDefault("ShowActivityStatus", PrivacySettings.ShowActivityStatus));
        
        PrivacySettings = newPrivacySettings;
        Audit(UpdatedBy);
    }

    public Dictionary<string, bool> GetPrivacySettings()
    {
        return new Dictionary<string, bool>
        {
            ["IsPublic"] = PrivacySettings.IsPublic,
            ["ShowEmail"] = PrivacySettings.ShowEmail,
            ["ShowLocation"] = PrivacySettings.ShowLocation,
            ["ShowOnlineStatus"] = PrivacySettings.ShowOnlineStatus,
            ["AllowMessagesFromStrangers"] = PrivacySettings.AllowMessagesFromStrangers,
            ["AllowTagging"] = PrivacySettings.AllowTagging,
            ["ShowActivityStatus"] = PrivacySettings.ShowActivityStatus
        };
    }

    public void Audit(string? user)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = user;
    }

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    // Session Management
    public UserSession StartSession(string sessionId, string? deviceInfo = null, string? ipAddress = null, string? userAgent = null, string? location = null)
    {
        var session = new UserSession(Id, sessionId, deviceInfo, ipAddress, userAgent, location);
        Sessions.Add(session);
        Audit(UpdatedBy);
        return session;
    }

    public void EndSession(string sessionId)
    {
        var session = Sessions.FirstOrDefault(s => s.SessionId == sessionId && s.IsActive);
        session?.EndSession();
        Audit(UpdatedBy);
    }

    public void EndAllSessions()
    {
        foreach (var session in Sessions.Where(s => s.IsActive))
        {
            session.EndSession();
        }
        Audit(UpdatedBy);
    }

    public void ExpireInactiveSessions(TimeSpan timeout)
    {
        var expiredSessions = Sessions.Where(s => s.IsActive && s.IsExpired(timeout));
        foreach (var session in expiredSessions)
        {
            session.ExpireSession();
        }
        Audit(UpdatedBy);
    }
}