using AutoMapper;
using CommunityCar.Application.Features.Maps.DTOs;
using CommunityCar.Application.Features.Maps.ViewModels;
using CommunityCar.Domain.Entities.Community.Maps;

namespace CommunityCar.Application.Features.Maps.Mappings;

public class MapsMappingProfile : Profile
{
    public MapsMappingProfile()
    {
        // PointOfInterest mappings
        CreateMap<PointOfInterest, PointOfInterestVM>()
            .ForMember(dest => dest.CreatedByName, opt => opt.Ignore())
            .ForMember(dest => dest.VerifiedByName, opt => opt.Ignore())
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls.ToList()));

        CreateMap<PointOfInterest, PointOfInterestSummaryVM>()
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls.ToList()))
            .ForMember(dest => dest.DistanceKm, opt => opt.Ignore());

        // CheckIn mappings
        CreateMap<CheckIn, CheckInVM>()
            .ForMember(dest => dest.PointOfInterestName, opt => opt.Ignore())
            .ForMember(dest => dest.UserName, opt => opt.Ignore());

        // Route mappings
        CreateMap<Route, RouteVM>()
            .ForMember(dest => dest.CreatedByName, opt => opt.Ignore())
            .ForMember(dest => dest.Waypoints, opt => opt.MapFrom(src => src.Waypoints))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.ToList()))
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls.ToList()));

        CreateMap<Route, RouteSummaryVM>()
            .ForMember(dest => dest.CreatedByName, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.ToList()))
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls.ToList()));

        // RouteWaypoint mappings
        CreateMap<RouteWaypoint, RouteWaypointVM>();
    }
}