using CommunityCar.Application.Features.Maps.DTOs;

namespace CommunityCar.Application.Services.Maps.Pricing;

public class StandardPricingStrategy : IPricingStrategy
{
    public string StrategyName => "Standard";

    public async Task<TripCostBreakdown> CalculateCostAsync(TripCostRequest request)
    {
        await Task.Delay(1); // Simulate async operation
        
        var pricing = GetPricingForVehicle(request.VehicleType, request.FuelType, request.Region);
        
        var breakdown = new TripCostBreakdown
        {
            PricingStrategy = StrategyName
        };

        // Base cost calculation
        breakdown.BaseCost = (decimal)request.DistanceKm * pricing.BaseFarePerKm + 
                           request.EstimatedTimeMinutes * pricing.BaseFarePerMinute;

        // Fuel cost calculation
        breakdown.FuelCost = CalculateFuelCost(request.DistanceKm, request.VehicleType, request.FuelType);

        // Toll cost calculation
        if (request.HasTolls)
        {
            breakdown.TollCost = (decimal)request.DistanceKm * pricing.TollCostPerKm;
        }

        // Surge pricing for peak hours
        if (request.IsPeakHour)
        {
            breakdown.SurgeCost = breakdown.BaseCost * (pricing.SurgePriceMultiplier - 1);
            breakdown.CostFactors.Add("Peak hour surge applied");
        }

        // Calculate total
        breakdown.TotalCost = breakdown.BaseCost + breakdown.FuelCost + 
                            breakdown.TollCost + breakdown.SurgeCost + breakdown.ParkingCost;

        // Add cost factors
        breakdown.CostFactors.Add($"Distance: {request.DistanceKm:F1} km");
        breakdown.CostFactors.Add($"Estimated time: {request.EstimatedTimeMinutes} minutes");
        breakdown.CostFactors.Add($"Vehicle: {request.VehicleType}");
        breakdown.CostFactors.Add($"Fuel: {request.FuelType}");

        return breakdown;
    }

    private PricingInfo GetPricingForVehicle(string vehicleType, string fuelType, string region)
    {
        // Return default pricing based on vehicle and fuel type
        return new PricingInfo
        {
            VehicleType = vehicleType,
            FuelType = fuelType,
            BaseFarePerKm = GetBaseFarePerKm(vehicleType),
            BaseFarePerMinute = 0.25m,
            TollCostPerKm = 0.10m,
            SurgePriceMultiplier = 1.5m,
            Region = region
        };
    }

    private decimal GetBaseFarePerKm(string vehicleType)
    {
        return vehicleType.ToLower() switch
        {
            "compact" => 0.40m,
            "sedan" => 0.50m,
            "suv" => 0.65m,
            "truck" => 0.80m,
            _ => 0.50m
        };
    }

    private decimal CalculateFuelCost(double distanceKm, string vehicleType, string fuelType)
    {
        // Simplified fuel consumption calculation
        var fuelConsumptionPer100Km = GetFuelConsumption(vehicleType, fuelType);
        var fuelPrice = GetFuelPrice(fuelType);
        
        return (decimal)((distanceKm / 100) * fuelConsumptionPer100Km * fuelPrice);
    }

    private double GetFuelConsumption(string vehicleType, string fuelType)
    {
        // Simplified consumption rates (liters per 100km or kWh per 100km)
        return vehicleType.ToLower() switch
        {
            "compact" => fuelType.ToLower() == "electric" ? 15.0 : 6.0,
            "sedan" => fuelType.ToLower() == "electric" ? 18.0 : 8.0,
            "suv" => fuelType.ToLower() == "electric" ? 22.0 : 10.0,
            "truck" => fuelType.ToLower() == "electric" ? 30.0 : 12.0,
            _ => fuelType.ToLower() == "electric" ? 18.0 : 8.0
        };
    }

    private double GetFuelPrice(string fuelType)
    {
        // Simplified fuel prices (per liter or per kWh)
        return fuelType.ToLower() switch
        {
            "gasoline" => 1.50,
            "diesel" => 1.60,
            "electric" => 0.12,
            "hybrid" => 1.45,
            _ => 1.50
        };
    }

    private class PricingInfo
    {
        public string VehicleType { get; set; } = string.Empty;
        public string FuelType { get; set; } = string.Empty;
        public decimal BaseFarePerKm { get; set; }
        public decimal BaseFarePerMinute { get; set; }
        public decimal TollCostPerKm { get; set; }
        public decimal SurgePriceMultiplier { get; set; }
        public string Region { get; set; } = string.Empty;
    }
}