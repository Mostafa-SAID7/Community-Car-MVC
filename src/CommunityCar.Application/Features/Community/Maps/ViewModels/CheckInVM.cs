using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.Maps.ViewModels;

public class CheckInVM
{
    public Guid Id { get; set; }
    public Guid PointOfInterestId { get; set; }
    public string PointOfInterestName { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime CheckInTime { get; set; }
    public string? Comment { get; set; }
    public double? Rating { get; set; }
    public bool IsPrivate { get; set; }
    public double? CheckInLatitude { get; set; }
    public double? CheckInLongitude { get; set; }
    public double? DistanceFromPOI { get; set; }
}