using AutoMapper;
using CommunityCar.Application.Features.Stories.ViewModels;
using CommunityCar.Domain.Entities.Community.Stories;

namespace CommunityCar.Application.Features.Stories.Mappings;

public class StoriesMappingProfile : global::AutoMapper.Profile
{
    public StoriesMappingProfile()
    {
        CreateMap<Story, StoryVM>()
            .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.VisibilityName, opt => opt.MapFrom(src => src.Visibility.ToString()))
            .ForMember(dest => dest.CarDisplayName, opt => opt.MapFrom(src => src.CarDisplayName))
            .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.IsExpired))
            .ForMember(dest => dest.TimeRemaining, opt => opt.MapFrom(src => FormatTimeRemaining(src.TimeRemaining)))
            .ForMember(dest => dest.TimeAgo, opt => opt.MapFrom(src => FormatTimeAgo(src.CreatedAt)))
            .ForMember(dest => dest.IsMultiMedia, opt => opt.MapFrom(src => src.IsMultiMedia))
            .ForMember(dest => dest.TotalMediaCount, opt => opt.MapFrom(src => src.TotalMediaCount))
            .ForMember(dest => dest.HasLocation, opt => opt.MapFrom(src => src.Latitude.HasValue && src.Longitude.HasValue))
            .ForMember(dest => dest.AuthorName, opt => opt.Ignore()) // Will be populated by service
            .ForMember(dest => dest.MentionedUserNames, opt => opt.Ignore()); // Will be populated by service
    }

    private static string FormatTimeRemaining(TimeSpan timeRemaining)
    {
        if (timeRemaining <= TimeSpan.Zero)
            return "Expired";

        if (timeRemaining.TotalDays >= 1)
            return $"{(int)timeRemaining.TotalDays}d {timeRemaining.Hours}h";
        
        if (timeRemaining.TotalHours >= 1)
            return $"{(int)timeRemaining.TotalHours}h {timeRemaining.Minutes}m";
        
        return $"{timeRemaining.Minutes}m";
    }

    private static string FormatTimeAgo(DateTime createdAt)
    {
        var timeAgo = DateTime.UtcNow - createdAt;

        if (timeAgo.TotalDays >= 1)
            return $"{(int)timeAgo.TotalDays}d ago";
        
        if (timeAgo.TotalHours >= 1)
            return $"{(int)timeAgo.TotalHours}h ago";
        
        if (timeAgo.TotalMinutes >= 1)
            return $"{(int)timeAgo.TotalMinutes}m ago";
        
        return "Just now";
    }
}