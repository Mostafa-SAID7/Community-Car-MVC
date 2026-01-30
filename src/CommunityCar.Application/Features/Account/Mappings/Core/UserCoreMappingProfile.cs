using AutoMapper;
using CommunityCar.Application.Features.Account.ViewModels.Core;
using CommunityCar.Domain.Entities.Account.Core;

namespace CommunityCar.Application.Features.Account.Mappings.Core;

public class UserCoreMappingProfile : AutoMapper.Profile
{
    public UserCoreMappingProfile()
    {
        CreateUserMappings();
        CreateProfileMappings();
    }

    private void CreateUserMappings()
    {
        // User Entity Mappings
        CreateMap<User, ProfileVM>()
            .ForMember(dest => dest.HasGoogleAccount, opt => opt.MapFrom(src => src.OAuthInfo.HasGoogleAccount))
            .ForMember(dest => dest.HasFacebookAccount, opt => opt.MapFrom(src => src.OAuthInfo.HasFacebookAccount))
            .ForMember(dest => dest.PostsCount, opt => opt.Ignore())
            .ForMember(dest => dest.CommentsCount, opt => opt.Ignore())
            .ForMember(dest => dest.LikesReceived, opt => opt.Ignore())
            .ForMember(dest => dest.SharesReceived, opt => opt.Ignore())
            .ForMember(dest => dest.FollowersCount, opt => opt.Ignore())
            .ForMember(dest => dest.FollowingCount, opt => opt.Ignore())
            .ForMember(dest => dest.AchievementsCount, opt => opt.Ignore())
            .ForMember(dest => dest.BadgesCount, opt => opt.Ignore());

        CreateMap<User, ProfileSettingsVM>()
            .ForMember(dest => dest.HasGoogleAccount, opt => opt.MapFrom(src => src.OAuthInfo.HasGoogleAccount))
            .ForMember(dest => dest.HasFacebookAccount, opt => opt.MapFrom(src => src.OAuthInfo.HasFacebookAccount))
            .ForMember(dest => dest.EmailNotifications, opt => opt.MapFrom(src => src.NotificationSettings.EmailNotificationsEnabled))
            .ForMember(dest => dest.PushNotifications, opt => opt.MapFrom(src => src.NotificationSettings.PushNotificationsEnabled))
            .ForMember(dest => dest.SmsNotifications, opt => opt.MapFrom(src => src.NotificationSettings.SmsNotificationsEnabled))
            .ForMember(dest => dest.MarketingEmails, opt => opt.MapFrom(src => src.NotificationSettings.MarketingEmailsEnabled))
            .ForMember(dest => dest.PublicProfile, opt => opt.MapFrom(src => src.PrivacySettings.IsPublic));

        // Request Mappings
        CreateMap<RegisterVM, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<UpdateProfileVM, User>()
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.UserName, opt => opt.Ignore());
            
        CreateMap<AccountIdentityVM, User>().ReverseMap();
        CreateMap<AccountClaimVM, User>().ReverseMap();
    }

    private void CreateProfileMappings()
    {
        // User statistics mappings
        CreateMap<User, ProfileStatsVM>()
            .ForMember(dest => dest.PostsCount, opt => opt.Ignore())
            .ForMember(dest => dest.CommentsCount, opt => opt.Ignore())
            .ForMember(dest => dest.LikesReceived, opt => opt.Ignore())
            .ForMember(dest => dest.SharesReceived, opt => opt.Ignore())
            .ForMember(dest => dest.FollowersCount, opt => opt.Ignore())
            .ForMember(dest => dest.FollowingCount, opt => opt.Ignore())
            .ForMember(dest => dest.AchievementsCount, opt => opt.Ignore())
            .ForMember(dest => dest.BadgesCount, opt => opt.Ignore())
            .ForMember(dest => dest.GalleryItemsCount, opt => opt.Ignore())
            .ForMember(dest => dest.JoinedDate, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.DaysActive, opt => opt.Ignore())
            .ForMember(dest => dest.LastActivityDate, opt => opt.MapFrom(src => src.LastLoginAt));
    }
}