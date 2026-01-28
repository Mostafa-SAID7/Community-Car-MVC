using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Application.Features.Maps.DTOs;
using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Community;

public class PointOfInterestRepository : BaseRepository<PointOfInterest>, IPointOfInterestRepository
{
    public PointOfInterestRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<PointOfInterest> Items, int TotalCount)> SearchAsync(
        MapsSearchRequest request, 
        CancellationToken cancellationToken = default)
    {
        var query = Context.PointsOfInterest.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(p => 
                p.Name.ToLower().Contains(searchTerm) ||
                p.Description.ToLower().Contains(searchTerm) ||
                (p.Address != null && p.Address.ToLower().Contains(searchTerm)));
        }

        if (request.Type.HasValue)
        {
            query = query.Where(p => p.Type == request.Type.Value);
        }

        if (request.Category.HasValue)
        {
            query = query.Where(p => p.Category == request.Category.Value);
        }

        if (request.IsVerified.HasValue)
        {
            query = query.Where(p => p.IsVerified == request.IsVerified.Value);
        }

        if (request.IsOpen24Hours.HasValue)
        {
            query = query.Where(p => p.IsOpen24Hours == request.IsOpen24Hours.Value);
        }

        if (request.MinRating.HasValue)
        {
            query = query.Where(p => p.AverageRating >= request.MinRating.Value);
        }

        if (request.Services?.Any() == true)
        {
            foreach (var service in request.Services)
            {
                query = query.Where(p => p.Services.Contains(service));
            }
        }

        if (request.PaymentMethods?.Any() == true)
        {
            foreach (var method in request.PaymentMethods)
            {
                query = query.Where(p => p.PaymentMethods.Contains(method));
            }
        }

        // Location-based filtering
        if (request.Latitude.HasValue && request.Longitude.HasValue)
        {
            var lat = request.Latitude.Value;
            var lng = request.Longitude.Value;
            var radius = request.RadiusKm;

            // Using Haversine formula approximation for distance filtering
            query = query.Where(p => 
                Math.Sqrt(
                    Math.Pow(69.1 * (p.Latitude - lat), 2) +
                    Math.Pow(69.1 * (lng - p.Longitude) * Math.Cos(lat / 57.3), 2)
                ) <= radius);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = request.SortBy?.ToLower() switch
        {
            "name" => request.SortDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            "rating" => request.SortDescending ? query.OrderByDescending(p => p.AverageRating) : query.OrderBy(p => p.AverageRating),
            "created" => request.SortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
            "distance" when request.Latitude.HasValue && request.Longitude.HasValue => 
                request.SortDescending ? 
                    query.OrderByDescending(p => Math.Sqrt(Math.Pow(69.1 * (p.Latitude - request.Latitude.Value), 2) + Math.Pow(69.1 * (request.Longitude.Value - p.Longitude) * Math.Cos(request.Latitude.Value / 57.3), 2))) :
                    query.OrderBy(p => Math.Sqrt(Math.Pow(69.1 * (p.Latitude - request.Latitude.Value), 2) + Math.Pow(69.1 * (request.Longitude.Value - p.Longitude) * Math.Cos(request.Latitude.Value / 57.3), 2))),
            _ => query.OrderBy(p => p.Name)
        };

        // Apply pagination
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IEnumerable<PointOfInterest>> GetNearbyAsync(
        double latitude, 
        double longitude, 
        double radiusKm, 
        POIType? type = null, 
        CancellationToken cancellationToken = default)
    {
        var query = Context.PointsOfInterest.AsQueryable();

        if (type.HasValue)
        {
            query = query.Where(p => p.Type == type.Value);
        }

        // Using Haversine formula approximation for distance filtering
        query = query.Where(p => 
            Math.Sqrt(
                Math.Pow(69.1 * (p.Latitude - latitude), 2) +
                Math.Pow(69.1 * (longitude - p.Longitude) * Math.Cos(latitude / 57.3), 2)
            ) <= radiusKm);

        // Order by distance
        query = query.OrderBy(p => 
            Math.Sqrt(
                Math.Pow(69.1 * (p.Latitude - latitude), 2) +
                Math.Pow(69.1 * (longitude - p.Longitude) * Math.Cos(latitude / 57.3), 2)
            ));

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PointOfInterest>> GetByTypeAsync(
        POIType type, 
        CancellationToken cancellationToken = default)
    {
        return await Context.PointsOfInterest
            .Where(p => p.Type == type)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PointOfInterest>> GetByCategoryAsync(
        POICategory category, 
        CancellationToken cancellationToken = default)
    {
        return await Context.PointsOfInterest
            .Where(p => p.Category == category)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PointOfInterest>> GetVerifiedAsync(
        CancellationToken cancellationToken = default)
    {
        return await Context.PointsOfInterest
            .Where(p => p.IsVerified)
            .OrderByDescending(p => p.VerifiedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PointOfInterest>> GetReportedAsync(
        CancellationToken cancellationToken = default)
    {
        return await Context.PointsOfInterest
            .Where(p => p.IsReported)
            .OrderByDescending(p => p.ReportCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PointOfInterest>> GetByCreatorAsync(
        Guid createdBy, 
        CancellationToken cancellationToken = default)
    {
        return await Context.PointsOfInterest
            .Where(p => p.CreatedBy == createdBy)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}

