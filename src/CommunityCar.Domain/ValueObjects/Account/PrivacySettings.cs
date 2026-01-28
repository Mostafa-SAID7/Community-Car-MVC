using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.ValueObjects.Account;

public class PrivacySettings : ValueObject
{
    public bool IsPublic { get; private set; }
    public bool ShowEmail { get; private set; }
    public bool ShowLocation { get; private set; }
    public bool ShowOnlineStatus { get; private set; }
    public bool AllowMessagesFromStrangers { get; private set; }
    public bool AllowTagging { get; private set; }
    public bool ShowActivityStatus { get; private set; }

    // Parameterless constructor for EF Core
    private PrivacySettings()
    {
        IsPublic = true;
        ShowEmail = false;
        ShowLocation = true;
        ShowOnlineStatus = true;
        AllowMessagesFromStrangers = true;
        AllowTagging = true;
        ShowActivityStatus = true;
    }

    public PrivacySettings(
        bool isPublic = true,
        bool showEmail = false,
        bool showLocation = true,
        bool showOnlineStatus = true,
        bool allowMessagesFromStrangers = true,
        bool allowTagging = true,
        bool showActivityStatus = true)
    {
        IsPublic = isPublic;
        ShowEmail = showEmail;
        ShowLocation = showLocation;
        ShowOnlineStatus = showOnlineStatus;
        AllowMessagesFromStrangers = allowMessagesFromStrangers;
        AllowTagging = allowTagging;
        ShowActivityStatus = showActivityStatus;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return IsPublic;
        yield return ShowEmail;
        yield return ShowLocation;
        yield return ShowOnlineStatus;
        yield return AllowMessagesFromStrangers;
        yield return AllowTagging;
        yield return ShowActivityStatus;
    }

    public static PrivacySettings Default => new();
    
    public static PrivacySettings Private => new(
        isPublic: false,
        showEmail: false,
        showLocation: false,
        showOnlineStatus: false,
        allowMessagesFromStrangers: false,
        allowTagging: false,
        showActivityStatus: false);

    public static PrivacySettings Public => new(
        isPublic: true,
        showEmail: true,
        showLocation: true,
        showOnlineStatus: true,
        allowMessagesFromStrangers: true,
        allowTagging: true,
        showActivityStatus: true);
}