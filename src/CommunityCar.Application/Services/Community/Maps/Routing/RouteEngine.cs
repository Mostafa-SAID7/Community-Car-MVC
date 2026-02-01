using CommunityCar.Application.Features.Community.Maps.ViewModels;
using CommunityCar.Application.Services.Maps.Pricing;
using CommunityCar.Application.Common.Interfaces.Data;
using CommunityCar.Domain.Enums.Community;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Application.Services.Maps.Routing;

public class RouteEngine : IRouteEngine
{
    private readonly IApplicationDbContext _context;
    private readonly Dictionary<string, IPricingStrategy> _pricingStrategies;

    public RouteEngine(
        IApplicationDbContext context,
        IEnumerable<IPricingStrategy> pricingStrategies)
    {
        _context = context;
        _pricingStrategies = pricingStrategies.ToDictionary(s => s.StrategyName, s => s);
    }

    public async Task<List<RouteOptionVM>> GetRouteOptionsAsync(RouteRequestVM request)
    {
        var routes = new List<RouteOptionVM>();

        // Generate different route types
        var routeTypes = new[] { RouteType.Fastest, RouteType.Shortest, RouteType.Cheapest, RouteType.EcoFriendly };

        foreach (var routeType in routeTypes)
        {
            var route = await GenerateRouteAsync(request, routeType);
            if (route != null)
            {
                routes.Add(route);
            }
        }

        // Get traffic conditions for the area
        var trafficConditions = await GetTrafficConditionsAsync(
            request.StartLatitude, request.StartLongitude,
            request.EndLatitude, request.EndLongitude);

        // Apply traffic conditions to routes
        foreach (var route in routes)
        {
            ApplyTrafficConditions(route, trafficConditions);
        }

        // Mark recommended route
        var recommendedRoute = DetermineRecommendedRoute(routes);
        if (recommendedRoute != null)
        {
            recommendedRoute.IsRecommended = true;
        }

        return routes.OrderBy(r => r.IsRecommended ? 0 : 1)
                    .ThenBy(r => r.EstimatedTimeMinutes + r.TrafficDelayMinutes)
                    .ToList();
    }

    public async Task<RouteOptionVM> GetOptimalRouteAsync(RouteRequestVM request, RouteType routeType)
    {
        var route = await GenerateRouteAsync(request, routeType);
        if (route == null) return null!;

        var trafficConditions = await GetTrafficConditionsAsync(
            request.StartLatitude, request.StartLongitude,
            request.EndLatitude, request.EndLongitude);

        ApplyTrafficConditions(route, trafficConditions);
        return route;
    }

    public async Task<List<RouteOptionVM>> GetAlternativeRoutesAsync(RouteRequestVM request)
    {
        // Generate up to 3 alternative routes with different characteristics
        var alternatives = new List<RouteOptionVM>();

        // Main route (fastest)
        var mainRoute = await GenerateRouteAsync(request, RouteType.Fastest);
        if (mainRoute != null) alternatives.Add(mainRoute);

        // Alternative 1: Avoid highways
        var noHighwayRequest = new RouteRequestVM
        {
            StartLocation = request.StartLocation,
            EndLocation = request.EndLocation,
            StartLatitude = request.StartLatitude,
            StartLongitude = request.StartLongitude,
            EndLatitude = request.EndLatitude,
            EndLongitude = request.EndLongitude,
            Waypoints = request.Waypoints,
            VehicleType = request.VehicleType,
            FuelType = request.FuelType,
            AvoidTolls = request.AvoidTolls,
            AvoidHighways = true,
            PreferEcoRoutes = request.PreferEcoRoutes,
            DepartureTime = request.DepartureTime,
            PreferredRouteTypes = request.PreferredRouteTypes
        };
        var altRoute1 = await GenerateRouteAsync(noHighwayRequest, RouteType.Shortest);
        if (altRoute1 != null && !IsSimilarRoute(altRoute1, alternatives))
        {
            altRoute1.Name = "Avoid Highways";
            alternatives.Add(altRoute1);
        }

        // Alternative 2: Avoid tolls
        var noTollRequest = new RouteRequestVM
        {
            StartLocation = request.StartLocation,
            EndLocation = request.EndLocation,
            StartLatitude = request.StartLatitude,
            StartLongitude = request.StartLongitude,
            EndLatitude = request.EndLatitude,
            EndLongitude = request.EndLongitude,
            Waypoints = request.Waypoints,
            VehicleType = request.VehicleType,
            FuelType = request.FuelType,
            AvoidTolls = true,
            AvoidHighways = request.AvoidHighways,
            PreferEcoRoutes = request.PreferEcoRoutes,
            DepartureTime = request.DepartureTime,
            PreferredRouteTypes = request.PreferredRouteTypes
        };
        var altRoute2 = await GenerateRouteAsync(noTollRequest, RouteType.Cheapest);
        if (altRoute2 != null && !IsSimilarRoute(altRoute2, alternatives))
        {
            altRoute2.Name = "Avoid Tolls";
            alternatives.Add(altRoute2);
        }

        return alternatives;
    }

    public async Task<RouteOptionVM> RecalculateRouteAsync(Guid routeId, TrafficConditionVM[] trafficConditions)
    {
        // Since we don't have RouteOptions entity, we'll simulate recalculation
        // In a real implementation, this would fetch from cache or external API
        var routeDto = new RouteOptionVM
        {
            Id = routeId,
            RouteType = RouteType.Fastest,
            Name = "Recalculated Route",
            Description = "Route recalculated with current traffic conditions",
            CalculatedAt = DateTime.UtcNow
        };

        ApplyTrafficConditions(routeDto, trafficConditions);
        return routeDto;
    }

    private async Task<RouteOptionVM> GenerateRouteAsync(RouteRequestVM request, RouteType routeType)
    {
        // This would integrate with external routing APIs (Google Maps, OpenStreetMap, etc.)
        // For now, we'll simulate route generation
        
        var route = new RouteOptionVM
        {
            Id = Guid.NewGuid(),
            RouteType = routeType,
            Name = GetRouteTypeName(routeType),
            Description = GetRouteTypeDescription(routeType),
            CalculatedAt = DateTime.UtcNow
        };

        // Simulate route calculation based on type
        var baseDistance = CalculateDistance(request.StartLatitude, request.StartLongitude,
                                           request.EndLatitude, request.EndLongitude);
        
        switch (routeType)
        {
            case RouteType.Shortest:
                route.DistanceKm = baseDistance;
                route.EstimatedTimeMinutes = (int)(baseDistance / 0.8); // Slower on shorter routes
                break;
            case RouteType.Fastest:
                route.DistanceKm = baseDistance * 1.1; // Slightly longer but faster
                route.EstimatedTimeMinutes = (int)(baseDistance / 1.2);
                break;
            case RouteType.Cheapest:
                route.DistanceKm = baseDistance * 0.95;
                route.EstimatedTimeMinutes = (int)(baseDistance / 0.9);
                route.HasTolls = false;
                break;
            case RouteType.EcoFriendly:
                route.DistanceKm = baseDistance * 1.05;
                route.EstimatedTimeMinutes = (int)(baseDistance / 1.0);
                route.IsEcoFriendly = true;
                route.CarbonFootprintKg = CalculateCarbonFootprint(route.DistanceKm, request.FuelType);
                break;
        }

        // Calculate cost using appropriate pricing strategy
        var pricingStrategy = GetPricingStrategy(routeType);
        var costRequest = new TripCostRequest
        {
            DistanceKm = route.DistanceKm,
            EstimatedTimeMinutes = route.EstimatedTimeMinutes,
            VehicleType = request.VehicleType,
            FuelType = request.FuelType,
            HasTolls = route.HasTolls,
            IsPeakHour = IsPeakHour(request.DepartureTime ?? DateTime.UtcNow),
            Region = "Default",
            TripDateTime = request.DepartureTime ?? DateTime.UtcNow
        };

        route.CostBreakdown = ConvertToViewModel(await pricingStrategy.CalculateCostAsync(costRequest));

        // Generate sample instructions
        route.Instructions = GenerateSampleInstructions(request, route);

        return route;
    }

    private async Task<TrafficConditionVM[]> GetTrafficConditionsAsync(
        double startLat, double startLng, double endLat, double endLng)
    {
        // Since we don't have TrafficConditions entity, we'll simulate traffic data
        // In a real implementation, this would integrate with traffic APIs
        await Task.Delay(1); // Simulate async operation
        
        var conditions = new List<TrafficConditionVM>();
        
        // Simulate some traffic conditions based on time of day
        var currentHour = DateTime.Now.Hour;
        if (currentHour >= 7 && currentHour <= 9 || currentHour >= 17 && currentHour <= 19)
        {
            conditions.Add(new TrafficConditionVM
            {
                Id = Guid.NewGuid(),
                Location = "Main Highway",
                Latitude = (startLat + endLat) / 2,
                Longitude = (startLng + endLng) / 2,
                Type = TrafficConditionType.HeavyTraffic,
                TrafficSeverity = TrafficSeverity.Moderate,
                Severity = "Moderate",
                Description = "Heavy traffic during rush hour",
                StartTime = DateTime.Now.AddHours(-1),
                EndTime = DateTime.Now.AddHours(1),
                DelayMinutes = 15,
                IsActive = true
            });
        }

        return conditions.ToArray();
    }

    private void ApplyTrafficConditions(RouteOptionVM route, TrafficConditionVM[] trafficConditions)
    {
        route.TrafficConditions = trafficConditions.ToList();
        
        // Calculate traffic delays
        var totalDelay = 0;
        foreach (var condition in trafficConditions)
        {
            if (condition.DelayMinutes.HasValue)
            {
                totalDelay += condition.DelayMinutes.Value;
            }
            else
            {
                // Estimate delay based on severity
                totalDelay += condition.TrafficSeverity switch
                {
                    TrafficSeverity.Low => 2,
                    TrafficSeverity.Moderate => 5,
                    TrafficSeverity.High => 10,
                    TrafficSeverity.Severe => 20,
                    _ => 0
                };
            }
        }

        route.TrafficDelayMinutes = totalDelay;
    }

    private RouteOptionVM? DetermineRecommendedRoute(List<RouteOptionVM> routes)
    {
        if (!routes.Any()) return null;

        // Score routes based on multiple factors
        var scoredRoutes = routes.Select(r => new
        {
            Route = r,
            Score = CalculateRouteScore(r)
        }).OrderByDescending(x => x.Score);

        return scoredRoutes.First().Route;
    }

    private double CalculateRouteScore(RouteOptionVM route)
    {
        // Scoring algorithm considering time, cost, and traffic
        var timeScore = Math.Max(0, 100 - (route.EstimatedTimeMinutes + route.TrafficDelayMinutes) / 2);
        var costScore = Math.Max(0, 100 - (double)route.CostBreakdown.TotalCost);
        var trafficScore = Math.Max(0, 100 - route.TrafficDelayMinutes * 2);
        var ecoScore = route.IsEcoFriendly ? 20 : 0;

        return (timeScore * 0.4) + (costScore * 0.3) + (trafficScore * 0.2) + (ecoScore * 0.1);
    }

    private bool IsSimilarRoute(RouteOptionVM newRoute, List<RouteOptionVM> existingRoutes)
    {
        return existingRoutes.Any(r => 
            Math.Abs(r.DistanceKm - newRoute.DistanceKm) < 2.0 &&
            Math.Abs(r.EstimatedTimeMinutes - newRoute.EstimatedTimeMinutes) < 10);
    }



    private double CalculateDistance(double lat1, double lng1, double lat2, double lng2)
    {
        // Haversine formula for distance calculation
        const double R = 6371; // Earth's radius in kilometers
        var dLat = ToRadians(lat2 - lat1);
        var dLng = ToRadians(lng2 - lng1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private double ToRadians(double degrees) => degrees * Math.PI / 180;

    private double CalculateCarbonFootprint(double distanceKm, string fuelType)
    {
        // CO2 emissions per km by fuel type
        var emissionFactor = fuelType.ToLower() switch
        {
            "gasoline" => 0.21, // kg CO2 per km
            "diesel" => 0.18,
            "hybrid" => 0.12,
            "electric" => 0.05, // Considering electricity generation
            _ => 0.21
        };

        return distanceKm * emissionFactor;
    }

    private IPricingStrategy GetPricingStrategy(RouteType routeType)
    {
        return routeType switch
        {
            RouteType.EcoFriendly => _pricingStrategies.GetValueOrDefault("Eco-Friendly") ?? _pricingStrategies["Standard"],
            _ => _pricingStrategies["Standard"]
        };
    }

    private bool IsPeakHour(DateTime dateTime)
    {
        var hour = dateTime.Hour;
        return (hour >= 7 && hour <= 9) || (hour >= 17 && hour <= 19);
    }

    private string GetRouteTypeName(RouteType routeType)
    {
        return routeType switch
        {
            RouteType.Fastest => "Fastest Route",
            RouteType.Shortest => "Shortest Route",
            RouteType.Cheapest => "Most Economical",
            RouteType.EcoFriendly => "Eco-Friendly",
            _ => "Standard Route"
        };
    }

    private string GetRouteTypeDescription(RouteType routeType)
    {
        return routeType switch
        {
            RouteType.Fastest => "Optimized for minimal travel time",
            RouteType.Shortest => "Optimized for minimal distance",
            RouteType.Cheapest => "Optimized for lowest cost including tolls",
            RouteType.EcoFriendly => "Optimized for minimal environmental impact",
            _ => "Balanced route considering time and distance"
        };
    }

    private List<RouteInstructionVM> GenerateSampleInstructions(RouteRequestVM request, RouteOptionVM route)
    {
        // This would normally come from the routing API
        return new List<RouteInstructionVM>
        {
            new() { Step = 1, Instruction = $"Head towards {request.EndLocation}", DistanceKm = route.DistanceKm * 0.1, TimeMinutes = 5 },
            new() { Step = 2, Instruction = "Continue straight", DistanceKm = route.DistanceKm * 0.7, TimeMinutes = route.EstimatedTimeMinutes - 10 },
            new() { Step = 3, Instruction = $"Arrive at {request.EndLocation}", DistanceKm = route.DistanceKm * 0.2, TimeMinutes = 5 }
        };
    }

    private TripCostBreakdownVM ConvertToViewModel(TripCostBreakdown breakdown)
    {
        return new TripCostBreakdownVM
        {
            FuelCost = breakdown.FuelCost,
            TollCost = breakdown.TollCost,
            ParkingCost = breakdown.ParkingCost,
            ServiceFee = breakdown.SurgeCost,
            TotalCost = breakdown.TotalCost,
            Currency = breakdown.Currency,
            CostItems = breakdown.AdditionalCosts.Select(kvp => new CostItemVM
            {
                Name = kvp.Key,
                Amount = kvp.Value,
                Description = $"{kvp.Key} cost"
            }).ToList()
        };
    }
}


