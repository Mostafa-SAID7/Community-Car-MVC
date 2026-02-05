using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Account.Media;

namespace CommunityCar.Web.Areas.Identity.Interfaces.Repositories;

/// <summary>
/// Repository interface for UserGallery entity operations
/// </summary>
public interface IUserGalleryRepository : IBaseRepository<UserGallery>
{
    #region Gallery Management
    Task<IEnumerable<UserGallery>> GetUserGalleryAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<UserGallery?> GetGalleryItemAsync(Guid userId, Guid itemId);
    Task<bool> AddGalleryItemAsync(Guid userId, string imageUrl, string? caption = null, Dictionary<string, object>? metadata = null);
    Task<bool> UpdateGalleryItemAsync(Guid itemId, string? caption = null, Dictionary<string, object>? metadata = null);
    Task<bool> RemoveGalleryItemAsync(Guid itemId);
    Task<bool> RemoveUserGalleryAsync(Guid userId);
    #endregion

    #region Gallery Organization
    Task<bool> SetGalleryItemOrderAsync(Guid userId, Dictionary<Guid, int> itemOrders);
    Task<IEnumerable<UserGallery>> GetGalleryByTagAsync(Guid userId, string tag);
    Task<bool> AddTagToGalleryItemAsync(Guid itemId, string tag);
    Task<bool> RemoveTagFromGalleryItemAsync(Guid itemId, string tag);
    Task<IEnumerable<string>> GetGalleryTagsAsync(Guid userId);
    #endregion

    #region Gallery Analytics
    Task<int> GetGalleryItemCountAsync(Guid userId);
    Task<IEnumerable<UserGallery>> GetRecentGalleryItemsAsync(Guid userId, int count = 10);
    Task<IEnumerable<UserGallery>> GetPopularGalleryItemsAsync(Guid userId, int count = 10);
    Task<long> GetGalleryStorageSizeAsync(Guid userId);
    #endregion

    #region Gallery Privacy
    Task<bool> SetGalleryItemVisibilityAsync(Guid itemId, bool isPublic);
    Task<IEnumerable<UserGallery>> GetPublicGalleryAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<IEnumerable<UserGallery>> GetPrivateGalleryAsync(Guid userId, int page = 1, int pageSize = 20);
    #endregion
}
