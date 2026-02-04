using CommunityCar.Application.Features.Community.Reviews.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Community.Reviews;

public interface IReviewsService
{
    Task<ReviewsSearchVM> SearchReviewsAsync(ReviewsSearchVM request);
    Task<ReviewVM?> GetByIdAsync(Guid id);
    Task<ReviewVM> CreateAsync(CreateReviewVM request);
    Task<ReviewVM> UpdateAsync(Guid id, UpdateReviewVM request);
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


