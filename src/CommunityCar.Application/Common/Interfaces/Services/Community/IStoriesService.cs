using CommunityCar.Application.Features.Community.Stories.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Community;

public interface IStoriesService
{
    Task<StoriesSearchVM> SearchStoriesAsync(StoriesSearchVM request);
    Task<StoryVM?> GetByIdAsync(Guid id);
    Task<StoryVM?> GetBySlugAsync(string slug);
    Task<StoryVM> CreateAsync(CreateStoryVM request);
    Task<StoryVM> UpdateAsync(Guid id, UpdateStoryVM request);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ArchiveAsync(Guid id);
    Task<bool> RestoreAsync(Guid id);
    Task<bool> SetFeaturedAsync(Guid id, bool featured);
    Task<bool> SetHighlightedAsync(Guid id, bool highlighted);
    Task<bool> ExtendDurationAsync(Guid id, int additionalHours);
    Task<bool> IncrementViewCountAsync(Guid id);
    Task<bool> LikeAsync(Guid id, Guid userId);
    Task<bool> UnlikeAsync(Guid id, Guid userId);
    Task<IEnumerable<string>> GetPopularTagsAsync(int count = 20);
    Task<IEnumerable<string>> GetAvailableCarMakesAsync();
    Task<StoriesStatsVM> GetStoriesStatsAsync();
    Task<IEnumerable<StoryVM>> GetActiveStoriesAsync();
    Task<IEnumerable<StoryVM>> GetStoriesByAuthorAsync(Guid authorId);
    Task CleanupExpiredStoriesAsync();
}


