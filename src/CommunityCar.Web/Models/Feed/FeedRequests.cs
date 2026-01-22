namespace CommunityCar.Web.Models.Feed;

public class MarkSeenRequest
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
}

public class InteractionRequest
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string InteractionType { get; set; } = string.Empty; // like, share, bookmark
}

public class CommentRequest
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}

public class BookmarkRequest
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
}

public class HideContentRequest
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
}

public class ReportContentRequest
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}