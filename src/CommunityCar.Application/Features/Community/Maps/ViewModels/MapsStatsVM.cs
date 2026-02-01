using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.Maps.ViewModels;

public class MapsStatsVM
{
    public int TotalPointsOfInterest { get; set; }
    public int TotalRoutes { get; set; }
    public int TotalCheckIns { get; set; }
    public int VerifiedPOIs { get; set; }
    public int ActiveEvents { get; set; }
    public Dictionary<POIType, int> POIsByType { get; set; } = new();
    public Dictionary<RouteType, int> RoutesByType { get; set; } = new();
    public List<PointOfInterestSummaryVM> PopularPOIs { get; set; } = new();
    public List<RouteSummaryVM> PopularRoutes { get; set; } = new();
}