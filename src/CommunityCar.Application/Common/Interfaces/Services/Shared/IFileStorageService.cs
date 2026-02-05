namespace CommunityCar.Application.Common.Interfaces.Services.Shared;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
    Task<bool> DeleteFileAsync(string fileUrl);
    Task<Stream> DownloadFileAsync(string fileUrl);
    Task<bool> FileExistsAsync(string fileUrl);
    Task<long> GetFileSizeAsync(string fileUrl);
    string GetFileUrl(string fileName);
}