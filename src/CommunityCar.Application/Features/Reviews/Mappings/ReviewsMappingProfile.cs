using AutoMapper;
using CommunityCar.Application.Features.Reviews.ViewModels;
using CommunityCar.Domain.Entities.Community.Reviews;

namespace CommunityCar.Application.Features.Reviews.Mappings;

public class ReviewsMappingProfile : Profile
{
    public ReviewsMappingProfile()
    {
        CreateMap<Review, ReviewVM>()
            .ForMember(dest => dest.ReviewerName, opt => opt.Ignore()) // Will be populated by service
            .ForMember(dest => dest.TimeAgo, opt => opt.Ignore()) // Will be calculated
            .ForMember(dest => dest.HasImages, opt => opt.MapFrom(src => src.ImageUrls.Any()))
            .ForMember(dest => dest.HasDetailedRatings, opt => opt.MapFrom(src => 
                src.QualityRating.HasValue || src.ValueRating.HasValue || 
                src.ReliabilityRating.HasValue || src.PerformanceRating.HasValue || 
                src.ComfortRating.HasValue));
    }
}