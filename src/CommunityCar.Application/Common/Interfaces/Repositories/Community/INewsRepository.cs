using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Community.News;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface INewsRepository : IBaseRepository<NewsItem>
{
    Task<int> GetCountByUserAndDateAsync(Guid userId, DateTime date);
    Task<IEnumerable<string>> GetPopularTagsAsync(int count);
    Task<IEnumerable<string>> GetAvailableCarMakesAsync();
    Task<NewsItem?> GetBySlugAsync(string slug);
    Task<IEnumerable<NewsItem>> GetPublishedAsync();
    Task<IEnumerable<NewsItem>> GetTopPublishedAsync(int count);
}
