using CommunityCar.Application.Common.Interfaces.Services.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Storage;

public class FileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<FileStorageService> _logger;
    private readonly string _uploadsPath;

    public FileStorageService(IWebHostEnvironment environment, ILogger<FileStorageService> logger)
    {
        _environment = environment;
        _logger = logger;
        _uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
        
        // Ensure uploads directory exists
        if (!Directory.Exists(_uploadsPath))
        {
            Directory.CreateDirectory(_uploadsPath);
        }
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        try
        {
            var uniqueFileName = await GenerateUniqueFileNameAsync(fileName);
            var filePath = Path.Combine(_uploadsPath, uniqueFileName);

            using var fileStreamOutput = new FileStream(filePath, FileMode.Create);
            await fileStream.CopyToAsync(fileStreamOutput);

            return $"/uploads/{uniqueFileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName}", fileName);
            throw;
        }
    }

    public async Task<string> UploadFileAsync(byte[] fileData, string fileName, string contentType)
    {
        try
        {
            var uniqueFileName = await GenerateUniqueFileNameAsync(fileName);
            var filePath = Path.Combine(_uploadsPath, uniqueFileName);

            await File.WriteAllBytesAsync(filePath, fileData);

            return $"/uploads/{uniqueFileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName}", fileName);
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(string fileUrl)
    {
        try
        {
            if (string.IsNullOrEmpty(fileUrl) || !fileUrl.StartsWith("/uploads/"))
                return false;

            var fileName = Path.GetFileName(fileUrl);
            var filePath = Path.Combine(_uploadsPath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FileUrl}", fileUrl);
            return false;
        }
    }

    public async Task<Stream> DownloadFileAsync(string fileUrl)
    {
        try
        {
            if (string.IsNullOrEmpty(fileUrl) || !fileUrl.StartsWith("/uploads/"))
                throw new FileNotFoundException("File not found");

            var fileName = Path.GetFileName(fileUrl);
            var filePath = Path.Combine(_uploadsPath, fileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found");

            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file {FileUrl}", fileUrl);
            throw;
        }
    }

    public async Task<bool> FileExistsAsync(string fileUrl)
    {
        try
        {
            if (string.IsNullOrEmpty(fileUrl) || !fileUrl.StartsWith("/uploads/"))
                return false;

            var fileName = Path.GetFileName(fileUrl);
            var filePath = Path.Combine(_uploadsPath, fileName);

            return File.Exists(filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking file existence {FileUrl}", fileUrl);
            return false;
        }
    }

    public async Task<string> GetFileUrlAsync(string fileName)
    {
        return $"/uploads/{fileName}";
    }

    public async Task<long> GetFileSizeAsync(string fileUrl)
    {
        try
        {
            if (string.IsNullOrEmpty(fileUrl) || !fileUrl.StartsWith("/uploads/"))
                return 0;

            var fileName = Path.GetFileName(fileUrl);
            var filePath = Path.Combine(_uploadsPath, fileName);

            if (!File.Exists(filePath))
                return 0;

            var fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file size {FileUrl}", fileUrl);
            return 0;
        }
    }

    public string GetFileExtension(string fileName)
    {
        return Path.GetExtension(fileName).ToLowerInvariant();
    }

    public bool IsValidImageFile(string fileName)
    {
        var extension = GetFileExtension(fileName);
        var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        return validExtensions.Contains(extension);
    }

    public bool IsValidVideoFile(string fileName)
    {
        var extension = GetFileExtension(fileName);
        var validExtensions = new[] { ".mp4", ".avi", ".mov", ".wmv", ".flv", ".webm" };
        return validExtensions.Contains(extension);
    }

    public async Task<string> GenerateUniqueFileNameAsync(string originalFileName)
    {
        var extension = GetFileExtension(originalFileName);
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        
        return $"{nameWithoutExtension}_{timestamp}_{uniqueId}{extension}";
    }
}