using CommunityCar.Domain.Enums.Shared;
using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Shared;

public interface ISharedSearchService
{
    // Universal search across all shared entities
    Task<UniversalSearchResultVM> SearchAllAsync(UniversalSearchVM request);
    
    // Entity-specific searches
    Task<SearchResultVM<BookmarkVM>> SearchBookmarksAsync(BookmarkSearchVM request);
    Task<SearchResultVM<CommentVM>> SearchCommentsAsync(CommentSearchVM request);
    Task<SearchResultVM<CategoryVM>> SearchCategoriesAsync(CategorySearchVM request);
    Task<SearchResultVM<TagVM>> SearchTagsAsync(TagSearchVM request);
    Task<SearchResultVM<ReactionVM>> SearchReactionsAsync(ReactionSearchVM request);
    Task<SearchResultVM<ShareVM>> SearchSharesAsync(ShareSearchVM request);
    Task<SearchResultVM<RatingVM>> SearchRatingsAsync(RatingSearchVM request);
    Task<SearchResultVM<VoteVM>> SearchVotesAsync(VoteSearchVM request);
    Task<SearchResultVM<ViewVM>> SearchViewsAsync(ViewSearchVM request);
    
    // Advanced search features
    Task<List<SearchSuggestionVM>> GetSearchSuggestionsAsync(string query, int maxResults = 10);
    Task<List<TrendingItemVM>> GetTrendingItemsAsync(EntityType? entityType = null, int maxResults = 20);
    Task<SearchAnalyticsVM> GetSearchAnalyticsAsync(Guid? userId = null);
    
    // Content discovery
    Task<List<RelatedContentVM>> GetRelatedContentAsync(Guid entityId, EntityType entityType, int maxResults = 10);
    Task<List<PopularContentVM>> GetPopularContentAsync(EntityType? entityType = null, TimeSpan? timeRange = null, int maxResults = 20);
}


