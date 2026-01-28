using CommunityCar.Domain.Entities.Account.Core;

namespace CommunityCar.Domain.Policies.Community;

/// <summary>
/// Policy for content moderation decisions
/// </summary>
public class ContentModerationPolicy : IAccessPolicy
{
    /// <summary>
    /// Determines if content should be auto-flagged for review
    /// </summary>
    public bool ShouldAutoFlag(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return false;

        var flaggedWords = new[]
        {
            "spam", "scam", "fake", "inappropriate", "offensive"
            // In practice, this would be a more comprehensive list
        };

        return flaggedWords.Any(word => 
            content.Contains(word, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Determines if a user can moderate content
    /// </summary>
    public bool CanModerate(User user)
    {
        // Check if user has moderator role
        // This would typically check user roles/permissions
        return user.TotalPoints >= 1000; // Simple example based on points
    }

    /// <summary>
    /// Determines if content can be published immediately
    /// </summary>
    public bool CanPublishImmediately(User author, string content)
    {
        // New users might need approval
        if (author.CreatedAt > DateTime.UtcNow.AddDays(-7))
            return false;

        // Users with low reputation might need approval
        if (author.TotalPoints < 100)
            return false;

        // Content that triggers auto-flagging needs review
        if (ShouldAutoFlag(content))
            return false;

        return true;
    }

    /// <summary>
    /// Determines if a user can delete content
    /// </summary>
    public bool CanDelete(User user, Guid contentAuthorId)
    {
        // Content author can delete their own content
        if (user.Id == contentAuthorId)
            return true;

        // Moderators can delete content
        if (CanModerate(user))
            return true;

        return false;
    }

    /// <summary>
    /// Determines if a user can edit content
    /// </summary>
    public bool CanEdit(User user, Guid contentAuthorId, DateTime contentCreatedAt)
    {
        // Only content author can edit
        if (user.Id != contentAuthorId)
            return false;

        // Cannot edit content older than 24 hours (example rule)
        if (DateTime.UtcNow - contentCreatedAt > TimeSpan.FromHours(24))
            return false;

        return true;
    }

    /// <summary>
    /// Determines if content should be hidden due to reports
    /// </summary>
    public bool ShouldHideContent(int reportCount, int viewCount)
    {
        if (viewCount == 0)
            return false;

        // Hide if more than 10% of viewers reported it
        var reportRatio = (double)reportCount / viewCount;
        return reportRatio > 0.1 && reportCount >= 3;
    }
}