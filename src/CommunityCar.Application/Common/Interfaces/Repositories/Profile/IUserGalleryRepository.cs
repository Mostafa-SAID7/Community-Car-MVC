using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Profile;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Profile;

public interface IUserGalleryRepository : IBaseRepository<UserGallery>
{
    Task<IEnumerable<UserGallery>> GetUserGalleryAsync(Guid userId, bool publicOnly = false);
    Task<IEnumerable<UserGallery>> GetFeaturedGalleryAsync(Guid userId);
    Task<IEnumerable<UserGallery>> GetGalleryByTypeAsync(Guid userId, MediaType mediaType, bool publicOnly = false);
    Task<IEnumerable<UserGallery>> GetGalleryByTagAsync(Guid userId, string tag, bool publicOnly = false);
    Task<int> GetGalleryCountAsync(Guid userId, bool publicOnly = false);
    Task<IEnumerable<UserGallery>> GetRecentGalleryAsync(Guid userId, int count = 10, bool publicOnly = false);
    Task<IEnumerable<UserGallery>> GetPopularGalleryAsync(Guid userId, int count = 10, bool publicOnly = false);
    Task<IEnumerable<string>> GetPopularTagsAsync(Guid userId);
    Task<UserGallery?> GetGalleryItemAsync(Guid itemId, Guid? userId = null);
    Task<bool> CanAccessGalleryItemAsync(Guid itemId, Guid? userId = null);
}


