using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.ValueObjects.Account;

public class NotificationSettings : ValueObject
{
    public bool EmailNotificationsEnabled { get; private set; }
    public bool PushNotificationsEnabled { get; private set; }
    public bool SmsNotificationsEnabled { get; private set; }
    public bool MarketingEmailsEnabled { get; private set; }
    public bool CommentNotificationsEnabled { get; private set; }
    public bool LikeNotificationsEnabled { get; private set; }
    public bool FollowNotificationsEnabled { get; private set; }
    public bool MessageNotificationsEnabled { get; private set; }

    // Parameterless constructor for EF Core
    private NotificationSettings()
    {
        EmailNotificationsEnabled = true;
        PushNotificationsEnabled = true;
        SmsNotificationsEnabled = false;
        MarketingEmailsEnabled = false;
        CommentNotificationsEnabled = true;
        LikeNotificationsEnabled = true;
        FollowNotificationsEnabled = true;
        MessageNotificationsEnabled = true;
    }

    public NotificationSettings(
        bool emailNotifications = true,
        bool pushNotifications = true,
        bool smsNotifications = false,
        bool marketingEmails = false,
        bool commentNotifications = true,
        bool likeNotifications = true,
        bool followNotifications = true,
        bool messageNotifications = true)
    {
        EmailNotificationsEnabled = emailNotifications;
        PushNotificationsEnabled = pushNotifications;
        SmsNotificationsEnabled = smsNotifications;
        MarketingEmailsEnabled = marketingEmails;
        CommentNotificationsEnabled = commentNotifications;
        LikeNotificationsEnabled = likeNotifications;
        FollowNotificationsEnabled = followNotifications;
        MessageNotificationsEnabled = messageNotifications;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return EmailNotificationsEnabled;
        yield return PushNotificationsEnabled;
        yield return SmsNotificationsEnabled;
        yield return MarketingEmailsEnabled;
        yield return CommentNotificationsEnabled;
        yield return LikeNotificationsEnabled;
        yield return FollowNotificationsEnabled;
        yield return MessageNotificationsEnabled;
    }

    public static NotificationSettings Default => new();
    
    public static NotificationSettings AllEnabled => new(
        emailNotifications: true,
        pushNotifications: true,
        smsNotifications: true,
        marketingEmails: true,
        commentNotifications: true,
        likeNotifications: true,
        followNotifications: true,
        messageNotifications: true);

    public static NotificationSettings AllDisabled => new(
        emailNotifications: false,
        pushNotifications: false,
        smsNotifications: false,
        marketingEmails: false,
        commentNotifications: false,
        likeNotifications: false,
        followNotifications: false,
        messageNotifications: false);
}