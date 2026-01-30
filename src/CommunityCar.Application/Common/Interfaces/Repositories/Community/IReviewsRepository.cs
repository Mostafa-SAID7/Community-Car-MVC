using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Community.Reviews;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface IReviewsRepository : IBaseRepository<Review>
{
    Task<int> GetCountByUserAndDateAsync(Guid userId, DateTime date);
    Task<double> GetAverageRatingByTargetAsync(Guid targetId, string targetType);
    Task<IEnumerable<Review>> GetByTargetAsync(Guid targetId, string targetType);
    Task<IEnumerable<string>> GetAvailableCarMakesAsync();
    Task<IEnumerable<Review>> GetApprovedAsync();
}
