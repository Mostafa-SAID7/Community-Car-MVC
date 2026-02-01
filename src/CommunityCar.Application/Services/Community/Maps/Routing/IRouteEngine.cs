using CommunityCar.Application.Features.Community.Maps.ViewModels;
using CommunityCar.Application.Services.Maps.Pricing;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Services.Maps.Routing;

public interface IRouteEngine
{
    Task<List<RouteOptionVM>> GetRouteOptionsAsync(RouteRequestVM request);
    Task<RouteOptionVM> GetOptimalRouteAsync(RouteRequestVM request, RouteType routeType);
    Task<List<RouteOptionVM>> GetAlternativeRoutesAsync(RouteRequestVM request);
    Task<RouteOptionVM> RecalculateRouteAsync(Guid routeId, TrafficConditionVM[] trafficConditions);
}




