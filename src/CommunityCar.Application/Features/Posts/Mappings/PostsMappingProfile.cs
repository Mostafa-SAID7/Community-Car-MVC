using AutoMapper;
using CommunityCar.Application.Features.Posts.DTOs;
using CommunityCar.Application.Features.Posts.ViewModels;
using CommunityCar.Domain.Entities.Community.Posts;

namespace CommunityCar.Application.Features.Posts.Mappings;

public class PostsMappingProfile : AutoMapper.Profile
{
    public PostsMappingProfile()
    {
        CreateMap<Post, PostSummaryVM>()
            .ForMember(dest => dest.AuthorName, opt => opt.Ignore())
            .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => (Guid?)null))
            .ForMember(dest => dest.GroupName, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => new List<string>()))
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => new List<string>()));

        CreateMap<Post, PostVM>()
            .ForMember(dest => dest.AuthorName, opt => opt.Ignore())
            .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => (Guid?)null))
            .ForMember(dest => dest.GroupName, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => (string?)null))
            .ForMember(dest => dest.IsPinned, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.AllowComments, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => new List<string>()))
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => new List<string>()));

        CreateMap<CreatePostRequest, Post>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());

        CreateMap<UpdatePostRequest, Post>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());
    }
}


