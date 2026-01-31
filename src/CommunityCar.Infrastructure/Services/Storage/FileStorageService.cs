using CommunityCar.Application.Common.Interfaces.Services.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Services.Storage;

public class FileStorageService : IFileStorageService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FileStorageService> _logger;

    public FileStorageService(IConfiguration configuration, ILogger<FileStorageService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        try
        {
            _logger.LogInformation("Starting file upload - fileName: {FileName}, contentType: {ContentType}", fileName, contentType);
            
            var fileStorageSettings = _configuration.GetSection("FileStorage");
            var provider = fileStorageSettings["Provider"];
            var localPath = fileStorageSettings["LocalPath"];

            _logger.LogInformation("File storage configuration - provider: {Provider}, localPath: {LocalPath}", provider, localPath);

            if (provider == "Local")
            {
                if (string.IsNullOrEmpty(localPath))
                {
                    _logger.LogError("LocalPath configuration is missing or empty");
                    throw new InvalidOperationException("LocalPath configuration is missing");
                }
                
                return await UploadToLocalAsync(fileStream, fileName, localPath);
            }

            // Add other providers (Azure Blob, AWS S3) here
            _logger.LogError("Unsupported file storage provider: {Provider}", provider);
            throw new NotSupportedException($"File storage provider '{provider}' is not supported");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName}", fileName);
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(string filePath)
    {
        try
        {
            var fileStorageSettings = _configuration.GetSection("FileStorage");
            var provider = fileStorageSettings["Provider"];

            if (provider == "Local")
            {
                return await DeleteFromLocalAsync(filePath);
            }

            throw new NotSupportedException($"File storage provider '{provider}' is not supported");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FilePath}", filePath);
            return false;
        }
    }

    public async Task<Stream?> GetFileAsync(string filePath)
    {
        try
        {
            var fileStorageSettings = _configuration.GetSection("FileStorage");
            var provider = fileStorageSettings["Provider"];

            if (provider == "Local")
            {
                return await GetFromLocalAsync(filePath);
            }

            throw new NotSupportedException($"File storage provider '{provider}' is not supported");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file {FilePath}", filePath);
            return null;
        }
    }

    private async Task<string> UploadToLocalAsync(Stream fileStream, string fileName, string basePath)
    {
        try
        {
            // Extract directory structure from fileName if it contains path separators
            var safeFileName = Path.GetFileName(fileName);
            var directoryPath = Path.GetDirectoryName(fileName);
            
            _logger.LogInformation("Processing upload - fileName: {FileName}, safeFileName: {SafeFileName}, directoryPath: {DirectoryPath}, basePath: {BasePath}", 
                fileName, safeFileName, directoryPath, basePath);
            
            // Build the full upload path
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", basePath.TrimStart('/'));
            
            // If fileName contains directory structure, add it to the path
            if (!string.IsNullOrEmpty(directoryPath))
            {
                uploadsPath = Path.Combine(uploadsPath, directoryPath);
            }
            
            _logger.LogInformation("Full upload path: {UploadsPath}", uploadsPath);
            
            // Ensure the full directory structure exists
            Directory.CreateDirectory(uploadsPath);
            _logger.LogInformation("Directory created/verified: {UploadsPath}", uploadsPath);

            // Create a unique filename to avoid conflicts
            var uniqueFileName = $"{Guid.NewGuid()}_{safeFileName}";
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            _logger.LogInformation("Writing file to: {FilePath}", filePath);

            using var fileStreamLocal = new FileStream(filePath, FileMode.Create);
            await fileStream.CopyToAsync(fileStreamLocal);

            _logger.LogInformation("File uploaded to local storage: {FilePath}", filePath);
            
            // Return the web-accessible path
            var webPath = !string.IsNullOrEmpty(directoryPath) 
                ? $"/{basePath.TrimStart('/')}/{directoryPath.Replace('\\', '/')}/{uniqueFileName}"
                : $"/{basePath.TrimStart('/')}/{uniqueFileName}";
                
            _logger.LogInformation("Returning web path: {WebPath}", webPath);
            return webPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UploadToLocalAsync - fileName: {FileName}, basePath: {BasePath}", fileName, basePath);
            throw;
        }
    }

    private Task<bool> DeleteFromLocalAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                _logger.LogInformation("File deleted from local storage: {FilePath}", filePath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file from local storage: {FilePath}", filePath);
            return Task.FromResult(false);
        }
    }

    private Task<Stream?> GetFromLocalAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                return Task.FromResult<Stream?>(new FileStream(fullPath, FileMode.Open, FileAccess.Read));
            }
            return Task.FromResult<Stream?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file from local storage: {FilePath}", filePath);
            return Task.FromResult<Stream?>(null);
        }
    }
}
