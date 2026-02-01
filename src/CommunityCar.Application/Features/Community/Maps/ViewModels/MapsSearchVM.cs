using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.Maps.ViewModels;

public class MapsSearchVM
{
    // Search parameters
    public string? SearchTerm { get; set; }
    public string? Query { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? RadiusKm { get; set; }
    public POIType? Type { get; set; }
    public POICategory? Category { get; set; }
    public List<string> Services { get; set; } = new();
    public bool? IsVerified { get; set; }
    public bool? IsOpen { get; set; }
    public bool? IsOpen24Hours { get; set; }
    public double? MinRating { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "Distance";
    public string SortOrder { get; set; } = "asc";
    
    // Results
    public List<PointOfInterestSummaryVM> Items { get; set; } = new();
    public List<PointOfInterestSummaryVM> Results { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}