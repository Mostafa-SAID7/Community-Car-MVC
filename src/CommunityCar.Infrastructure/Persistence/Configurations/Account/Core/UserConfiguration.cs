using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.ValueObjects.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunityCar.Infrastructure.Persistence.Configurations.Account.Core;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        // Configure Profile value object
        builder.OwnsOne(u => u.Profile, profile =>
        {
            profile.Property(p => p.FullName).HasMaxLength(100).HasColumnName("FullName");
            profile.Property(p => p.FirstName).HasMaxLength(50).HasColumnName("FirstName");
            profile.Property(p => p.LastName).HasMaxLength(50).HasColumnName("LastName");
            profile.Property(p => p.Bio).HasMaxLength(500).HasColumnName("Bio");
            profile.Property(p => p.BioAr).HasMaxLength(500).HasColumnName("BioAr");
            profile.Property(p => p.City).HasMaxLength(100).HasColumnName("City");
            profile.Property(p => p.CityAr).HasMaxLength(100).HasColumnName("CityAr");
            profile.Property(p => p.Country).HasMaxLength(100).HasColumnName("Country");
            profile.Property(p => p.CountryAr).HasMaxLength(100).HasColumnName("CountryAr");
            profile.Property(p => p.Website).HasMaxLength(200).HasColumnName("Website");
            profile.Property(p => p.ProfilePictureUrl).HasMaxLength(500).HasColumnName("ProfilePictureUrl");
            profile.Property(p => p.CoverImageUrl).HasMaxLength(500).HasColumnName("CoverImageUrl");
        });

        // Configure PrivacySettings value object
        builder.OwnsOne(u => u.PrivacySettings, privacy =>
        {
            privacy.Property(p => p.IsPublic).HasColumnName("IsPublic");
            privacy.Property(p => p.ShowEmail).HasColumnName("ShowEmail");
            privacy.Property(p => p.ShowLocation).HasColumnName("ShowLocation");
            privacy.Property(p => p.ShowOnlineStatus).HasColumnName("ShowOnlineStatus");
            privacy.Property(p => p.AllowMessagesFromStrangers).HasColumnName("AllowMessagesFromStrangers");
            privacy.Property(p => p.AllowTagging).HasColumnName("AllowTagging");
            privacy.Property(p => p.ShowActivityStatus).HasColumnName("ShowActivityStatus");
        });

        // Configure NotificationSettings value object
        builder.OwnsOne(u => u.NotificationSettings, notifications =>
        {
            notifications.Property(n => n.EmailNotificationsEnabled).HasColumnName("EmailNotificationsEnabled");
            notifications.Property(n => n.PushNotificationsEnabled).HasColumnName("PushNotificationsEnabled");
            notifications.Property(n => n.SmsNotificationsEnabled).HasColumnName("SmsNotificationsEnabled");
            notifications.Property(n => n.MarketingEmailsEnabled).HasColumnName("MarketingEmailsEnabled");
            notifications.Property(n => n.CommentNotificationsEnabled).HasColumnName("CommentNotificationsEnabled");
            notifications.Property(n => n.LikeNotificationsEnabled).HasColumnName("LikeNotificationsEnabled");
            notifications.Property(n => n.FollowNotificationsEnabled).HasColumnName("FollowNotificationsEnabled");
            notifications.Property(n => n.MessageNotificationsEnabled).HasColumnName("MessageNotificationsEnabled");
        });

        // Configure TwoFactorSettings value object
        builder.OwnsOne(u => u.TwoFactorSettings, twoFactor =>
        {
            twoFactor.Property(t => t.TwoFactorEnabled).HasColumnName("TwoFactorEnabled");
            twoFactor.Property(t => t.TwoFactorSecretKey).HasMaxLength(200).HasColumnName("TwoFactorSecretKey");
            twoFactor.Property(t => t.TwoFactorEnabledAt).HasColumnName("TwoFactorEnabledAt");
            twoFactor.Property(t => t.BackupCodes).HasColumnName("BackupCodes");
            twoFactor.Property(t => t.FailedTwoFactorAttempts).HasColumnName("FailedTwoFactorAttempts");
            twoFactor.Property(t => t.TwoFactorLockoutEnd).HasColumnName("TwoFactorLockoutEnd");
            twoFactor.Property(t => t.SmsEnabled).HasColumnName("SmsEnabled");
            twoFactor.Property(t => t.EmailTwoFactorEnabled).HasColumnName("EmailTwoFactorEnabled");
        });

        // Configure OAuthInfo value object
        builder.OwnsOne(u => u.OAuthInfo, oauth =>
        {
            oauth.Property(o => o.GoogleId).HasMaxLength(100).HasColumnName("GoogleId");
            oauth.Property(o => o.FacebookId).HasMaxLength(100).HasColumnName("FacebookId");
            oauth.Property(o => o.LastLoginAt).HasColumnName("LastLoginAt");
            oauth.Property(o => o.LastLoginProvider).HasMaxLength(50).HasColumnName("LastLoginProvider");
        });

        // Configure backward compatibility properties as ignored
        builder.Ignore(u => u.FullName);
        builder.Ignore(u => u.FirstName);
        builder.Ignore(u => u.LastName);
        builder.Ignore(u => u.Bio);
        builder.Ignore(u => u.City);
        builder.Ignore(u => u.Country);
        builder.Ignore(u => u.Website);
        builder.Ignore(u => u.ProfilePictureUrl);
        builder.Ignore(u => u.CoverImageUrl);
        builder.Ignore(u => u.IsPublic);
        builder.Ignore(u => u.ShowEmail);
        builder.Ignore(u => u.ShowLocation);
        builder.Ignore(u => u.ShowOnlineStatus);
        builder.Ignore(u => u.AllowMessagesFromStrangers);
        builder.Ignore(u => u.AllowTagging);
        builder.Ignore(u => u.ShowActivityStatus);
        builder.Ignore(u => u.EmailNotificationsEnabled);
        builder.Ignore(u => u.PushNotificationsEnabled);
        builder.Ignore(u => u.SmsNotificationsEnabled);
        builder.Ignore(u => u.MarketingEmailsEnabled);
        builder.Ignore(u => u.CommentNotificationsEnabled);
        builder.Ignore(u => u.LikeNotificationsEnabled);
        builder.Ignore(u => u.FollowNotificationsEnabled);
        builder.Ignore(u => u.MessageNotificationsEnabled);
        builder.Ignore(u => u.GoogleId);
        builder.Ignore(u => u.FacebookId);
        builder.Ignore(u => u.LastLoginAt);
        builder.Ignore(u => u.LastLoginProvider);
        builder.Ignore(u => u.TwoFactorSecretKey);
        builder.Ignore(u => u.TwoFactorEnabledAt);
        builder.Ignore(u => u.BackupCodes);
        builder.Ignore(u => u.FailedTwoFactorAttempts);
        builder.Ignore(u => u.TwoFactorLockoutEnd);
        builder.Ignore(u => u.SmsEnabled);
        builder.Ignore(u => u.EmailTwoFactorEnabled);
    }
}