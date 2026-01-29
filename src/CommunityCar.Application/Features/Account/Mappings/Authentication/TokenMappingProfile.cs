using AutoMapper;
using CommunityCar.Application.Features.Account.ViewModels.Authentication;
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

    }
}