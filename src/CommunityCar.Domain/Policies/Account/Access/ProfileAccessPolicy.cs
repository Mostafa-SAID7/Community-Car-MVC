using CommunityCar.Domain.Policies;

namespace CommunityCar.Domain.Policies.Account.Access;

/// <summary>
/// Policy for controlling access to user profiles
/// </summary>
public class ProfileAccessPolicy : IAccessPolicy<ProfileAccessRequest>
{
    private readonly Func<Guid, Task<IEnumerable<string>>> _getUserRoles;
    private readonly Func<Guid, Task<ProfilePrivacySettings>> _getPrivacySettings;
    private readonly Func<Guid, Guid, Task<bool>> _areUsersConnected;

    public ProfileAccessPolicy(
        Func<Guid, Task<IEnumerable<string>>> getUserRoles,
        Func<Guid, Task<ProfilePrivacySettings>> getPrivacySettings,
        Func<Guid, Guid, Task<bool>> areUsersConnected)
    {
        _getUserRoles = getUserRoles ?? throw new ArgumentNullException(nameof(getUserRoles));
        _getPrivacySettings = getPrivacySettings ?? throw new ArgumentNullException(nameof(getPrivacySettings));
        _areUsersConnected = areUsersConnected ?? throw new ArgumentNullException(nameof(areUsersConnected));
    }

    public bool CanAccess(Guid userId, ProfileAccessRequest resource)
    {
        return CanAccessAsync(userId, resource).GetAwaiter().GetResult();
    }

    public async Task<bool> CanAccessAsync(Guid userId, ProfileAccessRequest request)
    {
        // Owner can always access their own profile
        if (userId == request.ProfileOwnerId)
            return true;

        // Check if user has admin privileges
        var userRoles = await _getUserRoles(userId);
        var adminRoles = new[] { "Admin", "SuperAdmin", "SystemAdmin", "Moderator" };
        if (userRoles.Any(role => adminRoles.Contains(role, StringComparer.OrdinalIgnoreCase)))
            return true;

        // Get privacy settings for the profile
        var privacySettings = await _getPrivacySettings(request.ProfileOwnerId);

        return request.AccessType switch
        {
            ProfileAccessType.ViewBasicInfo => await CanViewBasicInfoAsync(userId, request.ProfileOwnerId, privacySettings),
            ProfileAccessType.ViewDetailedInfo => await CanViewDetailedInfoAsync(userId, request.ProfileOwnerId, privacySettings),
            ProfileAccessType.ViewContactInfo => await CanViewContactInfoAsync(userId, request.ProfileOwnerId, privacySettings),
            ProfileAccessType.ViewActivity => await CanViewActivityAsync(userId, request.ProfileOwnerId, privacySettings),
            ProfileAccessType.ViewConnections => await CanViewConnectionsAsync(userId, request.ProfileOwnerId, privacySettings),
            ProfileAccessType.SendMessage => await CanSendMessageAsync(userId, request.ProfileOwnerId, privacySettings),
            ProfileAccessType.ViewGallery => await CanViewGalleryAsync(userId, request.ProfileOwnerId, privacySettings),
            _ => false
        };
    }

    private async Task<bool> CanViewBasicInfoAsync(Guid viewerId, Guid profileOwnerId, ProfilePrivacySettings settings)
    {
        return settings.BasicInfoVisibility switch
        {
            PrivacyLevel.Public => true,
            PrivacyLevel.Connections => await _areUsersConnected(viewerId, profileOwnerId),
            PrivacyLevel.Private => false,
            _ => false
        };
    }

    private async Task<bool> CanViewDetailedInfoAsync(Guid viewerId, Guid profileOwnerId, ProfilePrivacySettings settings)
    {
        return settings.DetailedInfoVisibility switch
        {
            PrivacyLevel.Public => true,
            PrivacyLevel.Connections => await _areUsersConnected(viewerId, profileOwnerId),
            PrivacyLevel.Private => false,
            _ => false
        };
    }

    private async Task<bool> CanViewContactInfoAsync(Guid viewerId, Guid profileOwnerId, ProfilePrivacySettings settings)
    {
        return settings.ContactInfoVisibility switch
        {
            PrivacyLevel.Public => true,
            PrivacyLevel.Connections => await _areUsersConnected(viewerId, profileOwnerId),
            PrivacyLevel.Private => false,
            _ => false
        };
    }

    private async Task<bool> CanViewActivityAsync(Guid viewerId, Guid profileOwnerId, ProfilePrivacySettings settings)
    {
        return settings.ActivityVisibility switch
        {
            PrivacyLevel.Public => true,
            PrivacyLevel.Connections => await _areUsersConnected(viewerId, profileOwnerId),
            PrivacyLevel.Private => false,
            _ => false
        };
    }

    private async Task<bool> CanViewConnectionsAsync(Guid viewerId, Guid profileOwnerId, ProfilePrivacySettings settings)
    {
        return settings.ConnectionsVisibility switch
        {
            PrivacyLevel.Public => true,
            PrivacyLevel.Connections => await _areUsersConnected(viewerId, profileOwnerId),
            PrivacyLevel.Private => false,
            _ => false
        };
    }

    private async Task<bool> CanSendMessageAsync(Guid senderId, Guid recipientId, ProfilePrivacySettings settings)
    {
        if (!settings.AllowMessages)
            return false;

        return settings.MessagePrivacy switch
        {
            PrivacyLevel.Public => true,
            PrivacyLevel.Connections => await _areUsersConnected(senderId, recipientId),
            PrivacyLevel.Private => false,
            _ => false
        };
    }

    private async Task<bool> CanViewGalleryAsync(Guid viewerId, Guid profileOwnerId, ProfilePrivacySettings settings)
    {
        return settings.GalleryVisibility switch
        {
            PrivacyLevel.Public => true,
            PrivacyLevel.Connections => await _areUsersConnected(viewerId, profileOwnerId),
            PrivacyLevel.Private => false,
            _ => false
        };
    }
}

public class ProfileAccessRequest
{
    public Guid ProfileOwnerId { get; set; }
    public ProfileAccessType AccessType { get; set; }
    public string RequestedField { get; set; } = string.Empty;
}

public enum ProfileAccessType
{
    ViewBasicInfo,
    ViewDetailedInfo,
    ViewContactInfo,
    ViewActivity,
    ViewConnections,
    SendMessage,
    ViewGallery
}

public class ProfilePrivacySettings
{
    public PrivacyLevel BasicInfoVisibility { get; set; } = PrivacyLevel.Public;
    public PrivacyLevel DetailedInfoVisibility { get; set; } = PrivacyLevel.Connections;
    public PrivacyLevel ContactInfoVisibility { get; set; } = PrivacyLevel.Connections;
    public PrivacyLevel ActivityVisibility { get; set; } = PrivacyLevel.Public;
    public PrivacyLevel ConnectionsVisibility { get; set; } = PrivacyLevel.Connections;
    public PrivacyLevel GalleryVisibility { get; set; } = PrivacyLevel.Connections;
    public PrivacyLevel MessagePrivacy { get; set; } = PrivacyLevel.Public;
    public bool AllowMessages { get; set; } = true;
    public bool ShowOnlineStatus { get; set; } = true;
    public bool AllowSearchEngineIndexing { get; set; } = false;
}

public enum PrivacyLevel
{
    Public = 0,      // Visible to everyone
    Connections = 1, // Visible to connected users only
    Private = 2      // Visible to owner only
}