using AutoMapper;
using CommunityCar.Application.Features.Profile.DTOs;
using CommunityCar.Application.Features.Profile.ViewModels;
using CommunityCar.Domain.Entities.Account.Core;

namespace CommunityCar.Application.Features.Profile.Mappings;

public class ProfileMappingProfile : AutoMapper.Profile
{
    public ProfileMappingProfile()
    {
        // User to ProfileVM mappings
        CreateMap<User, ProfileVM>()
            .ForMember(dest => dest.HasGoogleAccount, opt => opt.MapFrom(src => src.OAuthInfo.HasGoogleAccount))
            .ForMember(dest => dest.HasFacebookAccount, opt => opt.MapFrom(src => src.OAuthInfo.HasFacebookAccount))
            .ForMember(dest => dest.PostsCount, opt => opt.Ignore())
            .ForMember(dest => dest.CommentsCount, opt => opt.Ignore())
            .ForMember(dest => dest.LikesReceived, opt => opt.Ignore());

        // User to ProfileSettingsVM mappings
        CreateMap<User, ProfileSettingsVM>()
            .ForMember(dest => dest.HasGoogleAccount, opt => opt.MapFrom(src => src.OAuthInfo.HasGoogleAccount))
            .ForMember(dest => dest.HasFacebookAccount, opt => opt.MapFrom(src => src.OAuthInfo.HasFacebookAccount))
            .ForMember(dest => dest.EmailNotifications, opt => opt.Ignore())
            .ForMember(dest => dest.PushNotifications, opt => opt.Ignore())
            .ForMember(dest => dest.SmsNotifications, opt => opt.Ignore())
            .ForMember(dest => dest.MarketingEmails, opt => opt.Ignore())
            .ForMember(dest => dest.ActiveSessions, opt => opt.Ignore());

        // UpdateProfileRequest to User mappings
        CreateMap<UpdateProfileRequest, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.UserName, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());
    }
}


