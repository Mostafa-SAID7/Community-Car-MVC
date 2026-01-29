using AutoMapper;

using CommunityCar.Application.Features.Account.ViewModels.Authentication;
using CommunityCar.Domain.Entities.Account.Authentication;

namespace CommunityCar.Application.Features.Account.Mappings.Authentication;

public class SessionMappingProfile : AutoMapper.Profile
{
    public SessionMappingProfile()
    {
        CreateSessionMappings();
    }

    private void CreateSessionMappings()
    {

        
        CreateMap<UserSession, UserSessionVM>()
            .ForMember(dest => dest.DeviceType, opt => opt.Ignore())
            .ForMember(dest => dest.Browser, opt => opt.Ignore())
            .ForMember(dest => dest.IsCurrent, opt => opt.Ignore())
            .ForMember(dest => dest.TimeAgo, opt => opt.Ignore())
            .ForMember(dest => dest.Duration, opt => opt.Ignore());


    }
}