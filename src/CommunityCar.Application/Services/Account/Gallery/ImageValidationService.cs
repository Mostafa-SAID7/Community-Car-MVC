using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Account.Gallery;

/// <summary>
/// Service for image validation
/// </summary>
public class ImageValidationService : IImageValidationService
{
    private readonly ILogger<ImageValidationService> _logger;

    // Configuration constants
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
    private static readonly string[] AllowedFormats = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    public ImageValidationService(ILogger<ImageValidationService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> IsValidImageAsync(byte[] imageData)
    {
        try
        {
            // Basic validation - check for image headers
            if (imageData.Length < 4) return false;

            // Check for common image file signatures
            var header = imageData.Take(4).ToArray();
            
            // JPEG
            if (header[0] == 0xFF && header[1] == 0xD8) return true;
            
            // PNG
            if (header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47) return true;
            
            // GIF
            if (header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46) return true;

            // WebP
            if (imageData.Length >= 12)
            {
                var riffHeader = imageData.Take(4).ToArray();
                var webpHeader = imageData.Skip(8).Take(4).ToArray();
                if (riffHeader[0] == 0x52 && riffHeader[1] == 0x49 && riffHeader[2] == 0x46 && riffHeader[3] == 0x46 &&
                    webpHeader[0] == 0x57 && webpHeader[1] == 0x45 && webpHeader[2] == 0x42 && webpHeader[3] == 0x50)
                {
                    return true;
                }
            }

            _logger.LogWarning("Invalid image format detected, header: {Header}", 
                string.Join(" ", header.Select(b => b.ToString("X2"))));
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating image data");
            return false;
        }
    }

    public async Task<bool> IsImageSizeAllowedAsync(long fileSize)
    {
        var isAllowed = fileSize <= MaxFileSize;
        
        if (!isAllowed)
        {
            _logger.LogWarning("Image size {Size} bytes exceeds maximum allowed size {MaxSize} bytes", 
                fileSize, MaxFileSize);
        }
        
        return await Task.FromResult(isAllowed);
    }

    public async Task<IEnumerable<string>> GetAllowedImageFormatsAsync()
    {
        return await Task.FromResult(AllowedFormats.AsEnumerable());
    }
}


