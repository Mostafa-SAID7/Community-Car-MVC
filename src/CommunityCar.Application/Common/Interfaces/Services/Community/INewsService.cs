using CommunityCar.Application.Features.News.DTOs;
using CommunityCar.Application.Features.News.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Community;

public interface INewsService
{
    Task<NewsSearchResponse> SearchNewsAsync(NewsSearchRequest request);
    Task<NewsItemVM?> GetByIdAsync(Guid id);
    Task<NewsItemVM?> GetBySlugAsync(string slug);
    Task<NewsItemVM> CreateAsync(CreateNewsRequest request);
    Task<NewsItemVM> UpdateAsync(Guid id, UpdateNewsRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> PublishAsync(Guid id);
    Task<bool> UnpublishAsync(Guid id);
    Task<bool> SetFeaturedAsync(Guid id, bool featured);
    Task<bool> SetPinnedAsync(Guid id, bool pinned);
    Task<bool> IncrementViewCountAsync(Guid id);
    Task<IEnumerable<string>> GetPopularTagsAsync(int count = 20);
    Task<IEnumerable<string>> GetAvailableCarMakesAsync();
    Task<NewsStatsVM> GetNewsStatsAsync();
}