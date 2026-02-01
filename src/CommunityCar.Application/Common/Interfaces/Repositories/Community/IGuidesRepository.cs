using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Community.Guides;
using CommunityCar.Application.Features.Guides.ViewModels;
using CommunityCar.Domain.Entities.Community.Stories; // This was accidentally added too? No, wait. 
using CommunityCar.Domain.Entities.Community.News;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface IGuidesRepository : IBaseRepository<Guide>
{
    Task<Guide?> GetBySlugAsync(string slug);
    Task<int> GetCountByUserAndDateAsync(Guid userId, DateTime date);
    Task<Guide?> GetGuideWithAuthorAsync(Guid id);
    Task<IEnumerable<Guide>> GetRelatedGuidesAsync(Guid guideId, int count);
    Task<IEnumerable<Guide>> GetGuidesByAuthorAsync(Guid authorId, int count);
    Task<IEnumerable<Guide>> GetGuidesAsync(GuideFilterVM filter);
    Task<int> GetTotalCountAsync(GuideFilterVM filter);
    Task<IEnumerable<string>> GetCategoriesAsync();
    Task<IEnumerable<string>> GetPopularTagsAsync(int count);
    Task<IEnumerable<Guide>> GetFeaturedGuidesAsync(int count);
    Task<IEnumerable<Guide>> GetVerifiedGuidesAsync(int count);
    Task<IEnumerable<Guide>> GetPopularGuidesAsync(int count);
    Task<IEnumerable<Guide>> GetRecentGuidesAsync(int count);
    Task<IEnumerable<Guide>> SearchGuidesAsync(string searchTerm, int count);
    Task<Guide> CreateGuideAsync(Guide guide);
    Task<Guide?> GetGuideByIdAsync(Guid id);
    Task UpdateGuideAsync(Guide guide);
    Task DeleteGuideAsync(Guid id);
    Task IncrementViewCountAsync(Guid guideId);
    Task<bool> IsBookmarkedByUserAsync(Guid guideId, Guid userId);
    Task<double?> GetUserRatingAsync(Guid guideId, Guid userId);
}
