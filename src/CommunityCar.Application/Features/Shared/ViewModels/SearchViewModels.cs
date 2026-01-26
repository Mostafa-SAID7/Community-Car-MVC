using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Features.Shared.ViewModels;

public class SearchResultVM<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
    public string Query { get; set; } = string.Empty;
    public TimeSpan SearchDuration { get; set; }
    public List<SearchFacetVM> Facets { get; set; } = new();
}

public class UniversalSearchResultVM
{
    public List<SearchItemVM> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public string Query { get; set; } = string.Empty;
    public TimeSpan SearchDuration { get; set; }
    public Dictionary<EntityType, int> EntityTypeCounts { get; set; } = new();
    public List<SearchFacetVM> Facets { get; set; } = new();
    public List<SearchSuggestionVM> Suggestions { get; set; } = new();
}

public class SearchItemVM
{
    public Guid Id { get; set; }
    public EntityType EntityType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public Guid? AuthorId { get; set; }
    public List<string> Tags { get; set; } = new();
    public string Category { get; set; } = string.Empty;
    public double RelevanceScore { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class SearchFacetVM
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<SearchFacetValueVM> Values { get; set; } = new();
}

public class SearchFacetValueVM
{
    public string Value { get; set; } = string.Empty;
    public string DisplayValue { get; set; } = string.Empty;
    public int Count { get; set; }
    public bool IsSelected { get; set; }
}

public class SearchSuggestionVM
{
    public string Text { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Score { get; set; }
}

public class TrendingItemVM
{
    public Guid Id { get; set; }
    public EntityType EntityType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int InteractionCount { get; set; }
    public int ViewCount { get; set; }
    public double TrendingScore { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Tags { get; set; } = new();
}

public class SearchAnalyticsVM
{
    public int TotalSearches { get; set; }
    public List<PopularQueryVM> PopularQueries { get; set; } = new();
    public Dictionary<EntityType, int> SearchesByEntityType { get; set; } = new();
    public Dictionary<string, int> SearchesByTimeOfDay { get; set; } = new();
    public double AverageResultsPerSearch { get; set; }
    public double AverageSearchDuration { get; set; }
}

public class PopularQueryVM
{
    public string Query { get; set; } = string.Empty;
    public int Count { get; set; }
    public DateTime LastSearched { get; set; }
}

public class RelatedContentVM
{
    public Guid Id { get; set; }
    public EntityType EntityType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public double SimilarityScore { get; set; }
    public List<string> CommonTags { get; set; } = new();
    public string RelationType { get; set; } = string.Empty; // "Similar", "Related", "Recommended"
}

public class PopularContentVM
{
    public Guid Id { get; set; }
    public EntityType EntityType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public int InteractionCount { get; set; }
    public double PopularityScore { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Tags { get; set; } = new();
}

// Individual entity view models
public class BookmarkVM
{
    public Guid Id { get; set; }
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public Guid UserId { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public string EntityTitle { get; set; } = string.Empty;
    public string EntityUrl { get; set; } = string.Empty;
}

public class CommentVM
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public Guid? ParentCommentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<CommentVM> Replies { get; set; } = new();
    public int ReplyCount { get; set; }
}

public class CategoryVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public List<CategoryVM> Children { get; set; } = new();
    public int ItemCount { get; set; }
}

public class TagVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int UsageCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ReactionVM
{
    public Guid Id { get; set; }
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public ReactionType Type { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ShareVM
{
    public Guid Id { get; set; }
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public ShareType ShareType { get; set; }
    public string? ShareMessage { get; set; }
    public string? Platform { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RatingVM
{
    public Guid Id { get; set; }
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int Value { get; set; }
    public string? Review { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class VoteVM
{
    public Guid Id { get; set; }
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public VoteType Type { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ViewVM
{
    public Guid Id { get; set; }
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}