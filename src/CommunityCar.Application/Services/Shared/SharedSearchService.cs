using CommunityCar.Application.Common.Interfaces.Services.Shared;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Application.Features.Shared.DTOs;
using CommunityCar.Application.Features.Shared.ViewModels;
using CommunityCar.Domain.Enums.Shared;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CommunityCar.Application.Services.Shared;

public class SharedSearchService : ISharedSearchService
{
    private readonly IBookmarkRepository _bookmarkRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IReactionRepository _reactionRepository;
    private readonly IShareRepository _shareRepository;
    private readonly IRatingRepository _ratingRepository;
    private readonly IVoteRepository _voteRepository;
    private readonly IViewRepository _viewRepository;
    private readonly ILogger<SharedSearchService> _logger;

    public SharedSearchService(
        IBookmarkRepository bookmarkRepository,
        ICommentRepository commentRepository,
        ICategoryRepository categoryRepository,
        ITagRepository tagRepository,
        IReactionRepository reactionRepository,
        IShareRepository shareRepository,
        IRatingRepository ratingRepository,
        IVoteRepository voteRepository,
        IViewRepository viewRepository,
        ILogger<SharedSearchService> logger)
    {
        _bookmarkRepository = bookmarkRepository;
        _commentRepository = commentRepository;
        _categoryRepository = categoryRepository;
        _tagRepository = tagRepository;
        _reactionRepository = reactionRepository;
        _shareRepository = shareRepository;
        _ratingRepository = ratingRepository;
        _voteRepository = voteRepository;
        _viewRepository = viewRepository;
        _logger = logger;
    }

    public async Task<UniversalSearchResultVM> SearchAllAsync(UniversalSearchRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new UniversalSearchResultVM
        {
            Query = request.Query,
            Page = request.Page,
            PageSize = request.PageSize
        };

        try
        {
            var searchTasks = new List<Task>();
            var searchResults = new List<SearchItemVM>();

            // Define entity types to search - use request types or default to all
            var entityTypes = request.EntityTypes?.Any() == true 
                ? request.EntityTypes 
                : Enum.GetValues<EntityType>().ToList();

            // Search across entity types sequentially to avoid DbContext concurrency issues
            foreach (var entityType in entityTypes)
            {
                var typeResults = entityType switch
                {
                    EntityType.Bookmark => await SearchBookmarksInternalAsync(request),
                    EntityType.Comment => await SearchCommentsInternalAsync(request),
                    EntityType.Category => await SearchCategoriesInternalAsync(request),
                    EntityType.Tag => await SearchTagsInternalAsync(request),
                    EntityType.Reaction => await SearchReactionsInternalAsync(request),
                    EntityType.Share => await SearchSharesInternalAsync(request),
                    EntityType.Rating => await SearchRatingsInternalAsync(request),
                    EntityType.Vote => await SearchVotesInternalAsync(request),
                    EntityType.View => await SearchViewsInternalAsync(request),
                    _ => new List<SearchItemVM>()
                };
                searchResults.AddRange(typeResults);
            }

            // Calculate relevance scores and sort
            searchResults = CalculateRelevanceScores(searchResults, request.Query)
                .OrderByDescending(r => r.RelevanceScore)
                .ToList();

            // Apply pagination
            result.TotalCount = searchResults.Count;
            result.Items = searchResults
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Generate entity type counts
            result.EntityTypeCounts = searchResults
                .GroupBy(r => r.EntityType)
                .ToDictionary(g => g.Key, g => g.Count());

            // Generate facets
            result.Facets = GenerateSearchFacets(searchResults, request);

            // Generate suggestions
            result.Suggestions = await GetSearchSuggestionsAsync(request.Query);

            stopwatch.Stop();
            result.SearchDuration = stopwatch.Elapsed;

            _logger.LogInformation("Universal search completed in {Duration}ms for query: {Query}", 
                stopwatch.ElapsedMilliseconds, request.Query);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing universal search for query: {Query}", request.Query);
            stopwatch.Stop();
            result.SearchDuration = stopwatch.Elapsed;
            return result;
        }
    }

    public async Task<SearchResultVM<BookmarkVM>> SearchBookmarksAsync(BookmarkSearchRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var bookmarks = await _bookmarkRepository.GetUserBookmarksAsync(
                request.UserId ?? Guid.Empty, 
                request.EntityType);

            var filteredBookmarks = bookmarks.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Query))
            {
                filteredBookmarks = filteredBookmarks.Where(b => 
                    (b.Note != null && b.Note.Contains(request.Query, StringComparison.OrdinalIgnoreCase)));
            }

            if (request.HasNotes)
            {
                filteredBookmarks = filteredBookmarks.Where(b => !string.IsNullOrEmpty(b.Note));
            }

            var totalCount = filteredBookmarks.Count();
            var items = filteredBookmarks
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(b => new BookmarkVM
                {
                    Id = b.Id,
                    EntityId = b.EntityId,
                    EntityType = b.EntityType,
                    UserId = b.UserId,
                    Note = b.Note,
                    CreatedAt = b.CreatedAt,
                    EntityTitle = GetEntityTitle(b.EntityId, b.EntityType),
                    EntityUrl = GetEntityUrl(b.EntityId, b.EntityType)
                })
                .ToList();

            stopwatch.Stop();
            return new SearchResultVM<BookmarkVM>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                Query = request.Query,
                SearchDuration = stopwatch.Elapsed
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching bookmarks");
            stopwatch.Stop();
            return new SearchResultVM<BookmarkVM>
            {
                SearchDuration = stopwatch.Elapsed,
                Query = request.Query
            };
        }
    }

    public async Task<SearchResultVM<CommentVM>> SearchCommentsAsync(CommentSearchRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var comments = request.EntityId.HasValue
                ? await _commentRepository.GetEntityCommentsAsync(request.EntityId.Value, request.EntityType ?? EntityType.Post)
                : await _commentRepository.GetUserCommentsAsync(request.UserId ?? Guid.Empty, request.EntityType);

            var filteredComments = comments.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Query))
            {
                filteredComments = filteredComments.Where(c => 
                    c.Content.Contains(request.Query, StringComparison.OrdinalIgnoreCase));
            }

            if (request.TopLevelOnly)
            {
                filteredComments = filteredComments.Where(c => c.ParentCommentId == null);
            }

            if (request.MinLength.HasValue)
            {
                filteredComments = filteredComments.Where(c => c.Content.Length >= request.MinLength.Value);
            }

            if (request.MaxLength.HasValue)
            {
                filteredComments = filteredComments.Where(c => c.Content.Length <= request.MaxLength.Value);
            }

            var items = filteredComments.Select(c => new CommentVM
            {
                Id = c.Id,
                Content = c.Content,
                EntityId = c.EntityId,
                EntityType = c.EntityType,
                AuthorId = c.AuthorId,
                AuthorName = GetUserName(c.AuthorId),
                ParentCommentId = c.ParentCommentId,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                ReplyCount = c.Replies.Count
            }).ToList();

            stopwatch.Stop();
            return new SearchResultVM<CommentVM>
            {
                Items = items,
                TotalCount = items.Count,
                Page = request.Page,
                PageSize = request.PageSize,
                Query = request.Query,
                SearchDuration = stopwatch.Elapsed
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching comments");
            stopwatch.Stop();
            return new SearchResultVM<CommentVM>
            {
                SearchDuration = stopwatch.Elapsed,
                Query = request.Query
            };
        }
    }

    public async Task<SearchResultVM<CategoryVM>> SearchCategoriesAsync(CategorySearchRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var categories = request.RootCategoriesOnly
                ? await _categoryRepository.GetRootCategoriesAsync()
                : await _categoryRepository.GetAllAsync();

            var filteredCategories = categories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Query))
            {
                filteredCategories = filteredCategories.Where(c => 
                    c.Name.Contains(request.Query, StringComparison.OrdinalIgnoreCase) ||
                    (c.Description != null && c.Description.Contains(request.Query, StringComparison.OrdinalIgnoreCase)));
            }

            if (request.ParentCategoryId.HasValue)
            {
                filteredCategories = filteredCategories.Where(c => c.ParentCategoryId == request.ParentCategoryId.Value);
            }

            var totalCount = filteredCategories.Count();
            var items = filteredCategories
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new CategoryVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    Description = c.Description,
                    ParentCategoryId = c.ParentCategoryId,
                    ParentCategoryName = GetCategoryName(c.ParentCategoryId),
                    ItemCount = GetCategoryItemCount(c.Id)
                })
                .ToList();

            stopwatch.Stop();
            return new SearchResultVM<CategoryVM>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                Query = request.Query,
                SearchDuration = stopwatch.Elapsed
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching categories");
            stopwatch.Stop();
            return new SearchResultVM<CategoryVM>
            {
                SearchDuration = stopwatch.Elapsed,
                Query = request.Query
            };
        }
    }

    public async Task<SearchResultVM<TagVM>> SearchTagsAsync(TagSearchRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var tags = request.PopularOnly
                ? await _tagRepository.GetPopularTagsAsync(request.PageSize * 2)
                : await _tagRepository.GetAllAsync();

            var filteredTags = tags.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Query))
            {
                filteredTags = filteredTags.Where(t => 
                    t.Name.Contains(request.Query, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(request.StartsWith))
            {
                filteredTags = filteredTags.Where(t => 
                    t.Name.StartsWith(request.StartsWith, StringComparison.OrdinalIgnoreCase));
            }

            if (request.MinUsageCount.HasValue)
            {
                filteredTags = filteredTags.Where(t => t.UsageCount >= request.MinUsageCount.Value);
            }

            if (request.MaxUsageCount.HasValue)
            {
                filteredTags = filteredTags.Where(t => t.UsageCount <= request.MaxUsageCount.Value);
            }

            var totalCount = filteredTags.Count();
            var items = filteredTags
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(t => new TagVM
                {
                    Id = t.Id,
                    Name = t.Name,
                    Slug = t.Slug,
                    UsageCount = t.UsageCount,
                    CreatedAt = t.CreatedAt
                })
                .ToList();

            stopwatch.Stop();
            return new SearchResultVM<TagVM>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                Query = request.Query,
                SearchDuration = stopwatch.Elapsed
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching tags");
            stopwatch.Stop();
            return new SearchResultVM<TagVM>
            {
                SearchDuration = stopwatch.Elapsed,
                Query = request.Query
            };
        }
    }

    // Implement other search methods...
    public async Task<SearchResultVM<ReactionVM>> SearchReactionsAsync(ReactionSearchRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var reactions = request.EntityId.HasValue
                ? await _reactionRepository.GetEntityReactionsAsync(request.EntityId.Value, request.EntityType ?? EntityType.Post)
                : await _reactionRepository.GetUserReactionsAsync(request.UserId ?? Guid.Empty, request.EntityType);

            var filteredReactions = reactions.AsQueryable();

            if (request.ReactionType.HasValue)
            {
                filteredReactions = filteredReactions.Where(r => r.Type == request.ReactionType.Value);
            }

            if (request.ReactionTypes?.Any() == true)
            {
                filteredReactions = filteredReactions.Where(r => request.ReactionTypes.Contains(r.Type));
            }

            var totalCount = filteredReactions.Count();
            var items = filteredReactions
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(r => new ReactionVM
                {
                    Id = r.Id,
                    EntityId = r.EntityId,
                    EntityType = r.EntityType,
                    UserId = r.UserId,
                    UserName = GetUserName(r.UserId),
                    Type = r.Type,
                    CreatedAt = r.CreatedAt
                })
                .ToList();

            stopwatch.Stop();
            return new SearchResultVM<ReactionVM>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                Query = request.Query,
                SearchDuration = stopwatch.Elapsed
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching reactions");
            stopwatch.Stop();
            return new SearchResultVM<ReactionVM>
            {
                SearchDuration = stopwatch.Elapsed,
                Query = request.Query
            };
        }
    }

    public async Task<SearchResultVM<ShareVM>> SearchSharesAsync(ShareSearchRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var shares = request.EntityId.HasValue
                ? await _shareRepository.GetEntitySharesAsync(request.EntityId.Value, request.EntityType ?? EntityType.Post)
                : await _shareRepository.GetUserSharesAsync(request.UserId ?? Guid.Empty, request.EntityType);

            var filteredShares = shares.AsQueryable();

            if (request.ShareType.HasValue)
            {
                filteredShares = filteredShares.Where(s => s.ShareType == request.ShareType.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Platform))
            {
                filteredShares = filteredShares.Where(s => 
                    s.Platform != null && s.Platform.Contains(request.Platform, StringComparison.OrdinalIgnoreCase));
            }

            if (request.Platforms?.Any() == true)
            {
                filteredShares = filteredShares.Where(s => 
                    s.Platform != null && request.Platforms.Contains(s.Platform));
            }

            var totalCount = filteredShares.Count();
            var items = filteredShares
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new ShareVM
                {
                    Id = s.Id,
                    EntityId = s.EntityId,
                    EntityType = s.EntityType,
                    UserId = s.UserId,
                    UserName = GetUserName(s.UserId),
                    ShareType = s.ShareType,
                    ShareMessage = s.ShareMessage,
                    Platform = s.Platform,
                    CreatedAt = s.CreatedAt
                })
                .ToList();

            stopwatch.Stop();
            return new SearchResultVM<ShareVM>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                Query = request.Query,
                SearchDuration = stopwatch.Elapsed
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching shares");
            stopwatch.Stop();
            return new SearchResultVM<ShareVM>
            {
                SearchDuration = stopwatch.Elapsed,
                Query = request.Query
            };
        }
    }

    public async Task<SearchResultVM<RatingVM>> SearchRatingsAsync(RatingSearchRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var ratings = request.EntityId.HasValue
                ? await _ratingRepository.GetEntityRatingsAsync(request.EntityId.Value, request.EntityType ?? EntityType.Post)
                : new List<Domain.Entities.Shared.Rating>();

            var filteredRatings = ratings.AsQueryable();

            if (request.MinRating.HasValue)
            {
                filteredRatings = filteredRatings.Where(r => r.Value >= request.MinRating.Value);
            }

            if (request.MaxRating.HasValue)
            {
                filteredRatings = filteredRatings.Where(r => r.Value <= request.MaxRating.Value);
            }

            if (request.HasReview)
            {
                filteredRatings = filteredRatings.Where(r => !string.IsNullOrEmpty(r.Review));
            }

            if (!string.IsNullOrWhiteSpace(request.Query) && request.HasReview)
            {
                filteredRatings = filteredRatings.Where(r => 
                    r.Review != null && r.Review.Contains(request.Query, StringComparison.OrdinalIgnoreCase));
            }

            var totalCount = filteredRatings.Count();
            var items = filteredRatings
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(r => new RatingVM
                {
                    Id = r.Id,
                    EntityId = r.EntityId,
                    EntityType = r.EntityType,
                    UserId = r.UserId,
                    UserName = GetUserName(r.UserId),
                    Value = r.Value,
                    Review = r.Review,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                })
                .ToList();

            stopwatch.Stop();
            return new SearchResultVM<RatingVM>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                Query = request.Query,
                SearchDuration = stopwatch.Elapsed
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching ratings");
            stopwatch.Stop();
            return new SearchResultVM<RatingVM>
            {
                SearchDuration = stopwatch.Elapsed,
                Query = request.Query
            };
        }
    }

    public async Task<SearchResultVM<VoteVM>> SearchVotesAsync(VoteSearchRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var votes = request.EntityId.HasValue
                ? await _voteRepository.GetVotesByEntityAsync(request.EntityId.Value, request.EntityType ?? EntityType.Question)
                : await _voteRepository.GetUserVotesAsync(request.UserId ?? Guid.Empty, request.EntityType);

            var filteredVotes = votes.AsQueryable();

            if (request.VoteType.HasValue)
            {
                filteredVotes = filteredVotes.Where(v => v.Type == request.VoteType.Value);
            }

            if (request.VoteTypes?.Any() == true)
            {
                filteredVotes = filteredVotes.Where(v => request.VoteTypes.Contains(v.Type));
            }

            var totalCount = filteredVotes.Count();
            var items = filteredVotes
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(v => new VoteVM
                {
                    Id = v.Id,
                    EntityId = v.EntityId,
                    EntityType = v.EntityType,
                    UserId = v.UserId,
                    UserName = GetUserName(v.UserId),
                    Type = v.Type,
                    CreatedAt = v.CreatedAt
                })
                .ToList();

            stopwatch.Stop();
            return new SearchResultVM<VoteVM>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                Query = request.Query,
                SearchDuration = stopwatch.Elapsed
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching votes");
            stopwatch.Stop();
            return new SearchResultVM<VoteVM>
            {
                SearchDuration = stopwatch.Elapsed,
                Query = request.Query
            };
        }
    }

    public async Task<SearchResultVM<ViewVM>> SearchViewsAsync(ViewSearchRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var views = request.EntityId.HasValue
                ? await _viewRepository.GetViewsByEntityAsync(request.EntityId.Value, request.EntityType ?? EntityType.Post)
                : await _viewRepository.GetUserViewsAsync(request.UserId ?? Guid.Empty, request.EntityType);

            var filteredViews = views.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.IpAddress))
            {
                filteredViews = filteredViews.Where(v => v.IpAddress == request.IpAddress);
            }

            if (request.AuthenticatedOnly)
            {
                filteredViews = filteredViews.Where(v => v.UserId.HasValue);
            }

            if (request.AnonymousOnly)
            {
                filteredViews = filteredViews.Where(v => !v.UserId.HasValue);
            }

            var totalCount = filteredViews.Count();
            var items = filteredViews
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(v => new ViewVM
                {
                    Id = v.Id,
                    EntityId = v.EntityId,
                    EntityType = v.EntityType,
                    UserId = v.UserId,
                    UserName = v.UserId.HasValue ? GetUserName(v.UserId.Value) : null,
                    IpAddress = v.IpAddress,
                    UserAgent = v.UserAgent,
                    CreatedAt = v.CreatedAt
                })
                .ToList();

            stopwatch.Stop();
            return new SearchResultVM<ViewVM>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                Query = request.Query,
                SearchDuration = stopwatch.Elapsed
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching views");
            stopwatch.Stop();
            return new SearchResultVM<ViewVM>
            {
                SearchDuration = stopwatch.Elapsed,
                Query = request.Query
            };
        }
    }

    public async Task<List<SearchSuggestionVM>> GetSearchSuggestionsAsync(string query, int maxResults = 10)
    {
        try
        {
            var suggestions = new List<SearchSuggestionVM>();

            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                return suggestions;

            // Get tag suggestions
            var tags = await _tagRepository.SearchTagsAsync(query, maxResults / 2);
            suggestions.AddRange(tags.Select(t => new SearchSuggestionVM
            {
                Text = t.Name,
                Type = "Tag",
                Count = t.UsageCount,
                Score = CalculateTagSuggestionScore(t.Name, query, t.UsageCount)
            }));

            // Get category suggestions
            var categories = await _categoryRepository.GetAllAsync();
            var matchingCategories = categories
                .Where(c => c.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                .Take(maxResults / 2)
                .Select(c => new SearchSuggestionVM
                {
                    Text = c.Name,
                    Type = "Category",
                    Count = GetCategoryItemCount(c.Id),
                    Score = CalculateCategorySuggestionScore(c.Name, query)
                });

            suggestions.AddRange(matchingCategories);

            return suggestions
                .OrderByDescending(s => s.Score)
                .Take(maxResults)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting search suggestions for query: {Query}", query);
            return new List<SearchSuggestionVM>();
        }
    }

    public async Task<List<TrendingItemVM>> GetTrendingItemsAsync(EntityType? entityType = null, int maxResults = 20)
    {
        try
        {
            // This would typically involve complex analytics
            // For now, return a simple implementation
            var trendingItems = new List<TrendingItemVM>();

            // Get popular tags as trending items
            var popularTags = await _tagRepository.GetPopularTagsAsync(maxResults);
            trendingItems.AddRange(popularTags.Select(t => new TrendingItemVM
            {
                Id = t.Id,
                EntityType = EntityType.Tag,
                Title = t.Name,
                Description = $"Used {t.UsageCount} times",
                Url = $"/tags/{t.Slug}",
                InteractionCount = t.UsageCount,
                TrendingScore = CalculateTrendingScore(t.UsageCount, t.CreatedAt),
                CreatedAt = t.CreatedAt,
                Tags = new List<string> { t.Name }
            }));

            return trendingItems
                .OrderByDescending(t => t.TrendingScore)
                .Take(maxResults)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trending items");
            return new List<TrendingItemVM>();
        }
    }

    public async Task<SearchAnalyticsVM> GetSearchAnalyticsAsync(Guid? userId = null)
    {
        try
        {
            // This would typically involve search analytics data
            // For now, return a basic implementation
            return new SearchAnalyticsVM
            {
                TotalSearches = 0,
                PopularQueries = new List<PopularQueryVM>(),
                SearchesByEntityType = new Dictionary<EntityType, int>(),
                SearchesByTimeOfDay = new Dictionary<string, int>(),
                AverageResultsPerSearch = 0,
                AverageSearchDuration = 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting search analytics");
            return new SearchAnalyticsVM();
        }
    }

    public async Task<List<RelatedContentVM>> GetRelatedContentAsync(Guid entityId, EntityType entityType, int maxResults = 10)
    {
        try
        {
            // This would involve complex similarity algorithms
            // For now, return a basic implementation
            return new List<RelatedContentVM>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting related content for {EntityType} {EntityId}", entityType, entityId);
            return new List<RelatedContentVM>();
        }
    }

    public async Task<List<PopularContentVM>> GetPopularContentAsync(EntityType? entityType = null, TimeSpan? timeRange = null, int maxResults = 20)
    {
        try
        {
            // This would involve view counts and interaction analytics
            // For now, return a basic implementation
            return new List<PopularContentVM>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular content");
            return new List<PopularContentVM>();
        }
    }

    // Private helper methods
    private async Task<List<SearchItemVM>> SearchBookmarksInternalAsync(UniversalSearchRequest request)
    {
        if (request.UserId == null) return new List<SearchItemVM>();

        var bookmarks = await _bookmarkRepository.GetUserBookmarksAsync(request.UserId.Value);
        return bookmarks
            .Where(b => string.IsNullOrWhiteSpace(request.Query) || 
                       (b.Note != null && b.Note.Contains(request.Query, StringComparison.OrdinalIgnoreCase)))
            .Select(b => new SearchItemVM
            {
                Id = b.Id,
                EntityType = EntityType.Bookmark,
                Title = $"Bookmark: {GetEntityTitle(b.EntityId, b.EntityType)}",
                Description = b.Note ?? "No notes",
                Url = GetEntityUrl(b.EntityId, b.EntityType),
                CreatedAt = b.CreatedAt,
                AuthorId = b.UserId,
                AuthorName = GetUserName(b.UserId)
            })
            .ToList();
    }

    private async Task<List<SearchItemVM>> SearchCommentsInternalAsync(UniversalSearchRequest request)
    {
        if (request.UserId == null) return new List<SearchItemVM>();

        var comments = await _commentRepository.GetUserCommentsAsync(request.UserId.Value);
        return comments
            .Where(c => string.IsNullOrWhiteSpace(request.Query) || 
                       c.Content.Contains(request.Query, StringComparison.OrdinalIgnoreCase))
            .Select(c => new SearchItemVM
            {
                Id = c.Id,
                EntityType = EntityType.Comment,
                Title = $"Comment on {c.EntityType}",
                Description = c.Content.Length > 200 ? c.Content.Substring(0, 200) + "..." : c.Content,
                Url = GetEntityUrl(c.EntityId, c.EntityType) + $"#comment-{c.Id}",
                CreatedAt = c.CreatedAt,
                AuthorId = c.AuthorId,
                AuthorName = GetUserName(c.AuthorId)
            })
            .ToList();
    }

    private async Task<List<SearchItemVM>> SearchCategoriesInternalAsync(UniversalSearchRequest request)
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories
            .Where(c => string.IsNullOrWhiteSpace(request.Query) || 
                       c.Name.Contains(request.Query, StringComparison.OrdinalIgnoreCase) ||
                       (c.Description != null && c.Description.Contains(request.Query, StringComparison.OrdinalIgnoreCase)))
            .Select(c => new SearchItemVM
            {
                Id = c.Id,
                EntityType = EntityType.Category,
                Title = c.Name,
                Description = c.Description ?? "No description",
                Url = $"/categories/{c.Slug}",
                CreatedAt = c.CreatedAt,
                Category = c.ParentCategoryId.HasValue ? GetCategoryName(c.ParentCategoryId) : "Root"
            })
            .ToList();
    }

    private async Task<List<SearchItemVM>> SearchTagsInternalAsync(UniversalSearchRequest request)
    {
        var tags = string.IsNullOrWhiteSpace(request.Query)
            ? await _tagRepository.GetPopularTagsAsync(50)
            : await _tagRepository.SearchTagsAsync(request.Query, 50);

        return tags.Select(t => new SearchItemVM
        {
            Id = t.Id,
            EntityType = EntityType.Tag,
            Title = t.Name,
            Description = $"Used {t.UsageCount} times",
            Url = $"/tags/{t.Slug}",
            CreatedAt = t.CreatedAt,
            Tags = new List<string> { t.Name }
        }).ToList();
    }

    private async Task<List<SearchItemVM>> SearchReactionsInternalAsync(UniversalSearchRequest request)
    {
        if (request.UserId == null) return new List<SearchItemVM>();

        var reactions = await _reactionRepository.GetUserReactionsAsync(request.UserId.Value);
        return reactions.Select(r => new SearchItemVM
        {
            Id = r.Id,
            EntityType = EntityType.Reaction,
            Title = $"{r.Type} reaction",
            Description = $"Reacted with {r.Type} to {r.EntityType}",
            Url = GetEntityUrl(r.EntityId, r.EntityType),
            CreatedAt = r.CreatedAt,
            AuthorId = r.UserId,
            AuthorName = GetUserName(r.UserId)
        }).ToList();
    }

    private async Task<List<SearchItemVM>> SearchSharesInternalAsync(UniversalSearchRequest request)
    {
        if (request.UserId == null) return new List<SearchItemVM>();

        var shares = await _shareRepository.GetUserSharesAsync(request.UserId.Value);
        return shares
            .Where(s => string.IsNullOrWhiteSpace(request.Query) || 
                       (s.ShareMessage != null && s.ShareMessage.Contains(request.Query, StringComparison.OrdinalIgnoreCase)))
            .Select(s => new SearchItemVM
            {
                Id = s.Id,
                EntityType = EntityType.Share,
                Title = $"Shared {s.EntityType}",
                Description = s.ShareMessage ?? $"Shared via {s.Platform}",
                Url = GetEntityUrl(s.EntityId, s.EntityType),
                CreatedAt = s.CreatedAt,
                AuthorId = s.UserId,
                AuthorName = GetUserName(s.UserId)
            })
            .ToList();
    }

    private async Task<List<SearchItemVM>> SearchRatingsInternalAsync(UniversalSearchRequest request)
    {
        // This would need a method to get user ratings
        return new List<SearchItemVM>();
    }

    private async Task<List<SearchItemVM>> SearchVotesInternalAsync(UniversalSearchRequest request)
    {
        if (request.UserId == null) return new List<SearchItemVM>();

        var votes = await _voteRepository.GetUserVotesAsync(request.UserId.Value);
        return votes.Select(v => new SearchItemVM
        {
            Id = v.Id,
            EntityType = EntityType.Vote,
            Title = $"{v.Type} vote",
            Description = $"Voted {v.Type} on {v.EntityType}",
            Url = GetEntityUrl(v.EntityId, v.EntityType),
            CreatedAt = v.CreatedAt,
            AuthorId = v.UserId,
            AuthorName = GetUserName(v.UserId)
        }).ToList();
    }

    private async Task<List<SearchItemVM>> SearchViewsInternalAsync(UniversalSearchRequest request)
    {
        // Views are typically not searched by users
        return new List<SearchItemVM>();
    }

    private List<SearchItemVM> CalculateRelevanceScores(List<SearchItemVM> items, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            foreach (var item in items)
            {
                item.RelevanceScore = 1.0;
            }
            return items;
        }

        foreach (var item in items)
        {
            var score = 0.0;

            // Title match (highest weight)
            if (item.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
            {
                score += 10.0;
                if (item.Title.StartsWith(query, StringComparison.OrdinalIgnoreCase))
                    score += 5.0;
            }

            // Description match
            if (item.Description.Contains(query, StringComparison.OrdinalIgnoreCase))
                score += 5.0;

            // Tag match
            if (item.Tags.Any(t => t.Contains(query, StringComparison.OrdinalIgnoreCase)))
                score += 3.0;

            // Category match
            if (item.Category.Contains(query, StringComparison.OrdinalIgnoreCase))
                score += 2.0;

            // Recency boost (newer content gets slight boost)
            var daysSinceCreation = (DateTime.UtcNow - item.CreatedAt).TotalDays;
            if (daysSinceCreation < 7)
                score += 1.0;
            else if (daysSinceCreation < 30)
                score += 0.5;

            item.RelevanceScore = score;
        }

        return items;
    }

    private List<SearchFacetVM> GenerateSearchFacets(List<SearchItemVM> items, UniversalSearchRequest request)
    {
        var facets = new List<SearchFacetVM>();

        // Entity Type facet
        var entityTypeFacet = new SearchFacetVM
        {
            Name = "EntityType",
            DisplayName = "Content Type",
            Values = items.GroupBy(i => i.EntityType)
                .Select(g => new SearchFacetValueVM
                {
                    Value = g.Key.ToString(),
                    DisplayValue = g.Key.ToString(),
                    Count = g.Count(),
                    IsSelected = request.EntityTypes?.Contains(g.Key) == true
                })
                .OrderByDescending(v => v.Count)
                .ToList()
        };
        facets.Add(entityTypeFacet);

        // Date facet
        var dateFacet = new SearchFacetVM
        {
            Name = "DateRange",
            DisplayName = "Date Range",
            Values = new List<SearchFacetValueVM>
            {
                new() { Value = "today", DisplayValue = "Today", Count = items.Count(i => i.CreatedAt.Date == DateTime.Today) },
                new() { Value = "week", DisplayValue = "This Week", Count = items.Count(i => i.CreatedAt >= DateTime.Today.AddDays(-7)) },
                new() { Value = "month", DisplayValue = "This Month", Count = items.Count(i => i.CreatedAt >= DateTime.Today.AddDays(-30)) },
                new() { Value = "year", DisplayValue = "This Year", Count = items.Count(i => i.CreatedAt >= DateTime.Today.AddDays(-365)) }
            }
        };
        facets.Add(dateFacet);

        return facets;
    }

    // Helper methods for getting entity information
    private string GetEntityTitle(Guid entityId, EntityType entityType)
    {
        // This would typically query the appropriate repository
        return $"{entityType} {entityId}";
    }

    private string GetEntityUrl(Guid entityId, EntityType entityType)
    {
        return entityType switch
        {
            EntityType.Post => $"/posts/{entityId}",
            EntityType.Question => $"/qa/{entityId}",
            EntityType.Answer => $"/qa/{entityId}",
            EntityType.Story => $"/feed#story-{entityId}",
            EntityType.Review => $"/reviews/{entityId}",
            EntityType.Event => $"/events/{entityId}",
            EntityType.Guide => $"/guides/{entityId}",
            EntityType.Group => $"/groups/{entityId}",
            EntityType.News => $"/news/{entityId}",
            EntityType.Category => $"/categories/{entityId}",
            EntityType.Tag => $"/tags/{entityId}",
            _ => $"/{entityType.ToString().ToLower()}/{entityId}"
        };
    }

    private string GetUserName(Guid userId)
    {
        // This would typically query the user repository
        return $"User {userId}";
    }

    private string GetCategoryName(Guid? categoryId)
    {
        if (!categoryId.HasValue) return string.Empty;
        // This would typically query the category repository
        return $"Category {categoryId}";
    }

    private int GetCategoryItemCount(Guid categoryId)
    {
        // This would typically count items in this category
        return 0;
    }

    private double CalculateTagSuggestionScore(string tagName, string query, int usageCount)
    {
        var score = 0.0;
        
        if (tagName.StartsWith(query, StringComparison.OrdinalIgnoreCase))
            score += 10.0;
        else if (tagName.Contains(query, StringComparison.OrdinalIgnoreCase))
            score += 5.0;

        // Usage count boost
        score += Math.Log10(usageCount + 1);

        return score;
    }

    private double CalculateCategorySuggestionScore(string categoryName, string query)
    {
        var score = 0.0;
        
        if (categoryName.StartsWith(query, StringComparison.OrdinalIgnoreCase))
            score += 10.0;
        else if (categoryName.Contains(query, StringComparison.OrdinalIgnoreCase))
            score += 5.0;

        return score;
    }

    private double CalculateTrendingScore(int interactionCount, DateTime createdAt)
    {
        var daysSinceCreation = (DateTime.UtcNow - createdAt).TotalDays;
        var recencyFactor = Math.Max(0.1, 1.0 - (daysSinceCreation / 30.0)); // Decay over 30 days
        return interactionCount * recencyFactor;
    }
}


