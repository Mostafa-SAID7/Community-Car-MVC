using AutoMapper;

using CommunityCar.Application.Features.Account.ViewModels.Authentication;
using CommunityCar.Domain.ValueObjects.Account;

namespace CommunityCar.Application.Features.Account.Mappings.Authentication;

public class OAuthMappingProfile : AutoMapper.Profile
{
    public OAuthMappingProfile()
    {
        CreateOAuthMappings();
    }

    private void CreateOAuthMappings()
    {

        
        CreateMap<OAuthInfo, OAuthConnectionsVM>()
            .ForMember(dest => dest.AvailableProviders, opt => opt.Ignore());
    }
}