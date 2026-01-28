namespace CommunityCar.Domain.Events.Community;

/// <summary>
/// Domain event raised when a post is liked
/// </summary>
public class PostLikedEvent : IDomainEvent
{
    public PostLikedEvent(Guid postId, Guid userId, Guid authorId)
    {
        PostId = postId;
        UserId = userId;
        AuthorId = authorId;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid PostId { get; }
    public Guid UserId { get; }
    public Guid AuthorId { get; }
    public DateTime OccurredOn { get; }
}