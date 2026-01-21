using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Application.Features.Maps.DTOs;
using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Community;

public class RouteRepository : BaseRepository<Route>, IRouteRepository
{
    public RouteRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Route> Items, int TotalCount)> SearchAsync(
        MapsRouteSearchRequest request, 
        CancellationToken cancellationToken = default)
    {
        var query = Context.Routes.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(r => 
                r.Name.ToLower().Contains(searchTerm) ||
                r.Description.ToLower().Contains(searchTerm));
        }

        if (request.Type.HasValue)
        {
            query = query.Where(r => r.Type == request.Type.Value);
        }

        if (request.Difficulty.HasValue)
        {
            query = query.Where(r => r.Difficulty == request.Difficulty.Value);
        }

        if (request.MinDistance.HasValue)
        {
            query = query.Where(r => r.DistanceKm >= request.MinDistance.Value);
        }

        if (request.MaxDistance.HasValue)
        {
            query = query.Where(r => r.DistanceKm <= request.MaxDistance.Value);
        }

        if (request.MinDuration.HasValue)
        {
            query = query.Where(r => r.EstimatedDurationMinutes >= request.MinDuration.Value);
        }

        if (request.MaxDuration.HasValue)
        {
            query = query.Where(r => r.EstimatedDurationMinutes <= request.MaxDuration.Value);
        }

        if (request.MinRating.HasValue)
        {
            query = query.Where(r => r.AverageRating >= request.MinRating.Value);
        }

        if (request.IsScenic.HasValue)
        {
            query = query.Where(r => r.IsScenic == request.IsScenic.Value);
        }

        if (request.HasTolls.HasValue)
        {
            query = query.Where(r => r.HasTolls == request.HasTolls.Value);
        }

        if (request.IsOffRoad.HasValue)
        {
            query = query.Where(r => r.IsOffRoad == request.IsOffRoad.Value);
        }

        if (request.Tags?.Any() == true)
        {
            foreach (var tag in request.Tags)
            {
                query = query.Where(r => r.Tags.Contains(tag.ToLowerInvariant()));
            }
        }

        // Location-based filtering (using first waypoint as reference)
        if (request.Latitude.HasValue && request.Longitude.HasValue)
        {
            var lat = request.Latitude.Value;
            var lng = request.Longitude.Value;
            var radius = request.RadiusKm;

            // This is a simplified approach - in a real application, you might want to check all waypoints
            query = query.Where(r => r.Waypoints.Any(w =>
                Math.Sqrt(
                    Math.Pow(69.1 * (w.Latitude - lat), 2) +
                    Math.Pow(69.1 * (lng - w.Longitude) * Math.Cos(lat / 57.3), 2)
                ) <= radius));
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = request.SortBy?.ToLower() switch
        {
            "name" => request.SortDescending ? query.OrderByDescending(r => r.Name) : query.OrderBy(r => r.Name),
            "rating" => request.SortDescending ? query.OrderByDescending(r => r.AverageRating) : query.OrderBy(r => r.AverageRating),
            "created" => request.SortDescending ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt),
            "popularity" => request.SortDescending ? query.OrderByDescending(r => r.TimesCompleted) : query.OrderBy(r => r.TimesCompleted),
            "distance" => request.SortDescending ? query.OrderByDescending(r => r.DistanceKm) : query.OrderBy(r => r.DistanceKm),
            _ => query.OrderBy(r => r.Name)
        };

        // Apply pagination
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IEnumerable<Route>> GetNearbyAsync(
        double latitude, 
        double longitude, 
        double radiusKm, 
        CancellationToken cancellationToken = default)
    {
        // This is a simplified approach - checking if any waypoint is within radius
        var routes = await Context.Routes
            .Where(r => r.Waypoints.Any(w =>
                Math.Sqrt(
                    Math.Pow(69.1 * (w.Latitude - latitude), 2) +
                    Math.Pow(69.1 * (longitude - w.Longitude) * Math.Cos(latitude / 57.3), 2)
                ) <= radiusKm))
            .ToListAsync(cancellationToken);

        return routes;
    }

    public async Task<IEnumerable<Route>> GetByTypeAsync(
        RouteType type, 
        CancellationToken cancellationToken = default)
    {
        return await Context.Routes
            .Where(r => r.Type == type)
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Route>> GetByDifficultyAsync(
        DifficultyLevel difficulty, 
        CancellationToken cancellationToken = default)
    {
        return await Context.Routes
            .Where(r => r.Difficulty == difficulty)
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Route>> GetByCreatorAsync(
        Guid createdBy, 
        CancellationToken cancellationToken = default)
    {
        return await Context.Routes
            .Where(r => r.CreatedBy == createdBy)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Route>> GetPopularAsync(
        int count = 10, 
        CancellationToken cancellationToken = default)
    {
        return await Context.Routes
            .OrderByDescending(r => r.TimesCompleted)
            .ThenByDescending(r => r.AverageRating)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Route>> GetByTagAsync(
        string tag, 
        CancellationToken cancellationToken = default)
    {
        return await Context.Routes
            .Where(r => r.Tags.Contains(tag.ToLowerInvariant()))
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }
}
