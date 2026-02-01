using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.Maps.ViewModels;

public class SubmitCommunityReportVM
{
    public Guid UserId { get; set; }
    public string Location { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public CommunityReportType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}