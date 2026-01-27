namespace CommunityCar.Application.Services.Account.Gallery;

/// <summary>
/// Interface for image validation
/// </summary>
public interface IImageValidationService
{
    Task<bool> IsValidImageAsync(byte[] imageData);
    Task<bool> IsImageSizeAllowedAsync(long fileSize);
    Task<IEnumerable<string>> GetAllowedImageFormatsAsync();
}


