using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Shared;

public interface IRatingRepository : IBaseRepository<Rating>
{
    Task<Rating?> GetUserRatingAsync(Guid entityId, EntityType entityType, Guid userId);
    Task<List<Rating>> GetEntityRatingsAsync(Guid entityId, EntityType entityType);
    Task<List<Rating>> GetEntityRatingsAsync(Guid entityId, EntityType entityType, int page, int pageSize);
    Task<double> GetAverageRatingAsync(Guid entityId, EntityType entityType);
    Task<int> GetRatingCountAsync(Guid entityId, EntityType entityType);
    Task<Dictionary<int, int>> GetRatingDistributionAsync(Guid entityId, EntityType entityType);
    Task<IEnumerable<Rating>> GetUserRatingsAsync(Guid userId, EntityType? entityType = null);
}



