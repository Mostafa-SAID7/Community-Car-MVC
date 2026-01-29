using AutoMapper;
using CommunityCar.Application.Features.Account.DTOs.Authentication;
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
        CreateMap<UserSession, UserSessionDTO>();
        
        CreateMap<UserSession, UserSessionVM>()
            .ForMember(dest => dest.DeviceType, opt => opt.Ignore())
            .ForMember(dest => dest.Browser, opt => opt.Ignore())
            .ForMember(dest => dest.IsCurrent, opt => opt.Ignore())
            .ForMember(dest => dest.TimeAgo, opt => opt.Ignore())
            .ForMember(dest => dest.Duration, opt => opt.Ignore());

        CreateMap<CreateSessionRequest, UserSession>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.LastActivityAt, opt => opt.Ignore())
            .ForMember(dest => dest.EndedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ExpiresAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.IsSuspicious, opt => opt.Ignore());
    }
}