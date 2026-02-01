using System.Text;

namespace CommunityCar.Web.Helpers;

/// <summary>
/// Helper class for file operations and management
/// </summary>
public static class FileHelper
{
    /// <summary>
    /// Allowed image file extensions
    /// </summary>
    public static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };

    /// <summary>
    /// Allowed document file extensions
    /// </summary>
    public static readonly string[] DocumentExtensions = { ".pdf", ".doc", ".docx", ".txt", ".rtf" };

    /// <summary>
    /// Allowed video file extensions
    /// </summary>
    public static readonly string[] VideoExtensions = { ".mp4", ".avi", ".mov", ".wmv", ".flv", ".webm" };

    /// <summary>
    /// Allowed audio file extensions
    /// </summary>
    public static readonly string[] AudioExtensions = { ".mp3", ".wav", ".ogg", ".m4a", ".aac" };

    /// <summary>
    /// Maximum file sizes by type (in bytes)
    /// </summary>
    public static readonly Dictionary<string, long> MaxFileSizes = new()
    {
        { "image", 5 * 1024 * 1024 }, // 5MB
        { "document", 10 * 1024 * 1024 }, // 10MB
        { "video", 100 * 1024 * 1024 }, // 100MB
        { "audio", 20 * 1024 * 1024 } // 20MB
    };

    /// <summary>
    /// Gets the file type category based on extension
    /// </summary>
    public static string GetFileTypeCategory(string fileName)
    {
        var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
        
        if (ImageExtensions.Contains(extension))
            return "image";
        
        if (DocumentExtensions.Contains(extension))
            return "document";
        
        if (VideoExtensions.Contains(extension))
            return "video";
        
        if (AudioExtensions.Contains(extension))
            return "audio";
        
        return "other";
    }

    /// <summary>
    /// Validates if a file is allowed based on extension and size
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidateFile(IFormFile file, string[] allowedExtensions, long maxSizeBytes)
    {
        if (file == null || file.Length == 0)
            return (false, "No file provided");

        var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
        
        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
            return (false, $"File type not allowed. Allowed types: {string.Join(", ", allowedExtensions)}");

        if (file.Length > maxSizeBytes)
        {
            var maxSizeMB = maxSizeBytes / (1024 * 1024);
            return (false, $"File size exceeds maximum allowed size of {maxSizeMB} MB");
        }

        return (true, string.Empty);
    }

    /// <summary>
    /// Generates a unique file name while preserving the extension
    /// </summary>
    public static string GenerateUniqueFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var guid = Guid.NewGuid().ToString("N")[..8];
        
        return $"{timestamp}_{guid}{extension}";
    }

    /// <summary>
    /// Sanitizes a file name by removing invalid characters
    /// </summary>
    public static string SanitizeFileName(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return "file";

        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new StringBuilder();

        foreach (var c in fileName)
        {
            if (!invalidChars.Contains(c))
                sanitized.Append(c);
            else
                sanitized.Append('_');
        }

        var result = sanitized.ToString().Trim();
        
        // Ensure the file name is not empty and not too long
        if (string.IsNullOrEmpty(result))
            result = "file";
        
        if (result.Length > 100)
            result = result.Substring(0, 100);

        return result;
    }

    /// <summary>
    /// Gets the MIME type based on file extension
    /// </summary>
    public static string GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
        
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".bmp" => "image/bmp",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".txt" => "text/plain",
            ".rtf" => "application/rtf",
            ".mp4" => "video/mp4",
            ".avi" => "video/x-msvideo",
            ".mov" => "video/quicktime",
            ".wmv" => "video/x-ms-wmv",
            ".flv" => "video/x-flv",
            ".webm" => "video/webm",
            ".mp3" => "audio/mpeg",
            ".wav" => "audio/wav",
            ".ogg" => "audio/ogg",
            ".m4a" => "audio/mp4",
            ".aac" => "audio/aac",
            _ => "application/octet-stream"
        };
    }

    /// <summary>
    /// Formats file size in human-readable format
    /// </summary>
    public static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        
        return $"{len:0.##} {sizes[order]}";
    }

    /// <summary>
    /// Creates a directory if it doesn't exist
    /// </summary>
    public static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    /// <summary>
    /// Saves a file to the specified path
    /// </summary>
    public static async Task<string> SaveFileAsync(IFormFile file, string uploadPath, string? customFileName = null)
    {
        EnsureDirectoryExists(uploadPath);
        
        var fileName = customFileName ?? GenerateUniqueFileName(file.FileName);
        var filePath = Path.Combine(uploadPath, fileName);
        
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
        
        return fileName;
    }

    /// <summary>
    /// Deletes a file if it exists
    /// </summary>
    public static bool DeleteFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets file information
    /// </summary>
    public static FileInfo? GetFileInfo(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                return new FileInfo(filePath);
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Reads file content as string
    /// </summary>
    public static async Task<string?> ReadFileAsStringAsync(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                return await File.ReadAllTextAsync(filePath);
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Writes string content to file
    /// </summary>
    public static async Task<bool> WriteStringToFileAsync(string filePath, string content)
    {
        try
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                EnsureDirectoryExists(directory);
            }
            
            await File.WriteAllTextAsync(filePath, content);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Copies a file to a new location
    /// </summary>
    public static bool CopyFile(string sourcePath, string destinationPath, bool overwrite = false)
    {
        try
        {
            if (File.Exists(sourcePath))
            {
                var directory = Path.GetDirectoryName(destinationPath);
                if (!string.IsNullOrEmpty(directory))
                {
                    EnsureDirectoryExists(directory);
                }
                
                File.Copy(sourcePath, destinationPath, overwrite);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Moves a file to a new location
    /// </summary>
    public static bool MoveFile(string sourcePath, string destinationPath)
    {
        try
        {
            if (File.Exists(sourcePath))
            {
                var directory = Path.GetDirectoryName(destinationPath);
                if (!string.IsNullOrEmpty(directory))
                {
                    EnsureDirectoryExists(directory);
                }
                
                File.Move(sourcePath, destinationPath);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets all files in a directory with optional pattern
    /// </summary>
    public static string[] GetFiles(string directoryPath, string searchPattern = "*.*")
    {
        try
        {
            if (Directory.Exists(directoryPath))
            {
                return Directory.GetFiles(directoryPath, searchPattern);
            }
            return Array.Empty<string>();
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

    /// <summary>
    /// Cleans up old files in a directory
    /// </summary>
    public static int CleanupOldFiles(string directoryPath, TimeSpan maxAge)
    {
        try
        {
            if (!Directory.Exists(directoryPath))
                return 0;

            var cutoffDate = DateTime.UtcNow - maxAge;
            var files = Directory.GetFiles(directoryPath);
            var deletedCount = 0;

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                if (fileInfo.CreationTimeUtc < cutoffDate)
                {
                    try
                    {
                        File.Delete(file);
                        deletedCount++;
                    }
                    catch
                    {
                        // Continue with other files if one fails
                    }
                }
            }

            return deletedCount;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Gets the total size of all files in a directory
    /// </summary>
    public static long GetDirectorySize(string directoryPath)
    {
        try
        {
            if (!Directory.Exists(directoryPath))
                return 0;

            var files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);
            return files.Sum(file => new FileInfo(file).Length);
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Validates image file by checking file signature (magic numbers)
    /// Note: For full image dimension validation, consider using ImageSharp or similar library
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidateImageFile(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return (false, "No file provided");

            var buffer = new byte[12]; // Increased to 12 bytes for WebP validation
            using var stream = file.OpenReadStream();
            var bytesRead = stream.Read(buffer, 0, 12);
            stream.Position = 0; // Reset stream position

            if (bytesRead < 8) // Minimum bytes needed for most formats
                return (false, "File too small to be a valid image");

            // Check for common image file signatures
            if (IsJpegFile(buffer) || IsPngFile(buffer) || IsGifFile(buffer) || IsWebpFile(buffer))
            {
                return (true, string.Empty);
            }

            return (false, "Invalid image file format");
        }
        catch
        {
            return (false, "Error validating image file");
        }
    }

    /// <summary>
    /// Checks if file is a JPEG based on magic numbers
    /// </summary>
    private static bool IsJpegFile(byte[] buffer)
    {
        return buffer.Length >= 3 && 
               buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF;
    }

    /// <summary>
    /// Checks if file is a PNG based on magic numbers
    /// </summary>
    private static bool IsPngFile(byte[] buffer)
    {
        return buffer.Length >= 8 && 
               buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47 &&
               buffer[4] == 0x0D && buffer[5] == 0x0A && buffer[6] == 0x1A && buffer[7] == 0x0A;
    }

    /// <summary>
    /// Checks if file is a GIF based on magic numbers
    /// </summary>
    private static bool IsGifFile(byte[] buffer)
    {
        return buffer.Length >= 6 && 
               buffer[0] == 0x47 && buffer[1] == 0x49 && buffer[2] == 0x46 &&
               buffer[3] == 0x38 && (buffer[4] == 0x37 || buffer[4] == 0x39) && buffer[5] == 0x61;
    }

    /// <summary>
    /// Checks if file is a WebP based on magic numbers
    /// </summary>
    private static bool IsWebpFile(byte[] buffer)
    {
        return buffer.Length >= 12 && 
               buffer[0] == 0x52 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x46 &&
               buffer[8] == 0x57 && buffer[9] == 0x45 && buffer[10] == 0x42 && buffer[11] == 0x50;
    }

    /// <summary>
    /// Placeholder for thumbnail creation - requires image processing library
    /// Consider using ImageSharp, SkiaSharp, or similar for actual implementation
    /// </summary>
    public static async Task<string?> CreateThumbnailPlaceholderAsync(string imagePath, string thumbnailPath, int width, int height)
    {
        // This is a placeholder method. For actual thumbnail creation, you would need:
        // 1. Install ImageSharp: dotnet add package SixLabors.ImageSharp
        // 2. Implement actual image resizing logic
        
        await Task.CompletedTask; // Placeholder to make method async
        
        // Return null to indicate thumbnail creation is not implemented
        // In a real implementation, this would create and save a thumbnail
        return null;
    }
}