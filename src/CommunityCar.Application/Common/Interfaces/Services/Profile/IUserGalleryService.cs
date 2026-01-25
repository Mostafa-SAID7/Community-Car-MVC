using CommunityCar.Application.Features.Profile.DTOs;

namespace CommunityCar.Application.Common.Interfaces.Services.Profile;

public interface IUserGalleryService
{
    Task<List<UserGalleryItemDTO>> GetUserGalleryAsync(Guid userId, GalleryFilterRequest? filter = null);
    Task<UserGalleryItemDTO?> GetGalleryItemAsync(Guid itemId);
    Task<Guid> CreateGalleryItemAsync(Guid userId, CreateGalleryItemRequest request, Stream mediaStream, string fileName);
    Task<bool> UpdateGalleryItemAsync(Guid itemId, UpdateGalleryItemRequest request);
    Task<bool> DeleteGalleryItemAsync(Guid itemId, Guid userId);
    Task<bool> ToggleFeaturedAsync(Guid itemId);
    Task<bool> ToggleVisibilityAsync(Guid itemId);
    Task<bool> ToggleItemVisibilityAsync(Guid itemId, Guid userId);
    Task<bool> IncrementViewsAsync(Guid itemId);
    Task<bool> ToggleLikeAsync(Guid itemId, Guid userId);
    Task<List<string>> GetPopularTagsAsync(Guid userId);
}