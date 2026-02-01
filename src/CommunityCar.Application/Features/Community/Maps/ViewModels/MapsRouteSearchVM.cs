using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.Maps.ViewModels;

public class MapsRouteSearchVM
{
    public string? Query { get; set; }
    public string? SearchTerm { get; set; }
    public RouteType? Type { get; set; }
    public DifficultyLevel? Difficulty { get; set; }
    public double? MinDistance { get; set; }
    public double? MaxDistance { get; set; }
    public int? MaxDuration { get; set; }
    public bool? IsScenic { get; set; }
    public bool? HasTolls { get; set; }
    public bool? IsOffRoad { get; set; }
    public double? MinRating { get; set; }
    public List<string> Tags { get; set; } = new();
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "Distance";
    public string SortOrder { get; set; } = "asc";
    
    // Results
    public List<RouteSummaryVM> Items { get; set; } = new();
    public List<RouteSummaryVM> Results { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}