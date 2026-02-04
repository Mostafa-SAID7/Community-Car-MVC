namespace CommunityCar.Application.Features.Dashboard.Storage.ViewModels;

public class FileInfoVM
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public string Owner { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
}