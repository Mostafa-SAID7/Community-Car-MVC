using CommunityCar.Application.Features.Community.News.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Community.News;

public interface INewsService
{
    Task<NewsSearchResponse> SearchNewsAsync(NewsSearchVM request);
    Task<NewsItemVM?> GetByIdAsync(Guid id, Guid? currentUserId = null);
    Task<NewsItemVM?> GetBySlugAsync(string slug);
    Task<Guid> CreateAsync(NewsCreateVM model, Guid authorId);
    Task UpdateAsync(Guid id, NewsEditVM model, Guid currentUserId);
    Task DeleteAsync(Guid id, Guid currentUserId);
    Task PublishAsync(Guid id, Guid currentUserId);
    Task UnpublishAsync(Guid id, Guid currentUserId);
    Task SetFeaturedAsync(Guid id, bool featured);
    Task SetPinnedAsync(Guid id, bool pinned);
    Task IncrementViewCountAsync(Guid id);
    Task LikeAsync(Guid id, Guid userId);
    Task UnlikeAsync(Guid id, Guid userId);
    Task BookmarkAsync(Guid id, Guid userId);
    Task UnbookmarkAsync(Guid id, Guid userId);
    Task CommentAsync(Guid id, Guid userId);
    Task ShareAsync(Guid id, Guid userId);
    Task<int> GetLikeCountAsync(Guid id);
    Task<IEnumerable<string>> GetPopularTagsAsync(int count = 20);
    Task<IEnumerable<string>> GetAvailableCarMakesAsync();
    Task<NewsStatsVM> GetNewsStatsAsync();
}


