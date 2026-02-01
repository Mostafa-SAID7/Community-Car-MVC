namespace CommunityCar.Application.Features.Shared.ViewModels.Notifications;

public class NewMessageNotificationRequest
{
    public Guid UserId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string ConversationId { get; set; } = string.Empty;
}

public class NewAnswerNotificationRequest
{
    public Guid UserId { get; set; }
    public string QuestionTitle { get; set; } = string.Empty;
    public Guid QuestionId { get; set; }
}

public class QuestionSolvedNotificationRequest
{
    public Guid UserId { get; set; }
    public string QuestionTitle { get; set; } = string.Empty;
    public Guid QuestionId { get; set; }
}

public class VoteReceivedNotificationRequest
{
    public Guid UserId { get; set; }
    public string ItemTitle { get; set; } = string.Empty;
    public bool IsUpvote { get; set; }
}

public class FriendRequestNotificationRequest
{
    public Guid UserId { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public Guid RequesterId { get; set; }
}