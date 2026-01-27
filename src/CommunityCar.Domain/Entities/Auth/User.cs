using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using CommunityCar.Domain.Events;
using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Auth;

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

    public string FullName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? BioAr { get; set; }
    public string? CityAr { get; set; }
    public string? CountryAr { get; set; }
    public string? Website { get; set; }
    public string? CoverImageUrl { get; set; }
    public bool IsActive { get; private set; }

    // Privacy Settings
    public bool IsPublic { get; set; } = true;
    public bool ShowEmail { get; set; } = false;
    public bool ShowLocation { get; set; } = true;
    public bool ShowOnlineStatus { get; set; } = true;
    public bool AllowMessagesFromStrangers { get; set; } = true;
    public bool AllowTagging { get; set; } = true;
    public bool ShowActivityStatus { get; set; } = true;
    public bool DataProcessingConsent { get; set; } = false;
    public bool MarketingEmailsConsent { get; set; } = false;
    
    // Security
    public DateTime? LastPasswordChangeAt { get; set; }

    // OAuth properties
    public string? GoogleId { get; set; }
    public string? FacebookId { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? LastLoginProvider { get; set; }

    // 2FA properties
    public bool IsTwoFactorEnabled { get; set; }
    public string? TwoFactorSecretKey { get; set; }
    public DateTime? TwoFactorEnabledAt { get; set; }
    public string? BackupCodes { get; set; } // JSON array of backup codes
    public int FailedTwoFactorAttempts { get; set; }
    public DateTime? TwoFactorLockoutEnd { get; set; }

    // SMS 2FA
    public bool SmsEnabled { get; set; }
    public string? SmsToken { get; set; }
    public DateTime? SmsTokenExpiry { get; set; }

    // Email 2FA
    public bool EmailTwoFactorEnabled { get; set; }
    public string? EmailTwoFactorToken { get; set; }
    public DateTime? EmailTwoFactorTokenExpiry { get; set; }

    // Re-implementing AggregateRoot properties
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public User(string email, string userName)
    {
        Email = email;
        UserName = userName;
        IsActive = true;
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        IsTwoFactorEnabled = false;
        SmsEnabled = false;
        EmailTwoFactorEnabled = false;
        FailedTwoFactorAttempts = 0;
    }

    // EF Core constructor
    public User() 
    {
        IsActive = true;
        IsTwoFactorEnabled = false;
        SmsEnabled = false;
        EmailTwoFactorEnabled = false;
        FailedTwoFactorAttempts = 0;
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

    public virtual void SoftDelete(string deletedBy)
    {
        if (IsDeleted) return;
        
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        IsActive = false; // Also deactivate the user
        UpdatedBy = deletedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public virtual void Restore(string restoredBy)
    {
        if (!IsDeleted) return;
        
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        IsActive = true; // Reactivate the user
        UpdatedBy = restoredBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLastLogin(string? provider = null)
    {
        LastLoginAt = DateTime.UtcNow;
        LastLoginProvider = provider;
        Audit(UpdatedBy);
    }

    public void LinkGoogleAccount(string googleId, string? profilePictureUrl = null)
    {
        GoogleId = googleId;
        if (!string.IsNullOrEmpty(profilePictureUrl))
        {
            ProfilePictureUrl = profilePictureUrl;
        }
        Audit(UpdatedBy);
    }

    public void LinkFacebookAccount(string facebookId, string? profilePictureUrl = null)
    {
        FacebookId = facebookId;
        if (!string.IsNullOrEmpty(profilePictureUrl))
        {
            ProfilePictureUrl = profilePictureUrl;
        }
        Audit(UpdatedBy);
    }

    public void UnlinkGoogleAccount()
    {
        GoogleId = null;
        Audit(UpdatedBy);
    }

    public void UnlinkFacebookAccount()
    {
        FacebookId = null;
        Audit(UpdatedBy);
    }

    public void EnableTwoFactor(string secretKey)
    {
        IsTwoFactorEnabled = true;
        TwoFactorSecretKey = secretKey;
        TwoFactorEnabledAt = DateTime.UtcNow;
        TwoFactorEnabled = true; // Identity property
        ResetTwoFactorFailures();
        Audit(UpdatedBy);
    }

    public void DisableTwoFactor()
    {
        IsTwoFactorEnabled = false;
        TwoFactorSecretKey = null;
        TwoFactorEnabledAt = null;
        TwoFactorEnabled = false; // Identity property
        BackupCodes = null;
        ResetTwoFactorFailures();
        Audit(UpdatedBy);
    }

    public void SetBackupCodes(string backupCodesJson)
    {
        BackupCodes = backupCodesJson;
        Audit(UpdatedBy);
    }

    public void IncrementTwoFactorFailures()
    {
        FailedTwoFactorAttempts++;
        if (FailedTwoFactorAttempts >= 5)
        {
            TwoFactorLockoutEnd = DateTime.UtcNow.AddMinutes(15);
        }
        Audit(UpdatedBy);
    }

    public void ResetTwoFactorFailures()
    {
        FailedTwoFactorAttempts = 0;
        TwoFactorLockoutEnd = null;
        Audit(UpdatedBy);
    }

    public bool IsTwoFactorLockedOut()
    {
        return TwoFactorLockoutEnd.HasValue && TwoFactorLockoutEnd > DateTime.UtcNow;
    }

    public void SetSmsToken(string token, TimeSpan expiry)
    {
        SmsToken = token;
        SmsTokenExpiry = DateTime.UtcNow.Add(expiry);
        Audit(UpdatedBy);
    }

    public void SetEmailTwoFactorToken(string token, TimeSpan expiry)
    {
        EmailTwoFactorToken = token;
        EmailTwoFactorTokenExpiry = DateTime.UtcNow.Add(expiry);
        Audit(UpdatedBy);
    }

    public bool IsValidSmsToken(string token)
    {
        return SmsToken == token && 
               SmsTokenExpiry.HasValue && 
               SmsTokenExpiry > DateTime.UtcNow;
    }

    public bool IsValidEmailTwoFactorToken(string token)
    {
        return EmailTwoFactorToken == token && 
               EmailTwoFactorTokenExpiry.HasValue && 
               EmailTwoFactorTokenExpiry > DateTime.UtcNow;
    }

    public void ClearSmsToken()
    {
        SmsToken = null;
        SmsTokenExpiry = null;
        Audit(UpdatedBy);
    }

    public void ClearEmailTwoFactorToken()
    {
        EmailTwoFactorToken = null;
        EmailTwoFactorTokenExpiry = null;
        Audit(UpdatedBy);
    }

    public void UpdateNotificationSettings(bool emailNotifications, bool pushNotifications, bool smsNotifications, bool marketingEmails)
    {
        // These would be stored in a separate UserSettings entity or as JSON in a settings column
        // For now, we'll just audit the change
        Audit(UpdatedBy);
    }

    public void UpdatePrivacySettings(Dictionary<string, bool> settings)
    {
        // These would be stored in a separate UserPrivacySettings entity or as JSON in a settings column
        // For now, we'll just audit the change
        Audit(UpdatedBy);
    }

    public Dictionary<string, bool> GetPrivacySettings()
    {
        // These would be retrieved from a separate UserPrivacySettings entity or from a JSON settings column
        // For now, return default settings
        return new Dictionary<string, bool>
        {
            ["ProfileVisible"] = true,
            ["EmailVisible"] = false,
            ["PhoneVisible"] = false,
            ["AllowMessages"] = true,
            ["AllowFriendRequests"] = true
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
}
