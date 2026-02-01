using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.Maps.ViewModels;

public class TrafficConditionVM
{
    public Guid Id { get; set; }
    public string Location { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Condition { get; set; } = string.Empty;
    public TrafficConditionType Type { get; set; }
    public string Severity { get; set; } = string.Empty;
    public TrafficSeverity TrafficSeverity { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime ReportedAt { get; set; }
    public string Description { get; set; } = string.Empty;
    public int? DelayMinutes { get; set; }
    public bool IsActive { get; set; }
}