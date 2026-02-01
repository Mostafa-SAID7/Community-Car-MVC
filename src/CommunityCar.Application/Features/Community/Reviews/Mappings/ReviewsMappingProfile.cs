using AutoMapper;
using CommunityCar.Application.Features.Community.Reviews.ViewModels;
using CommunityCar.Domain.Entities.Community.Reviews;

namespace CommunityCar.Application.Features.Community.Reviews.Mappings;

public class ReviewsMappingProfile : AutoMapper.Profile
{
    public ReviewsMappingProfile()
    {
        CreateMap<Review, ReviewVM>()
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls.ToList()))
            .ForMember(dest => dest.Pros, opt => opt.MapFrom(src => src.Pros.ToList()))
            .ForMember(dest => dest.Cons, opt => opt.MapFrom(src => src.Cons.ToList()))
            .ForMember(dest => dest.ReviewerName, opt => opt.Ignore()) // Will be populated by service
            .ForMember(dest => dest.TimeAgo, opt => opt.Ignore()) // Will be calculated
            .ForMember(dest => dest.HasImages, opt => opt.MapFrom(src => src.ImageUrls.Any()))
            .ForMember(dest => dest.HasDetailedRatings, opt => opt.MapFrom(src => 
                src.QualityRating.HasValue || src.ValueRating.HasValue || 
                src.ReliabilityRating.HasValue || src.PerformanceRating.HasValue || 
                src.ComfortRating.HasValue));
    }
}


