using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Maps.DTOs;

public class MapsRouteSearchRequest
{
    public string? SearchTerm { get; set; }
    public RouteType? Type { get; set; }
    public DifficultyLevel? Difficulty { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double RadiusKm { get; set; } = 50;
    public double? MinDistance { get; set; }
    public double? MaxDistance { get; set; }
    public int? MinDuration { get; set; }
    public int? MaxDuration { get; set; }
    public double? MinRating { get; set; }
    public bool? IsScenic { get; set; }
    public bool? HasTolls { get; set; }
    public bool? IsOffRoad { get; set; }
    public List<string>? Tags { get; set; }
    public string? SortBy { get; set; } = "distance"; // distance, rating, name, created, popularity
    public bool SortDescending { get; set; } = false;
    
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;
    
    [Range(1, 100)]
    public int PageSize { get; set; } = 20;
}

public class MapsRouteSearchResponse
{
    public IEnumerable<RouteSummaryVM> Items { get; set; } = new List<RouteSummaryVM>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

public class RouteSummaryVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public RouteType Type { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public double DistanceKm { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int TimesCompleted { get; set; }
    public bool IsScenic { get; set; }
    public bool HasTolls { get; set; }
    public bool IsOffRoad { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
}


