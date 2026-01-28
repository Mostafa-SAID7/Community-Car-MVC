using CommunityCar.Domain.Enums.Shared;
using CommunityCar.Application.Features.Shared.DTOs;
using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Shared;

public interface ISharedSearchService
{
    // Universal search across all shared entities
    Task<UniversalSearchResultVM> SearchAllAsync(UniversalSearchRequest request);
    
    // Entity-specific searches
    Task<SearchResultVM<BookmarkVM>> SearchBookmarksAsync(BookmarkSearchRequest request);
    Task<SearchResultVM<CommentVM>> SearchCommentsAsync(CommentSearchRequest request);
    Task<SearchResultVM<CategoryVM>> SearchCategoriesAsync(CategorySearchRequest request);
    Task<SearchResultVM<TagVM>> SearchTagsAsync(TagSearchRequest request);
    Task<SearchResultVM<ReactionVM>> SearchReactionsAsync(ReactionSearchRequest request);
    Task<SearchResultVM<ShareVM>> SearchSharesAsync(ShareSearchRequest request);
    Task<SearchResultVM<RatingVM>> SearchRatingsAsync(RatingSearchRequest request);
    Task<SearchResultVM<VoteVM>> SearchVotesAsync(VoteSearchRequest request);
    Task<SearchResultVM<ViewVM>> SearchViewsAsync(ViewSearchRequest request);
    
    // Advanced search features
    Task<List<SearchSuggestionVM>> GetSearchSuggestionsAsync(string query, int maxResults = 10);
    Task<List<TrendingItemVM>> GetTrendingItemsAsync(EntityType? entityType = null, int maxResults = 20);
    Task<SearchAnalyticsVM> GetSearchAnalyticsAsync(Guid? userId = null);
    
    // Content discovery
    Task<List<RelatedContentVM>> GetRelatedContentAsync(Guid entityId, EntityType entityType, int maxResults = 10);
    Task<List<PopularContentVM>> GetPopularContentAsync(EntityType? entityType = null, TimeSpan? timeRange = null, int maxResults = 20);
}


