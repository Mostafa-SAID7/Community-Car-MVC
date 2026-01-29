using AutoMapper;
using CommunityCar.Application.Features.Account.ViewModels.Social;
using CommunityCar.Domain.Entities.Account.Profile;

namespace CommunityCar.Application.Features.Account.Mappings.Social;

public class UserFollowingMappingProfile : AutoMapper.Profile
{
    public UserFollowingMappingProfile()
    {
        CreateFollowingMappings();
    }

    private void CreateFollowingMappings()
    {
        CreateMap<UserFollowing, NetworkUserVM>()

            .ForMember(dest => dest.FollowerName, opt => opt.Ignore())
            .ForMember(dest => dest.FollowingName, opt => opt.Ignore())
            .ForMember(dest => dest.FollowerProfilePicture, opt => opt.Ignore())
            .ForMember(dest => dest.FollowingProfilePicture, opt => opt.Ignore())
            .ForMember(dest => dest.FollowerBio, opt => opt.Ignore())
            .ForMember(dest => dest.FollowingBio, opt => opt.Ignore())
            .ForMember(dest => dest.FollowedTimeAgo, opt => opt.Ignore())
            .ForMember(dest => dest.IsMutual, opt => opt.Ignore())
            .ForMember(dest => dest.IsOnline, opt => opt.Ignore())
            .ForMember(dest => dest.LastSeen, opt => opt.Ignore());

        CreateMap<FollowUserRequest, UserFollowing>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }
}