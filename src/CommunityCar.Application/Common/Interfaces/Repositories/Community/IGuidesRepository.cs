using CommunityCar.Domain.Entities.Community.Guides;
using CommunityCar.Application.Features.Guides.DTOs;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface IGuidesRepository
{
    Task<Guide?> GetGuideByIdAsync(Guid id);
    Task<Guide?> GetGuideWithAuthorAsync(Guid id);
    Task<IEnumerable<Guide>> GetGuidesAsync(GuideFilterDTO filter);
    Task<IEnumerable<Guide>> GetFeaturedGuidesAsync(int count = 10);
    Task<IEnumerable<Guide>> GetVerifiedGuidesAsync(int count = 10);
    Task<IEnumerable<Guide>> GetPopularGuidesAsync(int count = 10);
    Task<IEnumerable<Guide>> GetRecentGuidesAsync(int count = 10);
    Task<IEnumerable<Guide>> GetGuidesByAuthorAsync(Guid authorId, int count = 10);
    Task<IEnumerable<Guide>> GetRelatedGuidesAsync(Guid guideId, int count = 5);
    Task<IEnumerable<Guide>> SearchGuidesAsync(string searchTerm, int count = 20);
    Task<Guide> CreateGuideAsync(Guide guide);
    Task<Guide> UpdateGuideAsync(Guide guide);
    Task DeleteGuideAsync(Guid id);
    Task<int> GetTotalCountAsync(GuideFilterDTO filter);
    Task<IEnumerable<string>> GetCategoriesAsync();
    Task<IEnumerable<string>> GetPopularTagsAsync(int count = 20);
    Task<bool> IsBookmarkedByUserAsync(Guid guideId, Guid userId);
    Task<double?> GetUserRatingAsync(Guid guideId, Guid userId);
    Task IncrementViewCountAsync(Guid guideId);
    Task<int> GetCountByUserAndDateAsync(Guid userId, DateTime date);
}


