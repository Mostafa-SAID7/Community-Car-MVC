using AutoMapper;
using CommunityCar.Application.Features.Account.DTOs.Authentication;
using CommunityCar.Domain.Entities.Account.Authentication;

namespace CommunityCar.Application.Features.Account.Mappings.Authentication;

public class TokenMappingProfile : AutoMapper.Profile
{
    public TokenMappingProfile()
    {
        CreateTokenMappings();
    }

    private void CreateTokenMappings()
    {
        CreateMap<UserToken, UserTokenDTO>();
        
        CreateMap<CreateTokenRequest, UserToken>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.IsExpired, opt => opt.Ignore());
    }
}