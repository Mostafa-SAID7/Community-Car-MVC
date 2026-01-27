using CommunityCar.Application.Services.Maps.Routing;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Services.Maps.Traffic;

public interface ITrafficService
{
    Task<List<TrafficConditionDto>> GetLiveTrafficAsync(double latitude, double longitude, double radiusKm = 10);
    Task<List<TrafficConditionDto>> GetTrafficOnRouteAsync(string routeGeometry);
    Task UpdateTrafficConditionsAsync();
    Task<bool> ReportTrafficConditionAsync(ReportTrafficConditionRequest request);
    Task<List<TrafficConditionDto>> GetTrafficAlertsAsync(Guid userId);
}

public class ReportTrafficConditionRequest
{
    public Guid UserId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public TrafficConditionType Type { get; set; }
    public TrafficSeverity Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}


