using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Community.Stories;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface IStoriesRepository : IBaseRepository<Story>
{
    Task<IEnumerable<Story>> GetActiveAsync();
    Task<IEnumerable<Story>> GetExpiredAsync();
    Task<IEnumerable<Story>> GetByAuthorAsync(Guid authorId);
    Task<IEnumerable<Story>> GetFeaturedAsync();
    Task<IEnumerable<Story>> GetHighlightedAsync();
    Task<IEnumerable<Story>> GetByVisibilityAsync(Domain.Enums.StoryVisibility visibility);
    Task<IEnumerable<Story>> GetByCarMakeAsync(string carMake);
    Task<IEnumerable<Story>> GetByTagAsync(string tag);
    Task<IEnumerable<Story>> GetByLocationAsync(double latitude, double longitude, double radiusKm);
    Task<IEnumerable<string>> GetPopularTagsAsync(int count);
    Task<IEnumerable<string>> GetAvailableCarMakesAsync();
    Task<int> GetActiveCountByAuthorAsync(Guid authorId);
    Task<bool> DeleteExpiredAsync();
}


