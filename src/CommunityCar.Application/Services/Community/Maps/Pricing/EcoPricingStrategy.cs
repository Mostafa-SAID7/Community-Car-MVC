using CommunityCar.Application.Features.Maps.DTOs;

namespace CommunityCar.Application.Services.Maps.Pricing;

public class EcoPricingStrategy : IPricingStrategy
{
    public string StrategyName => "Eco-Friendly";

    public async Task<TripCostBreakdown> CalculateCostAsync(TripCostRequest request)
    {
        // Use standard pricing as base
        var standardStrategy = new StandardPricingStrategy();
        var breakdown = await standardStrategy.CalculateCostAsync(request);
        breakdown.PricingStrategy = StrategyName;

        // Apply eco-friendly discounts
        if (request.FuelType.ToLower() == "electric")
        {
            var ecoDiscount = breakdown.TotalCost * 0.15m; // 15% discount for electric
            breakdown.AdditionalCosts["Eco Discount"] = -ecoDiscount;
            breakdown.TotalCost -= ecoDiscount;
            breakdown.CostFactors.Add("15% eco-friendly discount applied");
        }
        else if (request.FuelType.ToLower() == "hybrid")
        {
            var ecoDiscount = breakdown.TotalCost * 0.10m; // 10% discount for hybrid
            breakdown.AdditionalCosts["Eco Discount"] = -ecoDiscount;
            breakdown.TotalCost -= ecoDiscount;
            breakdown.CostFactors.Add("10% eco-friendly discount applied");
        }

        // Add carbon offset cost for non-electric vehicles
        if (request.FuelType.ToLower() != "electric")
        {
            var carbonOffset = (decimal)request.DistanceKm * 0.02m; // $0.02 per km for carbon offset
            breakdown.AdditionalCosts["Carbon Offset"] = carbonOffset;
            breakdown.TotalCost += carbonOffset;
            breakdown.CostFactors.Add("Carbon offset included");
        }

        return breakdown;
    }
}