using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Community.Maps;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface ICheckInRepository : IBaseRepository<CheckIn>
{
    Task<IEnumerable<CheckIn>> GetByPointOfInterestAsync(Guid pointOfInterestId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<CheckIn>> GetByUserAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<CheckIn>> GetRecentCheckInsAsync(int count, CancellationToken cancellationToken = default);
}
