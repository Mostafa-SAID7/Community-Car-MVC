using CommunityCar.Application.Features.Maps.DTOs;
using CommunityCar.Application.Services.Maps.Pricing;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Services.Maps.Routing;

public interface IRouteEngine
{
    Task<List<RouteOptionDto>> GetRouteOptionsAsync(RouteRequestDto request);
    Task<RouteOptionDto> GetOptimalRouteAsync(RouteRequestDto request, RouteType routeType);
    Task<List<RouteOptionDto>> GetAlternativeRoutesAsync(RouteRequestDto request);
    Task<RouteOptionDto> RecalculateRouteAsync(Guid routeId, TrafficConditionDto[] trafficConditions);
}

public class RouteRequestDto
{
    public string StartLocation { get; set; } = string.Empty;
    public string EndLocation { get; set; } = string.Empty;
    public double StartLatitude { get; set; }
    public double StartLongitude { get; set; }
    public double EndLatitude { get; set; }
    public double EndLongitude { get; set; }
    public List<WaypointDto> Waypoints { get; set; } = new();
    public string VehicleType { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
    public bool AvoidTolls { get; set; }
    public bool AvoidHighways { get; set; }
    public bool PreferEcoRoutes { get; set; }
    public DateTime DepartureTime { get; set; }
    public List<RouteType> PreferredRouteTypes { get; set; } = new();
}

public class WaypointDto
{
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Order { get; set; }
    public bool IsRequired { get; set; } = true;
}

public class RouteOptionDto
{
    public Guid Id { get; set; }
    public RouteType RouteType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double DistanceKm { get; set; }
    public int EstimatedTimeMinutes { get; set; }
    public int TrafficDelayMinutes { get; set; }
    public TripCostBreakdown CostBreakdown { get; set; } = new();
    public bool HasTolls { get; set; }
    public bool IsEcoFriendly { get; set; }
    public double CarbonFootprintKg { get; set; }
    public string RouteGeometry { get; set; } = string.Empty;
    public List<RouteInstructionDto> Instructions { get; set; } = new();
    public List<WaypointDto> Waypoints { get; set; } = new();
    public bool IsRecommended { get; set; }
    public DateTime CalculatedAt { get; set; }
    public List<TrafficConditionDto> TrafficConditions { get; set; } = new();
}

public class RouteInstructionDto
{
    public int Step { get; set; }
    public string Instruction { get; set; } = string.Empty;
    public double DistanceKm { get; set; }
    public int TimeMinutes { get; set; }
    public string Direction { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class TrafficConditionDto
{
    public Guid Id { get; set; }
    public string Location { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public TrafficConditionType Type { get; set; }
    public TrafficSeverity Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? DelayMinutes { get; set; }
    public bool IsActive { get; set; }
}


