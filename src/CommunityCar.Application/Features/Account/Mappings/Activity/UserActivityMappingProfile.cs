using AutoMapper;
using CommunityCar.Application.Features.Account.DTOs.Activity;
using CommunityCar.Application.Features.Account.ViewModels.Activity;
using CommunityCar.Domain.Entities.Account.Analytics;

namespace CommunityCar.Application.Features.Account.Mappings.Activity;

public class UserActivityMappingProfile : AutoMapper.Profile
{
    public UserActivityMappingProfile()
    {
        CreateActivityMappings();
    }

    private void CreateActivityMappings()
    {
        CreateMap<UserActivity, UserActivityDTO>();
        
        CreateMap<UserActivity, UserActivityVM>()
            .ForMember(dest => dest.TimeAgo, opt => opt.Ignore())
            .ForMember(dest => dest.ActivityIcon, opt => opt.Ignore())
            .ForMember(dest => dest.ActivityColor, opt => opt.Ignore());

        CreateMap<CreateActivityRequest, UserActivity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }
}