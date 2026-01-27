using AutoMapper;
using CommunityCar.Application.Features.Events.DTOs;
using CommunityCar.Application.Features.Events.ViewModels;
using CommunityCar.Domain.Entities.Community.Events;

namespace CommunityCar.Application.Features.Events.Mappings;

public class EventMappingProfile : AutoMapper.Profile
{
    public EventMappingProfile()
    {
        CreateMap<Event, EventSummaryVM>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.ToList()))
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls.ToList()))
            .ForMember(dest => dest.OrganizerName, opt => opt.Ignore())
            .ForMember(dest => dest.DistanceKm, opt => opt.Ignore());

        CreateMap<Event, EventVM>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.ToList()))
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls.ToList()))
            .ForMember(dest => dest.OrganizerName, opt => opt.Ignore());

        CreateMap<CreateEventRequest, Event>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.OrganizerId, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.AttendeeCount, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ForMember(dest => dest.ImageUrls, opt => opt.Ignore())
            .ForMember(dest => dest.IsCancelled, opt => opt.Ignore());

        CreateMap<UpdateEventRequest, Event>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.OrganizerId, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.AttendeeCount, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ForMember(dest => dest.ImageUrls, opt => opt.Ignore())
            .ForMember(dest => dest.IsCancelled, opt => opt.Ignore());
    }
}


