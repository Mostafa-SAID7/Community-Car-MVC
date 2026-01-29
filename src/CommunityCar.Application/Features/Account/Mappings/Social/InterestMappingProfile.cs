using AutoMapper;
using CommunityCar.Application.Features.Account.DTOs.Social;
using CommunityCar.Application.Features.Account.ViewModels.Social;
using CommunityCar.Domain.Entities.Account.Profile;

namespace CommunityCar.Application.Features.Account.Mappings.Social;

public class InterestMappingProfile : AutoMapper.Profile
{
    public InterestMappingProfile()
    {
        CreateInterestMappings();
    }

    private void CreateInterestMappings()
    {
        CreateMap<UserInterest, UserInterestDTO>()
            .ForMember(dest => dest.InterestName, opt => opt.Ignore());

        CreateMap<UserInterest, ProfileInterestVM>()
            .ForMember(dest => dest.InterestName, opt => opt.Ignore())
            .ForMember(dest => dest.InterestDescription, opt => opt.Ignore())
            .ForMember(dest => dest.CategoryIcon, opt => opt.MapFrom(src => GetCategoryIcon(src.Category)))
            .ForMember(dest => dest.CategoryColor, opt => opt.MapFrom(src => GetCategoryColor(src.Category)))
            .ForMember(dest => dest.UsersWithInterestCount, opt => opt.Ignore());

        CreateMap<AddInterestRequest, UserInterest>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.InterestName, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }

    private static string GetCategoryIcon(string? category)
    {
        return category?.ToLowerInvariant() switch
        {
            "automotive" => "fas fa-car",
            "technology" => "fas fa-laptop",
            "sports" => "fas fa-football-ball",
            "music" => "fas fa-music",
            "travel" => "fas fa-plane",
            "food" => "fas fa-utensils",
            "photography" => "fas fa-camera",
            "gaming" => "fas fa-gamepad",
            "fitness" => "fas fa-dumbbell",
            "books" => "fas fa-book",
            _ => "fas fa-heart"
        };
    }

    private static string GetCategoryColor(string? category)
    {
        return category?.ToLowerInvariant() switch
        {
            "automotive" => "red",
            "technology" => "blue",
            "sports" => "green",
            "music" => "purple",
            "travel" => "teal",
            "food" => "orange",
            "photography" => "pink",
            "gaming" => "indigo",
            "fitness" => "lime",
            "books" => "amber",
            _ => "gray"
        };
    }
}