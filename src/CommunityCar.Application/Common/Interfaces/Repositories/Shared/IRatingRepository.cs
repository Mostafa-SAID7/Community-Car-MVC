using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Shared;

public interface IRatingRepository : IBaseRepository<Rating>
{
    Task<Rating?> GetUserRatingAsync(Guid entityId, EntityType entityType, Guid userId);
    Task<List<Rating>> GetEntityRatingsAsync(Guid entityId, EntityType entityType);
    Task<double> GetAverageRatingAsync(Guid entityId, EntityType entityType);
    Task<Dictionary<int, int>> GetRatingDistributionAsync(Guid entityId, EntityType entityType);
}
