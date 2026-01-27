namespace CommunityCar.Application.Services.Account.Gallery;

/// <summary>
/// Interface for image operations (profile picture, cover image, etc.)
/// </summary>
public interface IImageOperationsService
{
    Task<bool> SetAsProfilePictureAsync(Guid userId, Guid imageId);
    Task<bool> SetAsCoverImageAsync(Guid userId, Guid imageId);
    Task<bool> UpdateImageCaptionAsync(Guid userId, Guid imageId, string caption);
    Task<bool> ReorderImagesAsync(Guid userId, IEnumerable<Guid> imageIds);
    Task<bool> ToggleItemVisibilityAsync(Guid userId, Guid imageId);
}


