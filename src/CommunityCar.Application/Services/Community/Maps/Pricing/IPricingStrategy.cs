using CommunityCar.Application.Features.Maps.DTOs;

namespace CommunityCar.Application.Services.Maps.Pricing;

public interface IPricingStrategy
{
    string StrategyName { get; }
    Task<TripCostBreakdown> CalculateCostAsync(TripCostRequest request);
}

public class TripCostRequest
{
    public double DistanceKm { get; set; }
    public int EstimatedTimeMinutes { get; set; }
    public string VehicleType { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
    public bool HasTolls { get; set; }
    public bool IsPeakHour { get; set; }
    public string Region { get; set; } = string.Empty;
    public DateTime TripDateTime { get; set; }
    public List<string> RouteFeatures { get; set; } = new(); // Highway, Urban, Rural
}

public class TripCostBreakdown
{
    public decimal BaseCost { get; set; }
    public decimal FuelCost { get; set; }
    public decimal TollCost { get; set; }
    public decimal SurgeCost { get; set; }
    public decimal ParkingCost { get; set; }
    public decimal TotalCost { get; set; }
    public string Currency { get; set; } = "USD";
    public Dictionary<string, decimal> AdditionalCosts { get; set; } = new();
    public List<string> CostFactors { get; set; } = new();
    public string PricingStrategy { get; set; } = string.Empty;
}


