using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Features.Account.ViewModels.Media;

namespace CommunityCar.Application.Common.Interfaces.Services.Account.Media;

public interface IUserGalleryService
{
    Task<Result<List<GalleryItemVM>>> GetUserGalleryAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<Result<GalleryItemVM>> GetGalleryItemAsync(Guid itemId);
    Task<Result<GalleryItemVM>> AddGalleryItemAsync(Guid userId, CreateGalleryItemVM request);
    Task<Result<GalleryItemVM>> UpdateGalleryItemAsync(Guid itemId, UpdateGalleryItemVM request);
    Task<Result> DeleteGalleryItemAsync(Guid itemId);
    Task<Result<List<GalleryCollectionVM>>> GetUserCollectionsAsync(Guid userId);
    Task<Result<GalleryCollectionVM>> CreateCollectionAsync(Guid userId, CreateGalleryCollectionVM request);
    Task<Result<GalleryCollectionVM>> UpdateCollectionAsync(Guid collectionId, UpdateGalleryCollectionVM request);
    Task<Result> DeleteCollectionAsync(Guid collectionId);
    Task<Result> AddItemToCollectionAsync(Guid itemId, Guid collectionId);
    Task<Result> RemoveItemFromCollectionAsync(Guid itemId, Guid collectionId);
    Task<Result<GalleryStatsVM>> GetGalleryStatsAsync(Guid userId);
}