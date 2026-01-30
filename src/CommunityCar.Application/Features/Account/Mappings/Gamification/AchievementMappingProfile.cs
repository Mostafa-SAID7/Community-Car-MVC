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
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.IconUrl, opt => opt.Ignore())
            .ForMember(dest => dest.ProgressPercentage, opt => opt.MapFrom(src => (int)src.ProgressPercentage))
            .ForMember(dest => dest.Progress, opt => opt.MapFrom(src => (double)src.CurrentProgress))
            .ForMember(dest => dest.UnlockedTimeAgo, opt => opt.Ignore())
            .ForMember(dest => dest.StatusText, opt => opt.MapFrom(src => src.IsCompleted ? "Unlocked" : "In Progress"))
            .ForMember(dest => dest.StatusColor, opt => opt.MapFrom(src => src.IsCompleted ? "success" : "warning"));

        CreateMap<GrantAchievementRequest, UserAchievement>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CurrentProgress, opt => opt.MapFrom(src => 100)) // placeholder for max
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        CreateMap<UpdateAchievementProgressRequest, UserAchievement>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AchievementId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.CurrentProgress, opt => opt.MapFrom(src => (int)src.Progress))
            .ForMember(dest => dest.IsCompleted, opt => opt.Ignore())
            .ForMember(dest => dest.CompletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }
}