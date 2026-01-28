using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Domain.Entities.Community.Maps;

public class RouteOption : BaseEntity
{
    public Guid RouteRequestId { get; set; }
    public RouteType RouteType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double DistanceKm { get; set; }
    public int EstimatedTimeMinutes { get; set; }
    public decimal EstimatedCost { get; set; }
    public decimal FuelCost { get; set; }
    public decimal TollCost { get; set; }
    public int TrafficDelayMinutes { get; set; }
    public bool HasTolls { get; set; }
    public bool IsEcoFriendly { get; set; }
    public double CarbonFootprintKg { get; set; }
    public string RouteGeometry { get; set; } = string.Empty; // GeoJSON
    public List<string> Instructions { get; set; } = new();
    public List<string> Waypoints { get; set; } = new();
    public DateTime CalculatedAt { get; set; }
    public bool IsRecommended { get; set; }
}

public class RouteRequest : BaseEntity
{
    public Guid UserId { get; set; }
    public string StartLocation { get; set; } = string.Empty;
    public string EndLocation { get; set; } = string.Empty;
    public double StartLatitude { get; set; }
    public double StartLongitude { get; set; }
    public double EndLatitude { get; set; }
    public double EndLongitude { get; set; }
    public List<string> Waypoints { get; set; } = new();
    public string VehicleType { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
    public bool AvoidTolls { get; set; }
    public bool AvoidHighways { get; set; }
    public bool PreferEcoRoutes { get; set; }
    public DateTime RequestedAt { get; set; }
    public RouteRequestStatus Status { get; set; }
    
    // Navigation property
    public List<RouteOption> RouteOptions { get; set; } = new();
}