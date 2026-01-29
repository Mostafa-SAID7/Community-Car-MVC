using AutoMapper;
using CommunityCar.Application.Features.Account.DTOs.Management;
using CommunityCar.Application.Features.Account.ViewModels.Management;
using CommunityCar.Domain.Entities.Account.Management;

namespace CommunityCar.Application.Features.Account.Mappings.Management;

public class UserManagementMappingProfile : AutoMapper.Profile
{
    public UserManagementMappingProfile()
    {
        CreateManagementMappings();
    }

    private void CreateManagementMappings()
    {
        CreateMap<UserManagement, UserManagementDTO>()
            .ForMember(dest => dest.UserName, opt => opt.Ignore())
            .ForMember(dest => dest.ManagerName, opt => opt.Ignore());

        CreateMap<UserManagement, UserManagementVM>()
            .ForMember(dest => dest.UserName, opt => opt.Ignore())
            .ForMember(dest => dest.UserEmail, opt => opt.Ignore())
            .ForMember(dest => dest.UserProfilePicture, opt => opt.Ignore())
            .ForMember(dest => dest.ManagerName, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedTimeAgo, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.StatusColor, opt => opt.Ignore());

        CreateMap<AssignManagerRequest, UserManagement>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore());
    }
}