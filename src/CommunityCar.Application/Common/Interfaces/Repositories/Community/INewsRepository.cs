using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Community.News;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface INewsRepository : IBaseRepository<NewsItem>
{
    Task<NewsItem?> GetBySlugAsync(string slug);
    Task<IEnumerable<NewsItem>> GetPublishedAsync();
    Task<IEnumerable<NewsItem>> GetFeaturedAsync();
    Task<IEnumerable<NewsItem>> GetPinnedAsync();
    Task<IEnumerable<NewsItem>> GetByAuthorAsync(Guid authorId);
    Task<IEnumerable<NewsItem>> GetByCategoryAsync(NewsCategory category);
    Task<IEnumerable<NewsItem>> GetByCarMakeAsync(string carMake);
    Task<IEnumerable<NewsItem>> GetByTagAsync(string tag);
    Task<IEnumerable<string>> GetPopularTagsAsync(int count);
    Task<IEnumerable<string>> GetAvailableCarMakesAsync();
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null);
    Task<int> GetCountByUserAndDateAsync(Guid userId, DateTime date);
}


