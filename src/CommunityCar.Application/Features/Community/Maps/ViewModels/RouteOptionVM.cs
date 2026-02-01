using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.Maps.ViewModels;

public class RouteOptionVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public RouteType RouteType { get; set; }
    public string Description { get; set; } = string.Empty;
    public double DistanceKm { get; set; }
    public int DurationMinutes { get; set; }
    public int EstimatedTimeMinutes { get; set; }
    public int TrafficDelayMinutes { get; set; }
    public decimal EstimatedFuelCost { get; set; }
    public decimal TollCost { get; set; }
    public bool HasTolls { get; set; }
    public bool IsEcoFriendly { get; set; }
    public double CarbonFootprintKg { get; set; }
    public string RouteGeometry { get; set; } = string.Empty;
    public List<RouteInstructionVM> Instructions { get; set; } = new();
    public List<WaypointVM> Waypoints { get; set; } = new();
    public bool IsRecommended { get; set; }
    public DateTime CalculatedAt { get; set; }
    public List<TrafficConditionVM> TrafficConditions { get; set; } = new();
    public TripCostBreakdownVM CostBreakdown { get; set; } = new();
}