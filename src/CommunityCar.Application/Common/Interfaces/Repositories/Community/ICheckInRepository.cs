using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Community.Maps;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface ICheckInRepository : IBaseRepository<CheckIn>
{
    Task<IEnumerable<CheckIn>> GetByPointOfInterestAsync(
        Guid pointOfInterestId, 
        int page = 1, 
        int pageSize = 20, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<CheckIn>> GetByUserAsync(
        Guid userId, 
        int page = 1, 
        int pageSize = 20, 
        CancellationToken cancellationToken = default);
    
    Task<CheckIn?> GetUserCheckInForPOIAsync(
        Guid userId, 
        Guid pointOfInterestId, 
        CancellationToken cancellationToken = default);
    
    Task<int> GetCheckInCountForPOIAsync(
        Guid pointOfInterestId, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<CheckIn>> GetRecentCheckInsAsync(
        int count = 10, 
        CancellationToken cancellationToken = default);
}