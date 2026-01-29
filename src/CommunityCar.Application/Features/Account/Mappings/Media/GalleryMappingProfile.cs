using AutoMapper;

using CommunityCar.Application.Features.Account.ViewModels.Media;
using CommunityCar.Domain.Entities.Account.Media;

namespace CommunityCar.Application.Features.Account.Mappings.Media;

public class GalleryMappingProfile : AutoMapper.Profile
{
    public GalleryMappingProfile()
    {
        CreateGalleryMappings();
    }

    private void CreateGalleryMappings()
    {

        
        CreateMap<UserGallery, UserGalleryItemVM>()
            .ForMember(dest => dest.ThumbnailUrl, opt => opt.Ignore())
            .ForMember(dest => dest.FileSizeFormatted, opt => opt.Ignore())
            .ForMember(dest => dest.UploadedTimeAgo, opt => opt.Ignore())
            .ForMember(dest => dest.PrivacyIcon, opt => opt.Ignore())
            .ForMember(dest => dest.PrivacyText, opt => opt.Ignore());

        CreateMap<AddGalleryItemRequest, UserGallery>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ViewCount, opt => opt.Ignore())
            .ForMember(dest => dest.DisplayOrder, opt => opt.Ignore());

        CreateMap<UpdateGalleryItemRequest, UserGallery>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
            .ForMember(dest => dest.FileSize, opt => opt.Ignore())
            .ForMember(dest => dest.MimeType, opt => opt.Ignore())
            .ForMember(dest => dest.ViewCount, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}