using CommunityCar.Application.Features.Community.News.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Community.News;

public interface INewsService
{
    Task<object> GetNewsAsync();
    Task<NewsSearchVM> SearchNewsAsync(string searchTerm, string? category = null, int page = 1, int pageSize = 20);
    Task<NewsVM?> GetByIdAsync(Guid id);
    Task<bool> IncrementViewCountAsync(Guid id);
    Task<NewsVM> CreateAsync(CreateNewsVM model);
    Task<bool> UpdateAsync(Guid id, EditNewsVM model);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> PublishAsync(Guid id);
    Task<bool> UnpublishAsync(Guid id);
    Task<bool> LikeAsync(Guid id, Guid userId);
    Task<int> GetLikeCountAsync(Guid id);
    Task<bool> UnlikeAsync(Guid id, Guid userId);
    Task<bool> BookmarkAsync(Guid id, Guid userId);
    Task<bool> UnbookmarkAsync(Guid id, Guid userId);
    Task<bool> CommentAsync(Guid id, Guid userId, string comment);
    Task<bool> ShareAsync(Guid id, Guid userId, string platform);
}