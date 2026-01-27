namespace CommunityCar.Application.Services.Account.Gallery;

/// <summary>
/// Interface for gallery storage management
/// </summary>
public interface IGalleryStorageService
{
    Task<int> GetImageCountAsync(Guid userId);
    Task<long> GetTotalStorageUsedAsync(Guid userId);
    Task<bool> IsStorageLimitExceededAsync(Guid userId);
}


