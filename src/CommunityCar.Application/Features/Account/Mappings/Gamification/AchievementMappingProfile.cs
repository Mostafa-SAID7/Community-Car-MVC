using AutoMapper;

using CommunityCar.Application.Features.Account.ViewModels.Gamification;
using CommunityCar.Domain.Entities.Account.Gamification;

namespace CommunityCar.Application.Features.Account.Mappings.Gamification;

public class AchievementMappingProfile : AutoMapper.Profile
{
    public AchievementMappingProfile()
    {
        CreateAchievementMappings();
    }

    private void CreateAchievementMappings()
    {


        CreateMap<UserAchievement, UserAchievementVM>()
            .ForMember(dest => dest.AchievementName, opt => opt.Ignore())
            .ForMember(dest => dest.AchievementDescription, opt => opt.Ignore())
            .ForMember(dest => dest.IconUrl, opt => opt.Ignore())
            .ForMember(dest => dest.ProgressPercentage, opt => opt.MapFrom(src => (int)(src.Progress * 100)))
            .ForMember(dest => dest.UnlockedTimeAgo, opt => opt.Ignore())
            .ForMember(dest => dest.StatusText, opt => opt.MapFrom(src => src.IsUnlocked ? "Unlocked" : "In Progress"))
            .ForMember(dest => dest.StatusColor, opt => opt.MapFrom(src => src.IsUnlocked ? "success" : "warning"));

        CreateMap<GrantAchievementRequest, UserAchievement>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Progress, opt => opt.MapFrom(src => 1.0))
            .ForMember(dest => dest.IsUnlocked, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.AchievementType, opt => opt.Ignore());

        CreateMap<UpdateAchievementProgressRequest, UserAchievement>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AchievementId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.IsUnlocked, opt => opt.MapFrom(src => src.Progress >= 1.0))
            .ForMember(dest => dest.UnlockedAt, opt => opt.MapFrom(src => src.Progress >= 1.0 ? DateTime.UtcNow : (DateTime?)null))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.AchievementType, opt => opt.Ignore());
    }
}