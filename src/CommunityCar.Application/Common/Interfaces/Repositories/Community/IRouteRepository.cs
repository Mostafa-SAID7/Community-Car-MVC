using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Application.Features.Maps.DTOs;
using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface IRouteRepository : IBaseRepository<Route>
{
    Task<(IEnumerable<Route> Items, int TotalCount)> SearchAsync(
        MapsRouteSearchRequest request, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Route>> GetNearbyAsync(
        double latitude, 
        double longitude, 
        double radiusKm, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Route>> GetByTypeAsync(
        RouteType type, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Route>> GetByDifficultyAsync(
        DifficultyLevel difficulty, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Route>> GetByCreatorAsync(
        Guid createdBy, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Route>> GetPopularAsync(
        int count = 10, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Route>> GetByTagAsync(
        string tag, 
        CancellationToken cancellationToken = default);
}