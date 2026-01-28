using CommunityCar.Domain.Entities.Account.Core;

namespace CommunityCar.Domain.Policies.Account;

/// <summary>
/// Policy for determining profile access permissions
/// </summary>
public class ProfileAccessPolicy : IAccessPolicy
{
    /// <summary>
    /// Determines if a user can view another user's profile
    /// </summary>
    public bool CanView(User viewer, User profileOwner)
    {
        // Profile owner can always view their own profile
        if (viewer.Id == profileOwner.Id)
            return true;

        // Inactive or deleted profiles cannot be viewed
        if (!profileOwner.IsActive || profileOwner.IsDeleted)
            return false;

        // Public profiles can be viewed by anyone
        if (profileOwner.PrivacySettings.IsPublic)
            return true;

        // For private profiles, check if viewer is following the profile owner
        // This would require a repository call, so we'll return false for now
        // In practice, this would be handled by the application layer
        return false;
    }

    /// <summary>
    /// Determines if a user can edit another user's profile
    /// </summary>
    public bool CanEdit(User editor, User profileOwner)
    {
        // Only the profile owner can edit their own profile
        // Admins could also edit profiles, but that would be handled separately
        return editor.Id == profileOwner.Id;
    }

    /// <summary>
    /// Determines if a user can view another user's private information
    /// </summary>
    public bool CanViewPrivateInfo(User viewer, User profileOwner)
    {
        // Profile owner can always view their own private info
        if (viewer.Id == profileOwner.Id)
            return true;

        // Check privacy settings
        if (!profileOwner.PrivacySettings.ShowEmail && 
            !profileOwner.PrivacySettings.ShowLocation &&
            !profileOwner.PrivacySettings.ShowOnlineStatus)
            return false;

        // For now, return false - this would need more complex logic
        // involving friendship/following relationships
        return false;
    }

    /// <summary>
    /// Determines if a user can contact another user
    /// </summary>
    public bool CanContact(User sender, User recipient)
    {
        // Cannot contact inactive or deleted users
        if (!recipient.IsActive || recipient.IsDeleted)
            return false;

        // Check if recipient allows messages from strangers
        if (!recipient.PrivacySettings.AllowMessagesFromStrangers)
        {
            // Would need to check if they are friends/following
            // For now, return false
            return false;
        }

        return true;
    }
}