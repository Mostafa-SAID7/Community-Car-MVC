using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.Maps.ViewModels;

// Route Engine ViewModels (converted from DTOs)
public class RouteRequestVM
{
    public string StartLocation { get; set; } = string.Empty;
    public double StartLatitude { get; set; }
    public double StartLongitude { get; set; }
    public string EndLocation { get; set; } = string.Empty;
    public double EndLatitude { get; set; }
    public double EndLongitude { get; set; }
    public List<WaypointVM> Waypoints { get; set; } = new();
    public string VehicleType { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
    public bool AvoidTolls { get; set; }
    public bool AvoidHighways { get; set; }
    public bool PreferEcoRoutes { get; set; }
    public DateTime? DepartureTime { get; set; }
    public List<RouteType> PreferredRouteTypes { get; set; } = new();
}