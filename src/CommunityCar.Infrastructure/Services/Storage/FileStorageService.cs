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
            var fileStorageSettings = _configuration.GetSection("FileStorage");
            var provider = fileStorageSettings["Provider"];
            var localPath = fileStorageSettings["LocalPath"];

            if (provider == "Local")
            {
                return await UploadToLocalAsync(fileStream, fileName, localPath!);
            }

            // Add other providers (Azure Blob, AWS S3) here
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
        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), basePath);
        Directory.CreateDirectory(uploadsPath);

        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(uploadsPath, uniqueFileName);

        using var fileStreamLocal = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(fileStreamLocal);

        _logger.LogInformation("File uploaded to local storage: {FilePath}", filePath);
        return $"/{basePath}/{uniqueFileName}";
    }

    private Task<bool> DeleteFromLocalAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath.TrimStart('/'));
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
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath.TrimStart('/'));
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