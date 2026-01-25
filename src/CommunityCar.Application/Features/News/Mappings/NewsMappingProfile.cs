using AutoMapper;
using CommunityCar.Application.Features.News.ViewModels;
using CommunityCar.Domain.Entities.Community.News;

namespace CommunityCar.Application.Features.News.Mappings;

public class NewsMappingProfile : AutoMapper.Profile
{
    public NewsMappingProfile()
    {
        CreateMap<NewsItem, NewsItemVM>()
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls.ToList()))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.ToList()))
            .ForMember(dest => dest.AuthorName, opt => opt.Ignore()) // Will be populated by service
            .ForMember(dest => dest.TimeAgo, opt => opt.Ignore()) // Will be calculated
            .ForMember(dest => dest.ReadingTime, opt => opt.Ignore()) // Will be calculated
            .ForMember(dest => dest.Excerpt, opt => opt.Ignore()); // Will be calculated
    }
}