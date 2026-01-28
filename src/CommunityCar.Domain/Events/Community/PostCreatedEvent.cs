namespace CommunityCar.Domain.Events.Community;

/// <summary>
/// Domain event raised when a new post is created
/// </summary>
public class PostCreatedEvent : IDomainEvent
{
    public PostCreatedEvent(Guid postId, Guid authorId, string title, bool isPublished)
    {
        PostId = postId;
        AuthorId = authorId;
        Title = title;
        IsPublished = isPublished;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid PostId { get; }
    public Guid AuthorId { get; }
    public string Title { get; }
    public bool IsPublished { get; }
    public DateTime OccurredOn { get; }
}