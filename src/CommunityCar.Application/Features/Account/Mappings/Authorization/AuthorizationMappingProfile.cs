using AutoMapper;
using CommunityCar.Application.Features.Account.ViewModels.Authorization;
using CommunityCar.Domain.Entities.Account.Authorization;

namespace CommunityCar.Application.Features.Account.Mappings.Authorization;

public class AuthorizationMappingProfile : Profile
{
    public AuthorizationMappingProfile()
    {
        // Role mappings
        CreateMap<Role, RoleVM>()
            .ForMember(dest => dest.Permissions, opt => opt.Ignore())
            .ForMember(dest => dest.UserCount, opt => opt.Ignore())
            .ForMember(dest => dest.PermissionCount, opt => opt.Ignore());

        // Permission mappings
        CreateMap<Permission, PermissionVM>();
    }
}
