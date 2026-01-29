using AutoMapper;
using CommunityCar.Application.Features.Account.DTOs.Social;
using CommunityCar.Application.Features.Account.ViewModels.Social;
using CommunityCar.Domain.Entities.Account.Profile;

namespace CommunityCar.Application.Features.Account.Mappings.Social;

public class ProfileViewMappingProfile : AutoMapper.Profile
{
    public ProfileViewMappingProfile()
    {
        CreateProfileViewMappings();
    }

    private void CreateProfileViewMappings()
    {
        CreateMap<UserProfileView, UserProfileViewDTO>()
            .ForMember(dest => dest.ViewerName, opt => opt.Ignore())
            .ForMember(dest => dest.ViewerProfilePicture, opt => opt.Ignore())
            .ForMember(dest => dest.Location, opt => opt.Ignore());

        CreateMap<UserProfileView, UserProfileViewVM>()
            .ForMember(dest => dest.ViewerName, opt => opt.Ignore())
            .ForMember(dest => dest.ViewerProfilePicture, opt => opt.Ignore())
            .ForMember(dest => dest.Location, opt => opt.Ignore())
            .ForMember(dest => dest.ViewedTimeAgo, opt => opt.Ignore())
            .ForMember(dest => dest.DeviceType, opt => opt.MapFrom(src => GetDeviceType(src.UserAgent)));

        CreateMap<RecordProfileViewRequest, UserProfileView>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ViewedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsAnonymous, opt => opt.MapFrom(src => src.ViewerId == Guid.Empty));
    }

    private static string GetDeviceType(string? userAgent)
    {
        if (string.IsNullOrEmpty(userAgent)) return "Unknown";

        var ua = userAgent.ToLowerInvariant();
        if (ua.Contains("mobile") || ua.Contains("android") || ua.Contains("iphone"))
            return "Mobile";
        if (ua.Contains("tablet") || ua.Contains("ipad"))
            return "Tablet";
        return "Desktop";
    }
}