using AutoMapper;
using CommunityCar.Application.Features.Account.ViewModels.Activity;
using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.Enums.Account;

namespace CommunityCar.Application.Features.Account.Mappings.Activity;

public class UserActivityMappingProfile : AutoMapper.Profile
{
    public UserActivityMappingProfile()
    {
        CreateActivityMappings();
    }

    private void CreateActivityMappings()
    {

        
        CreateMap<UserActivity, TimelineActivityVM>()
            .ForMember(dest => dest.TimeAgo, opt => opt.Ignore())
            .ForMember(dest => dest.ActivityIcon, opt => opt.Ignore())
            .ForMember(dest => dest.ActivityColor, opt => opt.Ignore());

        CreateMap<CreateActivityRequest, UserActivity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        CreateMap<RecordProfileViewRequest, UserActivity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => ActivityType.View))
            .ForMember(dest => dest.EntityType, opt => opt.MapFrom(src => "Profile"))
            .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.ProfileUserId))
            .ForMember(dest => dest.ActivityDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }
}