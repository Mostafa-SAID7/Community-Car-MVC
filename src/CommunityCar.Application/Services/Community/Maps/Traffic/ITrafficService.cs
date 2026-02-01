using CommunityCar.Application.Features.Community.Maps.ViewModels;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Services.Maps.Traffic;

public interface ITrafficService
{
    Task<List<TrafficConditionVM>> GetLiveTrafficAsync(double latitude, double longitude, double radiusKm = 10);
    Task<List<TrafficConditionVM>> GetTrafficOnRouteAsync(string routeGeometry);
    Task UpdateTrafficConditionsAsync();
    Task<bool> ReportTrafficConditionAsync(ReportTrafficConditionVM request);
    Task<List<TrafficConditionVM>> GetTrafficAlertsAsync(Guid userId);
}

public class ReportTrafficConditionVM
{
    public Guid UserId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public TrafficConditionType Type { get; set; }
    public TrafficSeverity Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}


