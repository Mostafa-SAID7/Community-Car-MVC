using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Community.Reviews;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface IReviewsRepository : IBaseRepository<Review>
{
    Task<IEnumerable<Review>> GetByTargetAsync(Guid targetId, string targetType);
    Task<IEnumerable<Review>> GetByReviewerAsync(Guid reviewerId);
    Task<IEnumerable<Review>> GetApprovedAsync();
    Task<IEnumerable<Review>> GetFlaggedAsync();
    Task<IEnumerable<Review>> GetVerifiedPurchasesAsync();
    Task<IEnumerable<Review>> GetByCarMakeAsync(string carMake);
    Task<IEnumerable<Review>> GetByRatingAsync(int rating);
    Task<IEnumerable<Review>> GetRecentAsync(int count);
    Task<IEnumerable<string>> GetAvailableCarMakesAsync();
    Task<double> GetAverageRatingByTargetAsync(Guid targetId, string targetType);
    Task<int> GetReviewCountByTargetAsync(Guid targetId, string targetType);
    Task<bool> HasUserReviewedTargetAsync(Guid userId, Guid targetId, string targetType);
    Task<int> GetCountByUserAndDateAsync(Guid userId, DateTime date);
}


