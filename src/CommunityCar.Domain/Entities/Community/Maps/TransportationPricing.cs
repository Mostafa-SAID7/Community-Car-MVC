using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Community.Maps;

public class TransportationPricing : BaseEntity
{
    public string VehicleType { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
    public decimal BaseFarePerKm { get; set; }
    public decimal BaseFarePerMinute { get; set; }
    public decimal SurgePriceMultiplier { get; set; } = 1.0m;
    public decimal TollCostPerKm { get; set; }
    public bool IsPeakHour { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string Region { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class TripCostEstimate : BaseEntity
{
    public Guid UserId { get; set; }
    public string VehicleType { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
    public double DistanceKm { get; set; }
    public int EstimatedTimeMinutes { get; set; }
    public decimal BaseCost { get; set; }
    public decimal FuelCost { get; set; }
    public decimal TollCost { get; set; }
    public decimal SurgeCost { get; set; }
    public decimal TotalCost { get; set; }
    public string StartLocation { get; set; } = string.Empty;
    public string EndLocation { get; set; } = string.Empty;
    public double StartLatitude { get; set; }
    public double StartLongitude { get; set; }
    public double EndLatitude { get; set; }
    public double EndLongitude { get; set; }
    public DateTime EstimatedAt { get; set; }
    public string PricingStrategy { get; set; } = string.Empty;
}