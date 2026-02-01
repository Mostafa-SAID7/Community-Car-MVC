using CommunityCar.Application.Features.Community.Reviews.DTOs;
using CommunityCar.Application.Features.Community.Reviews.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Community;

public interface IReviewsService
{
    Task<ReviewsSearchResponse> SearchReviewsAsync(ReviewsSearchRequest request);
    Task<ReviewVM?> GetByIdAsync(Guid id);
    Task<ReviewVM> CreateAsync(CreateReviewRequest request);
    Task<ReviewVM> UpdateAsync(Guid id, UpdateReviewRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ApproveAsync(Guid id);
    Task<bool> FlagAsync(Guid id);
    Task<bool> UnflagAsync(Guid id);
    Task<bool> MarkHelpfulAsync(Guid id, Guid userId, bool isHelpful);
    Task<bool> IncrementViewCountAsync(Guid id);
    Task<IEnumerable<string>> GetAvailableCarMakesAsync();
    Task<ReviewsStatsVM> GetReviewsStatsAsync();
    Task<IEnumerable<ReviewVM>> GetReviewsByTargetAsync(Guid targetId, string targetType);
    Task<double> GetAverageRatingByTargetAsync(Guid targetId, string targetType);
}


