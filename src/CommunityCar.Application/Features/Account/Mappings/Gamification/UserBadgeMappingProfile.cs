using AutoMapper;

using CommunityCar.Application.Features.Account.ViewModels.Gamification;
using CommunityCar.Domain.Entities.Account.Gamification;

namespace CommunityCar.Application.Features.Account.Mappings.Gamification;

public class UserBadgeMappingProfile : AutoMapper.Profile
{
    public UserBadgeMappingProfile()
    {
        CreateBadgeMappings();
    }

    private void CreateBadgeMappings()
    {


        CreateMap<UserBadge, UserBadgeVM>()
            .ForMember(dest => dest.BadgeName, opt => opt.Ignore())
            .ForMember(dest => dest.BadgeDescription, opt => opt.Ignore())
            .ForMember(dest => dest.IconUrl, opt => opt.Ignore())
            .ForMember(dest => dest.AwardedTimeAgo, opt => opt.Ignore())
            .ForMember(dest => dest.RarityColor, opt => opt.MapFrom(src => GetRarityColor(src.Rarity)))
            .ForMember(dest => dest.CategoryIcon, opt => opt.MapFrom(src => GetCategoryIcon(src.Category)));

        CreateMap<AwardBadgeRequest, UserBadge>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Rarity, opt => opt.Ignore())
            .ForMember(dest => dest.IsDisplayed, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.DisplayOrder, opt => opt.MapFrom(src => 0));
    }

    private static string GetRarityColor(string rarity)
    {
        return rarity?.ToLowerInvariant() switch
        {
            "common" => "gray",
            "uncommon" => "green",
            "rare" => "blue",
            "epic" => "purple",
            "legendary" => "orange",
            _ => "gray"
        };
    }

    private static string GetCategoryIcon(string category)
    {
        return category?.ToLowerInvariant() switch
        {
            "social" => "fas fa-users",
            "content" => "fas fa-edit",
            "engagement" => "fas fa-heart",
            "milestone" => "fas fa-trophy",
            "special" => "fas fa-star",
            _ => "fas fa-award"
        };
    }
}