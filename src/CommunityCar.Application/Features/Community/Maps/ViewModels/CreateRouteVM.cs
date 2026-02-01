using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.Maps.ViewModels;

public class CreateRouteVM
{
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public RouteType Type { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public double DistanceKm { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public bool IsScenic { get; set; }
    public bool HasTolls { get; set; }
    public bool IsOffRoad { get; set; }
    public string? SurfaceType { get; set; }
    public string? SurfaceTypeAr { get; set; }
    public string? BestTimeToVisit { get; set; }
    public string? BestTimeToVisitAr { get; set; }
    public string? SafetyNotes { get; set; }
    public string? SafetyNotesAr { get; set; }
    public List<RouteWaypointVM> Waypoints { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
}