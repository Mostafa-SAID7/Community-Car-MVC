using CommunityCar.Application.Features.Guides.DTOs;
using CommunityCar.Application.Features.Guides.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Community;

public interface IGuidesService
{
    Task<GuideDetailVM?> GetGuideAsync(Guid id, Guid? currentUserId = null);
    Task<GuideListVM> GetGuidesAsync(GuideFilterDTO filter, Guid? currentUserId = null);
    Task<IEnumerable<GuideVM>> GetFeaturedGuidesAsync(int count = 10, Guid? currentUserId = null);
    Task<IEnumerable<GuideVM>> GetVerifiedGuidesAsync(int count = 10, Guid? currentUserId = null);
    Task<IEnumerable<GuideVM>> GetPopularGuidesAsync(int count = 10, Guid? currentUserId = null);
    Task<IEnumerable<GuideVM>> GetRecentGuidesAsync(int count = 10, Guid? currentUserId = null);
    Task<IEnumerable<GuideVM>> GetGuidesByAuthorAsync(Guid authorId, int count = 10, Guid? currentUserId = null);
    Task<IEnumerable<GuideVM>> SearchGuidesAsync(string searchTerm, int count = 20, Guid? currentUserId = null);
    Task<GuideResultDTO> CreateGuideAsync(CreateGuideDTO dto, Guid authorId);
    Task<GuideResultDTO> UpdateGuideAsync(UpdateGuideDTO dto, Guid currentUserId);
    Task<GuideResultDTO> DeleteGuideAsync(Guid id, Guid currentUserId);
    Task<GuideResultDTO> PublishGuideAsync(Guid id, Guid currentUserId);
    Task<GuideResultDTO> UnpublishGuideAsync(Guid id, Guid currentUserId);
    Task<GuideResultDTO> VerifyGuideAsync(Guid id, Guid currentUserId);
    Task<GuideResultDTO> FeatureGuideAsync(Guid id, Guid currentUserId);
    Task<GuideResultDTO> UnfeatureGuideAsync(Guid id, Guid currentUserId);
    Task<GuideResultDTO> BookmarkGuideAsync(Guid guideId, Guid userId);
    Task<GuideResultDTO> UnbookmarkGuideAsync(Guid guideId, Guid userId);
    Task<GuideResultDTO> RateGuideAsync(Guid guideId, Guid userId, double rating);
    Task IncrementViewCountAsync(Guid guideId);
    Task<GuideCreateVM> GetCreateViewModelAsync();
}


