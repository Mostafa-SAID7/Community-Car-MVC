namespace CommunityCar.Application.Common.Interfaces.Services.Storage;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
    Task<string> UploadFileAsync(byte[] fileData, string fileName, string contentType);
    Task<bool> DeleteFileAsync(string fileUrl);
    Task<Stream> DownloadFileAsync(string fileUrl);
    Task<bool> FileExistsAsync(string fileUrl);
    Task<string> GetFileUrlAsync(string fileName);
    Task<long> GetFileSizeAsync(string fileUrl);
    string GetFileExtension(string fileName);
    bool IsValidImageFile(string fileName);
    bool IsValidVideoFile(string fileName);
    Task<string> GenerateUniqueFileNameAsync(string originalFileName);
}