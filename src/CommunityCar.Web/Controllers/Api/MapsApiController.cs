using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Services.Maps.Routing;
using CommunityCar.Application.Services.Maps.Pricing;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Web.Controllers.Api;

[ApiController]
[Route("api/maps")]
public class MapsApiController : ControllerBase
{
    private readonly IRouteEngine _routeEngine;
    private readonly IEnumerable<IPricingStrategy> _pricingStrategies;

    public MapsApiController(
        IRouteEngine routeEngine,
        IEnumerable<IPricingStrategy> pricingStrategies)
    {
        _routeEngine = routeEngine;
        _pricingStrategies = pricingStrategies;
    }

    [HttpPost("routes/calculate")]
    public async Task<IActionResult> CalculateRoute([FromBody] RouteRequestDto request)
    {
        try
        {
            var routes = await _routeEngine.GetRouteOptionsAsync(request);
            return Ok(routes);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("routes/optimal")]
    public async Task<IActionResult> GetOptimalRoute([FromBody] RouteRequestDto request, [FromQuery] RouteType routeType = RouteType.Fastest)
    {
        try
        {
            var route = await _routeEngine.GetOptimalRouteAsync(request, routeType);
            return Ok(route);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("routes/alternatives")]
    public async Task<IActionResult> GetAlternativeRoutes([FromBody] RouteRequestDto request)
    {
        try
        {
            var routes = await _routeEngine.GetAlternativeRoutesAsync(request);
            return Ok(routes);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("routes/{routeId}/recalculate")]
    public async Task<IActionResult> RecalculateRoute(Guid routeId, [FromBody] TrafficConditionDto[] trafficConditions)
    {
        try
        {
            var route = await _routeEngine.RecalculateRouteAsync(routeId, trafficConditions);
            return Ok(route);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("pricing/strategies")]
    public IActionResult GetPricingStrategies()
    {
        var strategies = _pricingStrategies.Select(s => new { Name = s.StrategyName });
        return Ok(strategies);
    }

    [HttpPost("pricing/calculate")]
    public async Task<IActionResult> CalculatePricing([FromBody] TripCostRequest request, [FromQuery] string strategy = "Standard")
    {
        try
        {
            var pricingStrategy = _pricingStrategies.FirstOrDefault(s => s.StrategyName == strategy);
            if (pricingStrategy == null)
            {
                return BadRequest(new { error = "Invalid pricing strategy" });
            }

            var cost = await pricingStrategy.CalculateCostAsync(request);
            return Ok(cost);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}