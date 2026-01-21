using AutoMapper;
using CommunityCar.Application.Features.Stories.ViewModels;
using CommunityCar.Domain.Entities.Community.Stories;

namespace CommunityCar.Application.Features.Stories.Mappings;

public class StoriesMappingProfile : Profile
{
    public StoriesMappingProfile()
    {
        CreateMap<Story, StoryVM>()
            .ForMember(dest => dest.AuthorName, opt => opt.Ignore()) // Will be populated by service
            .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.VisibilityName, opt => opt.MapFrom(src => src.Visibility.ToString()))
            .ForMember(dest => dest.MentionedUserNames, opt => opt.Ignore()) // Will be populated by service
            .ForMember(dest => dest.TimeAgo, opt => opt.Ignore()) // Will be calculated
            .ForMember(dest => dest.TimeRemaining, opt => opt.Ignore()) // Will be calculated
            .ForMember(dest => dest.HasLocation, opt => opt.MapFrom(src => src.Latitude.HasValue && src.Longitude.HasValue));
    }
}