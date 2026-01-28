using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Shared.DTOs;

public class BaseSearchRequest
{
    public string Query { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "CreatedAt";
    public string SortDirection { get; set; } = "DESC";
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Guid? UserId { get; set; }
}

public class UniversalSearchRequest : BaseSearchRequest
{
    public List<EntityType>? EntityTypes { get; set; }
    public List<string>? Categories { get; set; }
    public List<string>? Tags { get; set; }
    public bool IncludeDeleted { get; set; } = false;
    public SearchScope Scope { get; set; } = SearchScope.All;
}

public class BookmarkSearchRequest : BaseSearchRequest
{
    public EntityType? EntityType { get; set; }
    public bool HasNotes { get; set; } = false;
    public List<string>? Tags { get; set; }
}

public class CommentSearchRequest : BaseSearchRequest
{
    public EntityType? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public bool TopLevelOnly { get; set; } = false;
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
}

public class CategorySearchRequest : BaseSearchRequest
{
    public Guid? ParentCategoryId { get; set; }
    public bool RootCategoriesOnly { get; set; } = false;
    public bool IncludeChildren { get; set; } = true;
    public int? MaxDepth { get; set; }
}

public class TagSearchRequest : BaseSearchRequest
{
    public int? MinUsageCount { get; set; }
    public int? MaxUsageCount { get; set; }
    public bool PopularOnly { get; set; } = false;
    public string? StartsWith { get; set; }
}

public class ReactionSearchRequest : BaseSearchRequest
{
    public EntityType? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public ReactionType? ReactionType { get; set; }
    public List<ReactionType>? ReactionTypes { get; set; }
}

public class ShareSearchRequest : BaseSearchRequest
{
    public EntityType? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public ShareType? ShareType { get; set; }
    public string? Platform { get; set; }
    public List<string>? Platforms { get; set; }
}

public class RatingSearchRequest : BaseSearchRequest
{
    public EntityType? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public int? MinRating { get; set; }
    public int? MaxRating { get; set; }
    public bool HasReview { get; set; } = false;
}

public class VoteSearchRequest : BaseSearchRequest
{
    public EntityType? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public VoteType? VoteType { get; set; }
    public List<VoteType>? VoteTypes { get; set; }
}

public class ViewSearchRequest : BaseSearchRequest
{
    public EntityType? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public string? IpAddress { get; set; }
    public bool AuthenticatedOnly { get; set; } = false;
    public bool AnonymousOnly { get; set; } = false;
}

public enum SearchScope
{
    All,
    MyContent,
    Following,
    Bookmarked,
    Recent,
    Popular,
    Trending
}


