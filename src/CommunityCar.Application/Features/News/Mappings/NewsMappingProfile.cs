using AutoMapper;
using CommunityCar.Application.Features.News.ViewModels;
using CommunityCar.Domain.Entities.Community.News;

namespace CommunityCar.Application.Features.News.Mappings;

public class NewsMappingProfile : Profile
{
    public NewsMappingProfile()
    {
        CreateMap<NewsItem, NewsItemVM>()
            .ForMember(dest => dest.AuthorName, opt => opt.Ignore()) // Will be populated by service
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.ToString()))
            .ForMember(dest => dest.TimeAgo, opt => opt.Ignore()) // Will be calculated
            .ForMember(dest => dest.ReadingTime, opt => opt.Ignore()) // Will be calculated
            .ForMember(dest => dest.HasMultipleImages, opt => opt.MapFrom(src => src.ImageUrls.Count > 1))
            .ForMember(dest => dest.Excerpt, opt => opt.Ignore()); // Will be calculated
    }
}