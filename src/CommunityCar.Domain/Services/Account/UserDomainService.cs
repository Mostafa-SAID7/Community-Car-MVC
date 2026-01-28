using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.ValueObjects.Common;

namespace CommunityCar.Domain.Services.Account;

/// <summary>
/// Domain service for user-related business logic
/// </summary>
public class UserDomainService : IDomainService
{
    /// <summary>
    /// Determines if two users can interact (follow, message, etc.)
    /// </summary>
    public bool CanUsersInteract(User user1, User user2)
    {
        // Users cannot interact with themselves
        if (user1.Id == user2.Id)
            return false;

        // Both users must be active
        if (!user1.IsActive || !user2.IsActive)
            return false;

        // Neither user should be deleted
        if (user1.IsDeleted || user2.IsDeleted)
            return false;

        // Additional business rules could be added here
        // e.g., checking for blocks, restrictions, etc.

        return true;
    }

    /// <summary>
    /// Calculates user reputation score based on various factors
    /// </summary>
    public int CalculateReputationScore(User user)
    {
        var score = 0;

        // Base score from total points
        score += user.TotalPoints;

        // Bonus for profile completeness
        if (!string.IsNullOrEmpty(user.Profile.ProfilePictureUrl))
            score += 10;

        if (!string.IsNullOrEmpty(user.Profile.Bio))
            score += 5;

        if (!string.IsNullOrEmpty(user.Profile.City))
            score += 5;

        // Bonus for account age (1 point per month)
        var accountAge = DateTime.UtcNow - user.CreatedAt;
        score += (int)accountAge.TotalDays / 30;

        // Bonus for verified email
        if (user.EmailConfirmed)
            score += 20;

        // Bonus for two-factor authentication
        if (user.TwoFactorSettings.TwoFactorEnabled)
            score += 15;

        return Math.Max(0, score);
    }

    /// <summary>
    /// Determines the user's trust level based on reputation and activity
    /// </summary>
    public UserTrustLevel GetUserTrustLevel(User user)
    {
        var reputation = CalculateReputationScore(user);
        var accountAge = DateTime.UtcNow - user.CreatedAt;

        if (reputation >= 1000 && accountAge.TotalDays >= 365)
            return UserTrustLevel.Trusted;

        if (reputation >= 500 && accountAge.TotalDays >= 180)
            return UserTrustLevel.Established;

        if (reputation >= 100 && accountAge.TotalDays >= 30)
            return UserTrustLevel.Regular;

        if (accountAge.TotalDays >= 7)
            return UserTrustLevel.New;

        return UserTrustLevel.Unverified;
    }

    /// <summary>
    /// Suggests username based on email or full name
    /// </summary>
    public List<string> SuggestUsernames(string email, string fullName)
    {
        var suggestions = new List<string>();

        if (!string.IsNullOrEmpty(email))
        {
            var emailPart = email.Split('@')[0];
            suggestions.Add(emailPart);
            suggestions.Add($"{emailPart}{DateTime.Now.Year}");
        }

        if (!string.IsNullOrEmpty(fullName))
        {
            var nameParts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (nameParts.Length >= 2)
            {
                suggestions.Add($"{nameParts[0].ToLower()}{nameParts[1].ToLower()}");
                suggestions.Add($"{nameParts[0].ToLower()}.{nameParts[1].ToLower()}");
                suggestions.Add($"{nameParts[0].ToLower()}_{nameParts[1].ToLower()}");
            }
            else if (nameParts.Length == 1)
            {
                suggestions.Add(nameParts[0].ToLower());
                suggestions.Add($"{nameParts[0].ToLower()}{DateTime.Now.Year}");
            }
        }

        return suggestions.Distinct().Take(5).ToList();
    }

    /// <summary>
    /// Validates if a user can perform a specific action based on their trust level
    /// </summary>
    public bool CanPerformAction(User user, UserAction action)
    {
        var trustLevel = GetUserTrustLevel(user);

        return action switch
        {
            UserAction.CreatePost => trustLevel >= UserTrustLevel.New,
            UserAction.CommentOnPost => trustLevel >= UserTrustLevel.New,
            UserAction.SendDirectMessage => trustLevel >= UserTrustLevel.Regular,
            UserAction.CreateGroup => trustLevel >= UserTrustLevel.Established,
            UserAction.ModerateContent => trustLevel >= UserTrustLevel.Trusted,
            _ => false
        };
    }
}

public enum UserTrustLevel
{
    Unverified = 0,
    New = 1,
    Regular = 2,
    Established = 3,
    Trusted = 4
}

public enum UserAction
{
    CreatePost,
    CommentOnPost,
    SendDirectMessage,
    CreateGroup,
    ModerateContent
}