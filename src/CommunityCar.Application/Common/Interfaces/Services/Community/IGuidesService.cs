using CommunityCar.Application.Features.Guides.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Community;

public interface IGuidesService
{
    Task<GuideDetailVM?> GetGuideAsync(Guid id, Guid? currentUserId = null);
    Task<GuideListVM> GetGuidesAsync(GuideFilterVM filter, Guid? currentUserId = null);
    Task<IEnumerable<GuideVM>> GetFeaturedGuidesAsync(int count = 10, Guid? currentUserId = null);
    Task<IEnumerable<GuideVM>> GetVerifiedGuidesAsync(int count = 10, Guid? currentUserId = null);
    Task<IEnumerable<GuideVM>> GetPopularGuidesAsync(int count = 10, Guid? currentUserId = null);
    Task<IEnumerable<GuideVM>> GetRecentGuidesAsync(int count = 10, Guid? currentUserId = null);
    Task<IEnumerable<GuideVM>> GetGuidesByAuthorAsync(Guid authorId, int count = 10, Guid? currentUserId = null);
    Task<IEnumerable<GuideVM>> SearchGuidesAsync(string searchTerm, int count = 20, Guid? currentUserId = null);
    Task<GuideResultVM> CreateGuideAsync(CreateGuideRequest dto, Guid authorId);
    Task<GuideResultVM> UpdateGuideAsync(UpdateGuideRequest dto, Guid currentUserId);
    Task<GuideResultVM> DeleteGuideAsync(Guid id, Guid currentUserId);
    Task<GuideResultVM> PublishGuideAsync(Guid id, Guid currentUserId);
    Task<GuideResultVM> UnpublishGuideAsync(Guid id, Guid currentUserId);
    Task<GuideResultVM> VerifyGuideAsync(Guid id, Guid currentUserId);
    Task<GuideResultVM> FeatureGuideAsync(Guid id, Guid currentUserId);
    Task<GuideResultVM> UnfeatureGuideAsync(Guid id, Guid currentUserId);
    Task<GuideResultVM> BookmarkGuideAsync(Guid guideId, Guid userId);
    Task<GuideResultVM> UnbookmarkGuideAsync(Guid guideId, Guid userId);
    Task<GuideResultVM> RateGuideAsync(Guid guideId, Guid userId, double rating);
    Task IncrementViewCountAsync(Guid guideId);
    Task<GuideCreateVM> GetCreateViewModelAsync();
}


