using AutoMapper;

using CommunityCar.Application.Features.Account.ViewModels.Management;
using CommunityCar.Domain.Entities.Account.Management;

namespace CommunityCar.Application.Features.Account.Mappings.Management;

public class ManagementActionMappingProfile : AutoMapper.Profile
{
    public ManagementActionMappingProfile()
    {
        CreateManagementActionMappings();
    }

    private void CreateManagementActionMappings()
    {


        CreateMap<UserManagementAction, UserManagementActionVM>()
            .ForMember(dest => dest.ManagerName, opt => opt.Ignore())
            .ForMember(dest => dest.TargetUserName, opt => opt.Ignore())
            .ForMember(dest => dest.ManagerProfilePicture, opt => opt.Ignore())
            .ForMember(dest => dest.TargetUserProfilePicture, opt => opt.Ignore())
            .ForMember(dest => dest.TimeAgo, opt => opt.Ignore())
            .ForMember(dest => dest.ActionIcon, opt => opt.Ignore())
            .ForMember(dest => dest.ActionColor, opt => opt.Ignore());

        CreateMap<CreateManagementActionRequest, UserManagementAction>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }
}