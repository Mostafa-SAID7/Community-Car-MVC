using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;

using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;

using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;

namespace CommunityCar.Application.Services.Account.Gallery;

/// <summary>
/// Interface for gallery CRUD operations
/// </summary>
public interface IGalleryManagementService
{
    Task<IEnumerable<UserGalleryItemVM>> GetUserGalleryAsync(Guid userId);
    Task<UserGalleryItemVM?> GetGalleryItemAsync(Guid userId, Guid imageId);
    Task<UserGalleryItemVM?> UploadImageAsync(UploadImageRequest request);
    Task<bool> DeleteImageAsync(Guid userId, Guid imageId);
}


