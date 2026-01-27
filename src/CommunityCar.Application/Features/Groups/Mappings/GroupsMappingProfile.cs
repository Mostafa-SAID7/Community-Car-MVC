using AutoMapper;
using CommunityCar.Application.Features.Groups.DTOs;
using CommunityCar.Application.Features.Groups.ViewModels;
using CommunityCar.Domain.Entities.Community.Groups;

namespace CommunityCar.Application.Features.Groups.Mappings;

public class GroupsMappingProfile : AutoMapper.Profile
{
    public GroupsMappingProfile()
    {
        CreateMap<Group, GroupSummaryVM>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags != null ? src.Tags.ToList() : new List<string>()))
            .ForMember(dest => dest.OwnerName, opt => opt.Ignore()); // Will be populated by service

        CreateMap<Group, GroupVM>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags != null ? src.Tags.ToList() : new List<string>()))
            .ForMember(dest => dest.OwnerName, opt => opt.Ignore()); // Will be populated by service

        CreateMap<CreateGroupRequest, Group>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ForMember(dest => dest.MemberCount, opt => opt.Ignore())
            .ForMember(dest => dest.PostCount, opt => opt.Ignore())
            .ForMember(dest => dest.LastActivityAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsVerified, opt => opt.Ignore())
            .ForMember(dest => dest.IsOfficial, opt => opt.Ignore());

        CreateMap<UpdateGroupRequest, Group>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ForMember(dest => dest.MemberCount, opt => opt.Ignore())
            .ForMember(dest => dest.PostCount, opt => opt.Ignore())
            .ForMember(dest => dest.LastActivityAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsVerified, opt => opt.Ignore())
            .ForMember(dest => dest.IsOfficial, opt => opt.Ignore());
    }
}


