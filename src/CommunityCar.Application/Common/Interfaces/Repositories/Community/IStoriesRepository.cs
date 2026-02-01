using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Community.Stories;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface IStoriesRepository : IBaseRepository<Story>
{
    Task<Story?> GetBySlugAsync(string slug);
    Task<IEnumerable<Story>> GetActiveAsync();
    Task<IEnumerable<Story>> GetExpiredAsync();
    Task<IEnumerable<Story>> GetByAuthorAsync(Guid authorId);
    Task DeleteExpiredAsync();
    Task<IEnumerable<string>> GetPopularTagsAsync(int count);
    Task<IEnumerable<string>> GetAvailableCarMakesAsync();
    Task<IEnumerable<Story>> GetTopActiveAsync(int count);
}
